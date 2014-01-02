using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class CSelection
    {
        public int X;
        public int Y;
        public int Type;
        public int ID;
        public int Action;
        public int menu;
        public int SelectedBuilding;
        public int dX;
        public int dY;
        public List<int> units;

        public CSelection()
        {
            X = -1;
            Y = -1;
            dX = -1;
            dY = -1;
            Type = 0;
            ID = 0;
            Action = 0;
            menu = 0;
            units = new List<int>();
        }

        public void Clear()
        {
            X = -1;
            Y = -1;
            dX = -1;
            dY = -1;
            Type = 0;
            ID = 0;
            Action = 0;
            menu = 0;
        }

        public void ClearPositions()
        {
            X = -1;
            Y = -1;
            dX = -1;
            dY = -1;
        }

        public void SetPositionsD(int x, int y)
        {
            dX = x;
            dY = y;
        }

        public void SetPositions(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void AddUnitToList(int x, int y, ref CPlayer player)
        {
            units.Clear();
            //Console.WriteLine("Add 1 unit");
            if (player.IsUnitSelected(x, y))
            {
                int m = player.GetUnitID(x, y);
                units.Add(m);
            }
        }

        public void AddUnitsToList(int xa, int ya, int xb, int yb, ref CBlock[,] blocks, ref CPlayer player)
        {
            units.Clear();
            if (xa > xb)
            {
                int a = xa;
                xa = xb;
                xb = a;
            }

            if (ya > yb)
            {
                int a = ya;
                ya = yb;
                yb = a;
            }
            for (int i = xa; i <= xb; i++)
            {
                for (int j = ya; j <= yb; j++)
                {
                    if (player.IsUnitSelected(i, j))
                    {
                        int m = player.GetUnitID(i, j);
                        units.Add(m);
                    }
                }
            }
            //Console.WriteLine(units.Count.ToString());
        }
    }
}
