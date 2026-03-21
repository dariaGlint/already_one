using System;
using AlreadyOne.Data;
using UnityEngine;

namespace AlreadyOne.Game
{
    [DisallowMultipleComponent]
    public sealed class MazeRenderer : MonoBehaviour
    {
        private static Sprite s_cellSprite;

        [Header("Colors")]
        [SerializeField] private Color _frontFloorColor = Color.white;
        [SerializeField] private Color _frontWallColor = new Color32(0x1A, 0x1A, 0x1A, 0xFF);
        [SerializeField] private Color _goalColor = new Color32(0x00, 0xCC, 0x44, 0xFF);
        [SerializeField] private Color _startFrontColor = new Color32(0xDD, 0xE8, 0xFF, 0xFF);
        [SerializeField] private Color _startBackColor = new Color32(0x33, 0x55, 0x88, 0xFF);
        [SerializeField] private Color _interactionColor = new Color32(0xCC, 0x33, 0x33, 0xFF);
        [SerializeField] private Color _flipZoneColor = new Color32(0x99, 0x99, 0x99, 0xFF);

        [Header("Rendering")]
        [SerializeField] private int _sortingOrder;

        private StageData _stageData;
        private GameObject[,] _cellObjects;
        private SpriteRenderer[,] _cellRenderers;
        private bool[] _collectedKeys = Array.Empty<bool>();
        private bool[] _openedDoors = Array.Empty<bool>();
        private bool _isFront = true;

        public void Initialize(StageData stageData)
        {
            Clear();

            if (stageData == null)
            {
                Debug.LogError("Cannot initialize the maze without stage data.");
                return;
            }

            _stageData = stageData;
            _cellObjects = new GameObject[stageData.Width, stageData.Height];
            _cellRenderers = new SpriteRenderer[stageData.Width, stageData.Height];

            Sprite cellSprite = GetCellSprite();

            for (int y = 0; y < stageData.Height; y++)
            {
                for (int x = 0; x < stageData.Width; x++)
                {
                    GameObject cellObject = new GameObject($"Cell_{x}_{y}");
                    cellObject.transform.SetParent(transform, false);
                    cellObject.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 0f);

                    SpriteRenderer spriteRenderer = cellObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = cellSprite;
                    spriteRenderer.sortingOrder = _sortingOrder;

                    _cellObjects[x, y] = cellObject;
                    _cellRenderers[x, y] = spriteRenderer;
                }
            }

            RefreshVisuals();
        }

        public void SetFace(bool isFront)
        {
            _isFront = isFront;
            RefreshVisuals();
        }

        public void SetInteractionState(bool[] collectedKeys, bool[] openedDoors)
        {
            _collectedKeys = collectedKeys ?? Array.Empty<bool>();
            _openedDoors = openedDoors ?? Array.Empty<bool>();
            RefreshVisuals();
        }

        public void Clear()
        {
            if (_cellObjects != null)
            {
                int width = _cellObjects.GetLength(0);
                int height = _cellObjects.GetLength(1);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        GameObject cellObject = _cellObjects[x, y];
                        if (cellObject == null)
                        {
                            continue;
                        }

                        cellObject.SetActive(false);
                        if (Application.isPlaying)
                        {
                            Destroy(cellObject);
                        }
                        else
                        {
                            DestroyImmediate(cellObject);
                        }
                    }
                }
            }

            _stageData = null;
            _cellObjects = null;
            _cellRenderers = null;
            _collectedKeys = Array.Empty<bool>();
            _openedDoors = Array.Empty<bool>();
            _isFront = true;
        }

        private void RefreshVisuals()
        {
            if (_stageData == null || _cellRenderers == null)
            {
                return;
            }

            for (int y = 0; y < _stageData.Height; y++)
            {
                for (int x = 0; x < _stageData.Width; x++)
                {
                    SpriteRenderer spriteRenderer = _cellRenderers[x, y];
                    if (spriteRenderer == null)
                    {
                        continue;
                    }

                    spriteRenderer.color = GetCellColor(x, y);
                }
            }
        }

        private Color GetCellColor(int x, int y)
        {
            int cell = _stageData.GetCell(x, y);
            Color cellColor;
            if (cell == 2)
            {
                cellColor = _flipZoneColor;
            }
            else
            {
                bool isLightOnCurrentFace = _isFront ? cell == 1 : cell == 0;
                cellColor = isLightOnCurrentFace ? _frontFloorColor : _frontWallColor;
            }

            if (x == _stageData.StartX && y == _stageData.StartY)
            {
                cellColor = _isFront ? _startFrontColor : _startBackColor;
            }

            if (x == _stageData.GoalX && y == _stageData.GoalY)
            {
                cellColor = _goalColor;
            }

            if (!_isFront && TryGetKeyIndex(x, y, out int keyIndex) && !IsStateEnabled(_collectedKeys, keyIndex))
            {
                cellColor = _interactionColor;
            }

            if (_isFront && TryGetDoorIndex(x, y, out int doorIndex) && !IsStateEnabled(_openedDoors, doorIndex))
            {
                cellColor = _interactionColor;
            }

            return cellColor;
        }

        private bool TryGetKeyIndex(int x, int y, out int keyIndex)
        {
            PositionData[] keys = _stageData.Keys;
            if (keys != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    PositionData key = keys[i];
                    if (key != null && key.X == x && key.Y == y)
                    {
                        keyIndex = i;
                        return true;
                    }
                }
            }

            keyIndex = -1;
            return false;
        }

        private bool TryGetDoorIndex(int x, int y, out int doorIndex)
        {
            PositionData[] doors = _stageData.Doors;
            if (doors != null)
            {
                for (int i = 0; i < doors.Length; i++)
                {
                    PositionData door = doors[i];
                    if (door != null && door.X == x && door.Y == y)
                    {
                        doorIndex = i;
                        return true;
                    }
                }
            }

            doorIndex = -1;
            return false;
        }

        private static bool IsStateEnabled(bool[] states, int index)
        {
            return states != null && index >= 0 && index < states.Length && states[index];
        }

        private static Sprite GetCellSprite()
        {
            if (s_cellSprite != null)
            {
                return s_cellSprite;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;

            s_cellSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            s_cellSprite.hideFlags = HideFlags.HideAndDontSave;
            return s_cellSprite;
        }
    }
}
