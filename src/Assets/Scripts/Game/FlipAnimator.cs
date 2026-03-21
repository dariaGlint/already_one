using System.Collections;
using UnityEngine;

namespace AlreadyOne.Game
{
    [DisallowMultipleComponent]
    public sealed class FlipAnimator : MonoBehaviour
    {
        private const float MinimumFlipDuration = 0.01f;
        private const float ShakeDuration = 0.15f;
        private const float ShakeAmplitude = 0.08f;
        private const float ShakeCycles = 4f;

        [Header("Scene References")]
        [SerializeField] private MazeRenderer _mazeRenderer;
        [SerializeField] private FlipManager _flipManager;
        [SerializeField] private PlayerController _playerController;

        [Header("Animation")]
        [SerializeField] [Min(MinimumFlipDuration)] private float _flipDuration = 0.3f;

        private GameManager _gameManager;
        private Coroutine _flipCoroutine;
        private Coroutine _shakeCoroutine;
        private Vector3 _mazeBaseLocalScale = Vector3.one;
        private Vector3 _mazeBaseLocalPosition = Vector3.zero;
        private Vector3 _playerBaseLocalScale = Vector3.one;
        private bool _hasPendingFlip;
        private bool _pendingIsFront;
        private bool _inputLocked;

        private void Awake()
        {
            CacheReferences();
            CaptureBaseTransforms();
        }

        private void OnEnable()
        {
            CacheReferences();

            if (_flipManager != null)
            {
                _flipManager.OnFlipped += HandleFlipped;
                _flipManager.OnFlipFailed += HandleFlipFailed;
            }
        }

        private void LateUpdate()
        {
            if (!_hasPendingFlip)
            {
                return;
            }

            _hasPendingFlip = false;
            BeginFlipAnimation(_pendingIsFront);
        }

        private void OnDisable()
        {
            if (_flipManager != null)
            {
                _flipManager.OnFlipped -= HandleFlipped;
                _flipManager.OnFlipFailed -= HandleFlipFailed;
            }

            _hasPendingFlip = false;
            StopRunningAnimations();
            RestoreTransforms();
            UnlockGameplayInput();
        }

        private void HandleFlipped(bool newIsFront)
        {
            _pendingIsFront = newIsFront;
            _hasPendingFlip = true;
        }

        private void HandleFlipFailed()
        {
            if (_mazeRenderer == null)
            {
                return;
            }

            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }

            if (_flipCoroutine == null)
            {
                RestoreTransforms();
            }

            _shakeCoroutine = StartCoroutine(ShakeMaze());
        }

        private void BeginFlipAnimation(bool newIsFront)
        {
            if (_mazeRenderer == null)
            {
                return;
            }

            StopRunningAnimations();
            RestoreTransforms();
            CaptureBaseTransforms();
            LockGameplayInput();

            // GameManager also responds to OnFlipped immediately. Revert here so the face swap stays hidden until the midpoint.
            _mazeRenderer.SetFace(!newIsFront);
            _flipCoroutine = StartCoroutine(AnimateFlip(newIsFront));
        }

