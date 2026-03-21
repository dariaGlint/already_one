using System;
using UnityEngine;

namespace AlreadyOne.Data
{
    [Serializable]
    public sealed class StageData
    {
        // Field names match JSON keys for JsonUtility deserialization.
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private int[] cells;
        [SerializeField] private int startX;
        [SerializeField] private int startY;
        [SerializeField] private int goalX;
        [SerializeField] private int goalY;
        [SerializeField] private PositionData[] keys;
        [SerializeField] private PositionData[] doors;

        public int Width => width;
        public int Height => height;
        public int[] Cells => cells;
        public int StartX => startX;
        public int StartY => startY;
        public int GoalX => goalX;
        public int GoalY => goalY;
        public PositionData[] Keys => keys;
        public PositionData[] Doors => doors;

        public bool IsWalkable(int x, int y, bool isFront)
        {
            int cell = GetCell(x, y);
            if (cell < 0)
            {
                return false;
            }

            // 0 = black (back only), 1 = white (front only), 2 = both faces walkable (flip zone)
            if (cell == 2)
            {
                return true;
            }

            return isFront ? cell == 1 : cell == 0;
        }

        public int GetCell(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height || cells == null)
            {
                return -1;
            }

            int index = (y * width) + x;
            if (index < 0 || index >= cells.Length)
            {
                return -1;
            }

            return cells[index];
        }
    }

    [Serializable]
    public sealed class PositionData
    {
        // Field names match JSON keys for JsonUtility deserialization.
        [SerializeField] private int x;
        [SerializeField] private int y;

        public int X => x;
        public int Y => y;
    }
}
