using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class CBlock
    {
        private int X { get; set; }
        private int Y { get; set; }
        private int building_id { get; set; }
        private int object_id { get; set; }
        private bool destroyable { get; set; }
        private int player_id { get; set; } // 0 - neutral, 1 - player one, 2 - player two
        public int[] Obstales = { 10, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 50, 51, 52, 53, 54, 60, 61, 62, 63, 100, 110, 111, 120, 130};
        public int[] Walls = { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
        public bool buildable { get; set; }
        public int loyality { get; set; }
        public int Terrain_type { get; set; }
        //public int city { get; set; }
        public bool Occupied { get; set; }

        public void Init(int x, int y)
        {
            X = x;
            Y = y;
            object_id = 0;
            destroyable = false;
            player_id = -1;
            buildable = false;
            Terrain_type = 0;
            loyality = 0;
            //city = 0;
            Occupied = false;
        }

        public void Clear()
        {
            object_id = 0;
            Occupied = false;
        }

        public bool IsObstale()
        {
            foreach (int a in Obstales)
            {
                if (object_id == a) return true;
            }
            return false;
        }

        public bool IsWall()
        {
            foreach (int a in Walls)
            {
                if (object_id == a) return true;
            }
            return false;
        }

        public int GetPlayerID()
        {
            return player_id;
        }

        public void SetPlayerID(int id)
        {
            player_id = id;
        }

        public bool IsUnitOnBlocks(ref CPlayer player)
        {
            ////Console.WriteLine(X.ToString() + " " + Y.ToString());
            for (int i = 0; i < player.Units.Count; i++)
            {
                if (player.Units[i].x == X && player.Units[i].y == Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUnitOnBlocks(ref List<CUnit> Units)
        {
            ////Console.WriteLine(X.ToString() + " " + Y.ToString());
            for (int i = 0; i < Units.Count; i++)
            {
                if (Units[i].x == X && Units[i].y == Y)
                {
                    return true;
                }
            }
            return false;
        }

        public void Test()
        {

        }

        public void Init(int x, int y, int obj_id)
        {
            X = x;
            Y = y;
            object_id = obj_id;
        }
        
        public int GetX()
        {
            return X;
        }

        public int GetY()
        {
            return Y;
        }

        public void SetObjectID(int id)
        {
            object_id = id;
        }

        public int GetBuildingID()
        {
            return building_id;
        }

        public int GetObjectID()
        {
            return object_id;
        }
    }
}
