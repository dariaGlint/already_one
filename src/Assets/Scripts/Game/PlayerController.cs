using System;
using AlreadyOne.Data;
using UnityEngine;

namespace AlreadyOne.Game
{
    [DisallowMultipleComponent]
    public sealed class PlayerController : MonoBehaviour
    {
        private static Sprite s_playerSprite;

        [Header("Rendering")]
        [SerializeField] private Color _playerColor = new Color32(0x33, 0x66, 0xCC, 0xFF);
        [SerializeField] private int _sortingOrder = 10;

        private StageData _stageData;
        private Transform _boardTransform;
        private SpriteRenderer _spriteRenderer;
        private bool[] _openedDoors = Array.Empty<bool>();
        private bool _isFront = true;

        public event Action<int, int> OnMoved;

        public int GridX { get; private set; }

        public int GridY { get; private set; }

        private void Awake()
        {
            if (!TryGetComponent(out _spriteRenderer))
            {
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            _spriteRenderer.sprite = GetPlayerSprite();
            _spriteRenderer.color = _playerColor;
            _spriteRenderer.sortingOrder = _sortingOrder;
        }

        private void Update()
        {
            if (_stageData == null)
            {
                return;
            }

            Vector2Int move = GetMoveInput();
            if (move == Vector2Int.zero)
            {
                return;
            }

            TryMove(GridX + move.x, GridY + move.y);
        }

        public void SetPosition(int x, int y)
        {
            GridX = x;
            GridY = y;
            UpdateWorldPosition();
        }

        public void SetStageData(StageData data, bool isFront)
        {
            _stageData = data;
            _isFront = isFront;
        }

        public void SetDoorStates(bool[] openedDoors)
        {
            _openedDoors = openedDoors ?? Array.Empty<bool>();
        }

        public void SetBoardTransform(Transform boardTransform)
        {
            _boardTransform = boardTransform;
            UpdateWorldPosition();
        }

        private Vector2Int GetMoveInput()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                return Vector2Int.up;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                return Vector2Int.down;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return Vector2Int.left;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                return Vector2Int.right;
            }

            return Vector2Int.zero;
        }

        private void TryMove(int targetX, int targetY)
        {
            if (!_stageData.IsWalkable(targetX, targetY, _isFront))
            {
                return;
            }

            if (IsClosedDoor(targetX, targetY))
            {
                return;
            }

            SetPosition(targetX, targetY);
            OnMoved?.Invoke(GridX, GridY);
        }

        private bool IsClosedDoor(int x, int y)
        {
            if (!_isFront)
            {
                return false;
            }

            PositionData[] doors = _stageData.Doors;
            if (doors == null)
            {
                return false;
            }

            for (int i = 0; i < doors.Length; i++)
            {
                PositionData door = doors[i];
                if (door == null || door.X != x || door.Y != y)
                {
                    continue;
                }

                return i >= _openedDoors.Length || !_openedDoors[i];
            }

            return false;
        }

        private void UpdateWorldPosition()
        {
            Vector3 localGridPosition = new Vector3(GridX + 0.5f, GridY + 0.5f, 0f);
            transform.position = _boardTransform != null ? _boardTransform.TransformPoint(localGridPosition) : localGridPosition;
        }

        private static Sprite GetPlayerSprite()
        {
            if (s_playerSprite != null)
            {
                return s_playerSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            s_playerSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            s_playerSprite.hideFlags = HideFlags.HideAndDontSave;
            return s_playerSprite;
        }
    }
}