        private IEnumerator AnimateFlip(bool newIsFront)
        {
            float halfDuration = Mathf.Max(MinimumFlipDuration, _flipDuration) * 0.5f;
            float elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / halfDuration);
                SetHorizontalScale(Mathf.Lerp(_mazeBaseLocalScale.x, 0f, progress));
                yield return null;
            }

            SetHorizontalScale(0f);
            _mazeRenderer.SetFace(newIsFront);

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / halfDuration);
                SetHorizontalScale(Mathf.Lerp(0f, _mazeBaseLocalScale.x, progress));
                yield return null;
            }

            RestoreTransforms();
            _flipCoroutine = null;
            UnlockGameplayInput();
        }

        private IEnumerator ShakeMaze()
        {
            float elapsed = 0f;

            while (elapsed < ShakeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / ShakeDuration);
                float damping = 1f - progress;
                float angle = progress * ShakeCycles * Mathf.PI * 2f;
                SetHorizontalOffset(Mathf.Sin(angle) * ShakeAmplitude * damping);
                yield return null;
            }

            _shakeCoroutine = null;
            RestoreTransforms();
        }

        private void StopRunningAnimations()
        {
            if (_flipCoroutine != null)
            {
                StopCoroutine(_flipCoroutine);
                _flipCoroutine = null;
            }

            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = null;
            }
        }

        private void LockGameplayInput()
        {
            if (_inputLocked)
            {
                return;
            }

            _inputLocked = true;

            if (_playerController != null)
            {
                _playerController.enabled = false;
            }

            if (_flipManager != null)
            {
                _flipManager.enabled = false;
            }
        }

        private void UnlockGameplayInput()
        {
            if (!_inputLocked)
            {
                return;
            }

            _inputLocked = false;

            if (!ShouldRestoreGameplayInput())
            {
                return;
            }

            if (_playerController != null && _playerController.gameObject.activeInHierarchy)
            {
                _playerController.enabled = true;
            }

            if (_flipManager != null && _flipManager.gameObject.activeInHierarchy)
            {
                _flipManager.enabled = true;
            }
        }

        private bool ShouldRestoreGameplayInput()
        {
            return _gameManager == null || (_gameManager.gameObject.activeInHierarchy && _gameManager.enabled);
        }

        private void SetHorizontalScale(float scaleX)
        {
            if (_mazeRenderer == null)
            {
                return;
            }

            Transform mazeTransform = _mazeRenderer.transform;
            Vector3 mazeScale = _mazeBaseLocalScale;
            mazeScale.x = scaleX;
            mazeTransform.localScale = mazeScale;
            SyncPlayerTransform();
        }

        private void SetHorizontalOffset(float offsetX)
        {
            if (_mazeRenderer == null)
            {
                return;
            }

            Transform mazeTransform = _mazeRenderer.transform;
            Vector3 mazePosition = _mazeBaseLocalPosition;
            mazePosition.x += offsetX;
            mazeTransform.localPosition = mazePosition;
            SyncPlayerTransform();
        }

        private void RestoreTransforms()
        {
            if (_mazeRenderer != null)
            {
                Transform mazeTransform = _mazeRenderer.transform;
                mazeTransform.localScale = _mazeBaseLocalScale;
                mazeTransform.localPosition = _mazeBaseLocalPosition;
            }

            if (_playerController != null)
            {
                _playerController.transform.localScale = _playerBaseLocalScale;
            }

            SyncPlayerTransform();
        }

        private void SyncPlayerTransform()
        {
            if (_mazeRenderer == null || _playerController == null)
            {
                return;
            }

            Vector3 localGridPosition = new Vector3(_playerController.GridX + 0.5f, _playerController.GridY + 0.5f, 0f);
            _playerController.transform.position = _mazeRenderer.transform.TransformPoint(localGridPosition);

            if (Mathf.Approximately(_mazeBaseLocalScale.x, 0f))
            {
                return;
            }

            Vector3 playerScale = _playerBaseLocalScale;
            float scaleRatio = _mazeRenderer.transform.localScale.x / _mazeBaseLocalScale.x;
            playerScale.x = _playerBaseLocalScale.x * scaleRatio;
            _playerController.transform.localScale = playerScale;
        }

        private void CacheReferences()
        {
            if (_mazeRenderer == null)
            {
                TryGetComponent(out _mazeRenderer);
                _mazeRenderer = _mazeRenderer ?? Object.FindFirstObjectByType<MazeRenderer>();
            }

            if (_flipManager == null)
            {
                TryGetComponent(out _flipManager);
                _flipManager = _flipManager ?? Object.FindFirstObjectByType<FlipManager>();
            }

            if (_playerController == null)
            {
                TryGetComponent(out _playerController);
                _playerController = _playerController ?? Object.FindFirstObjectByType<PlayerController>();
            }

            if (_gameManager == null)
            {
                TryGetComponent(out _gameManager);
                _gameManager = _gameManager ?? Object.FindFirstObjectByType<GameManager>();
            }
        }

        private void CaptureBaseTransforms()
        {
            if (_mazeRenderer != null)
            {
                _mazeBaseLocalScale = _mazeRenderer.transform.localScale;
                _mazeBaseLocalPosition = _mazeRenderer.transform.localPosition;
            }

            if (_playerController != null)
            {
                _playerBaseLocalScale = _playerController.transform.localScale;
            }
        }
    }
}
