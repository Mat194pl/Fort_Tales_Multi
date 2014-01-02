using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class CRoadBuilder
    {
        CBlock[,] Blocks;
        public int[] Road_types = { 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };

        public CRoadBuilder(ref CBlock[,] block)
        {
            Blocks = block;
            //Walls_types = {20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40};
        }

        public bool IsUnitOnBlocks(int x, int y, CPlayer player)
        {
            //Console.WriteLine(x.ToString() + " " + y.ToString());
            for (int i = 0; i < player.Units.Count; i++)
            {
                if (player.Units[i].x == x && player.Units[i].y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public void BuildRoad(int x, int y, CPlayer player)
        {
            if (Blocks[x, y].GetObjectID() == 0 && !IsUnitOnBlocks(x, y, player) && Blocks[x, y].buildable == true)
            {
                int id;
                if (player.Objects.Count == 0) id = 0;
                else
                id = player.Objects.Last().Id;
                player.Objects.Add(new CPlayerObject(ref player, x, y, 70, 1000, true, id + 1, 10000));
                //player.Resources -= 100;
                bool a = false;
                bool b = false;
                bool c = false;
                bool d = false;
                Blocks[x, y].SetPlayerID(player.ID);
                if (x < (Blocks.GetLength(0) - 1))
                {
                    if (SearchNeighbours(x + 1, y, Road_types))
                    {
                        c = true;
                    }
                }
                if (x > 0)
                {
                    if (SearchNeighbours(x - 1, y, Road_types))
                    {
                        a = true;
                    }
                }
                if (y < (Blocks.GetLength(1) - 1))
                {
                    if (SearchNeighbours(x, y + 1, Road_types))
                    {
                        d = true;
                    }
                }
                if (y > 0)
                {
                    if (SearchNeighbours(x, y - 1, Road_types))
                    {
                        b = true;
                    }
                }

                if (!a && !b && !c && !d) Blocks[x, y].SetObjectID(70);
                if (a && !b && !c && !d) Blocks[x, y].SetObjectID(71);
                if (!a && b && !c && !d) Blocks[x, y].SetObjectID(72);
                if (!a && !b && c && !d) Blocks[x, y].SetObjectID(73);
                if (!a && !b && !c && d) Blocks[x, y].SetObjectID(74);
                if (a && b && !c && !d) Blocks[x, y].SetObjectID(75);
                if (!a && b && c && !d) Blocks[x, y].SetObjectID(76);
                if (!a && !b && c && d) Blocks[x, y].SetObjectID(77);
                if (a && !b && !c && d) Blocks[x, y].SetObjectID(78);
                if (a && !b && c && !d) Blocks[x, y].SetObjectID(79);
                if (!a && b && !c && d) Blocks[x, y].SetObjectID(80);
                if (a && b && c && !d) Blocks[x, y].SetObjectID(83);
                if (!a && b && c && d) Blocks[x, y].SetObjectID(84);
                if (a && !b && c && d) Blocks[x, y].SetObjectID(85);
                if (a && b && !c && d) Blocks[x, y].SetObjectID(86);
                if (a && b && c && d) Blocks[x, y].SetObjectID(87);

                if (a || b || c || d)
                {
                    if (a)
                    {
                        if (Blocks[x - 1, y].GetObjectID() == 70) Blocks[x - 1, y].SetObjectID(73);
                        if (Blocks[x - 1, y].GetObjectID() == 71) Blocks[x - 1, y].SetObjectID(79);
                        if (Blocks[x - 1, y].GetObjectID() == 72) Blocks[x - 1, y].SetObjectID(76);
                        //if (Blocks[x - 1, y].GetObjectID() == 23) Blocks[x - 1, y].SetObjectID(21);
                        if (Blocks[x - 1, y].GetObjectID() == 74) Blocks[x - 1, y].SetObjectID(77);
                        if (Blocks[x - 1, y].GetObjectID() == 75) Blocks[x - 1, y].SetObjectID(83);
                        //if (Blocks[x - 1, y].GetObjectID() == 26) Blocks[x - 1, y].SetObjectID(21);
                        //if (Blocks[x - 1, y].GetObjectID() == 27) Blocks[x - 1, y].SetObjectID(33);
                        if (Blocks[x - 1, y].GetObjectID() == 78) Blocks[x - 1, y].SetObjectID(85);
                        //if (Blocks[x - 1, y].GetObjectID() == 29) Blocks[x - 1, y].SetObjectID(35);
                        if (Blocks[x - 1, y].GetObjectID() == 80) Blocks[x - 1, y].SetObjectID(84);
                        //if (Blocks[x - 1, y].GetObjectID() == 31) Blocks[x - 1, y].SetObjectID(33);
                        if (Blocks[x - 1, y].GetObjectID() == 82) Blocks[x - 1, y].SetObjectID(84);
                        //if (Blocks[x - 1, y].GetObjectID() == 33) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 34) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 35) Blocks[x - 1, y].SetObjectID(33);
                        if (Blocks[x - 1, y].GetObjectID() == 86) Blocks[x - 1, y].SetObjectID(87);
                        //if (Blocks[x - 1, y].GetObjectID() == 37) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 38) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 39) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 40) Blocks[x - 1, y].SetObjectID(33);
                    }
                    if (b)
                    {
                        if (Blocks[x, y - 1].GetObjectID() == 70) Blocks[x, y - 1].SetObjectID(74);
                        if (Blocks[x, y - 1].GetObjectID() == 71) Blocks[x, y - 1].SetObjectID(78);
                        if (Blocks[x, y - 1].GetObjectID() == 72) Blocks[x, y - 1].SetObjectID(80);
                        if (Blocks[x, y - 1].GetObjectID() == 73) Blocks[x, y - 1].SetObjectID(77);
                        //if (Blocks[x, y - 1].GetObjectID() == 24) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 75) Blocks[x, y - 1].SetObjectID(86);
                        if (Blocks[x, y - 1].GetObjectID() == 76) Blocks[x, y - 1].SetObjectID(84);
                        //if (Blocks[x, y - 1].GetObjectID() == 27) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 28) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 79) Blocks[x, y - 1].SetObjectID(85);
                        //if (Blocks[x, y - 1].GetObjectID() == 30) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 81) Blocks[x, y - 1].SetObjectID(85);
                        //if (Blocks[x, y - 1].GetObjectID() == 32) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 83) Blocks[x, y - 1].SetObjectID(87);
                        //if (Blocks[x, y - 1].GetObjectID() == 34) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 35) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 36) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 37) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 38) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 39) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 40) Blocks[x, y - 1].SetObjectID(24);
                    }
                    if (c)
                    {
                        if (Blocks[x + 1, y].GetObjectID() == 70) Blocks[x + 1, y].SetObjectID(71);
                        //if (Blocks[x + 1, y].GetObjectID() == 21) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 72) Blocks[x + 1, y].SetObjectID(75);
                        if (Blocks[x + 1, y].GetObjectID() == 73) Blocks[x + 1, y].SetObjectID(79);
                        if (Blocks[x + 1, y].GetObjectID() == 74) Blocks[x + 1, y].SetObjectID(78);
                        //if (Blocks[x + 1, y].GetObjectID() == 25) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 76) Blocks[x + 1, y].SetObjectID(83);
                        if (Blocks[x + 1, y].GetObjectID() == 77) Blocks[x + 1, y].SetObjectID(85);
                        //if (Blocks[x + 1, y].GetObjectID() == 28) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 29) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 80) Blocks[x + 1, y].SetObjectID(86);
                        //if (Blocks[x + 1, y].GetObjectID() == 31) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 82) Blocks[x + 1, y].SetObjectID(86);
                        //if (Blocks[x + 1, y].GetObjectID() == 33) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 84) Blocks[x + 1, y].SetObjectID(87);
                        //if (Blocks[x + 1, y].GetObjectID() == 35) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 36) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 37) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 38) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 39) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 40) Blocks[x + 1, y].SetObjectID(21);
                    }
                    if (d)
                    {
                        if (Blocks[x, y + 1].GetObjectID() == 70) Blocks[x, y + 1].SetObjectID(72);
                        if (Blocks[x, y + 1].GetObjectID() == 71) Blocks[x, y + 1].SetObjectID(75);
                        //if (Blocks[x, y + 1].GetObjectID() == 22) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 73) Blocks[x, y + 1].SetObjectID(76);
                        if (Blocks[x, y + 1].GetObjectID() == 74) Blocks[x, y + 1].SetObjectID(80);
                        //if (Blocks[x, y + 1].GetObjectID() == 25) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 26) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 77) Blocks[x, y + 1].SetObjectID(84);
                        if (Blocks[x, y + 1].GetObjectID() == 78) Blocks[x, y + 1].SetObjectID(86);
                        if (Blocks[x, y + 1].GetObjectID() == 79) Blocks[x, y + 1].SetObjectID(83);
                        //if (Blocks[x, y + 1].GetObjectID() == 30) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 81) Blocks[x, y + 1].SetObjectID(83);
                        //if (Blocks[x, y + 1].GetObjectID() == 32) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 33) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 34) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 85) Blocks[x, y + 1].SetObjectID(87);
                        //if (Blocks[x, y + 1].GetObjectID() == 36) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 37) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 38) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 39) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 40) Blocks[x, y + 1].SetObjectID(22);
                    }
                }
            }
        }

     

        private bool SearchNeighbours(int x, int y, params int[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Blocks[x, y].GetObjectID() == a[i]) return true;
            }
            return false;
        }

        public bool IsRoad(int x, int y)
        {
            for (int i = 0; i < Road_types.Length; i++)
            {
                ////Console.WriteLine(x.ToString() + " " + y.ToString());
                if (Blocks[x, y].GetObjectID() == Road_types[i]) return true;
            }
            return false;
        }


        public void RemoveWall(int x, int y)
        {
            bool e = false;
            for (int i = 20; i < 90; i++)
            {
                if (Blocks[x, y].GetObjectID() == i)
                {
                    e = true;
                }
            }
            if (Blocks[x, y].GetObjectID() == 60)
            {
                Blocks[x, y].SetObjectID(0);
                Blocks[x + 1, y].SetObjectID(0);
                Blocks[x + 1, y + 1].SetObjectID(0);
                Blocks[x, y + 1].SetObjectID(0);
            }
            if (e)
            {
                Blocks[x, y].SetObjectID(0);
                if (Blocks[x, y].GetObjectID() == 0)
                {
                    bool a = false;
                    bool b = false;
                    bool c = false;
                    bool d = false;
                    if (x < (Blocks.GetLength(0) - 1))
                    {
                        if (SearchNeighbours(x + 1, y, Road_types))
                        {
                            c = true;
                        }
                    }
                    if (x > 0)
                    {
                        if (SearchNeighbours(x - 1, y, Road_types))
                        {
                            a = true;
                        }
                    }
                    if (y < (Blocks.GetLength(1) - 1))
                    {
                        if (SearchNeighbours(x, y + 1, Road_types))
                        {
                            d = true;
                        }
                    }
                    if (y > 0)
                    {
                        if (SearchNeighbours(x, y - 1, Road_types))
                        {
                            b = true;
                        }
                    }
                    if (a || b || c || d)
                    {
                        if (a)
                        {
                            if (Blocks[x - 1, y].GetObjectID() == 73) Blocks[x - 1, y].SetObjectID(70);
                            if (Blocks[x - 1, y].GetObjectID() == 76) Blocks[x - 1, y].SetObjectID(72);
                            if (Blocks[x - 1, y].GetObjectID() == 77) Blocks[x - 1, y].SetObjectID(74);
                            if (Blocks[x - 1, y].GetObjectID() == 79) Blocks[x - 1, y].SetObjectID(71);
                            if (Blocks[x - 1, y].GetObjectID() == 81) Blocks[x - 1, y].SetObjectID(71);
                            if (Blocks[x - 1, y].GetObjectID() == 83) Blocks[x - 1, y].SetObjectID(75);
                            if (Blocks[x - 1, y].GetObjectID() == 84) Blocks[x - 1, y].SetObjectID(80);
                            if (Blocks[x - 1, y].GetObjectID() == 85) Blocks[x - 1, y].SetObjectID(78);
                            if (Blocks[x - 1, y].GetObjectID() == 87) Blocks[x - 1, y].SetObjectID(86);
                            if (Blocks[x - 1, y].GetObjectID() == 88) Blocks[x - 1, y].SetObjectID(86);
                            if (Blocks[x - 1, y].GetObjectID() == 89) Blocks[x - 1, y].SetObjectID(86);
                            if (Blocks[x - 1, y].GetObjectID() == 90) Blocks[x - 1, y].SetObjectID(86);
                        }
                        if (b)
                        {
                            if (Blocks[x, y - 1].GetObjectID() == 74) Blocks[x, y - 1].SetObjectID(70);
                            if (Blocks[x, y - 1].GetObjectID() == 77) Blocks[x, y - 1].SetObjectID(73);
                            if (Blocks[x, y - 1].GetObjectID() == 78) Blocks[x, y - 1].SetObjectID(71);
                            if (Blocks[x, y - 1].GetObjectID() == 80) Blocks[x, y - 1].SetObjectID(72);
                            if (Blocks[x, y - 1].GetObjectID() == 82) Blocks[x, y - 1].SetObjectID(72);
                            if (Blocks[x, y - 1].GetObjectID() == 84) Blocks[x, y - 1].SetObjectID(76);
                            if (Blocks[x, y - 1].GetObjectID() == 85) Blocks[x, y - 1].SetObjectID(79);
                            if (Blocks[x, y - 1].GetObjectID() == 86) Blocks[x, y - 1].SetObjectID(75);
                            if (Blocks[x, y - 1].GetObjectID() == 87) Blocks[x, y - 1].SetObjectID(83);
                            if (Blocks[x, y - 1].GetObjectID() == 88) Blocks[x, y - 1].SetObjectID(83);
                            if (Blocks[x, y - 1].GetObjectID() == 89) Blocks[x, y - 1].SetObjectID(83);
                            if (Blocks[x, y - 1].GetObjectID() == 90) Blocks[x, y - 1].SetObjectID(83);
                        }
                        if (c)
                        {
                            if (Blocks[x + 1, y].GetObjectID() == 71) Blocks[x + 1, y].SetObjectID(70);
                            if (Blocks[x + 1, y].GetObjectID() == 75) Blocks[x + 1, y].SetObjectID(72);
                            if (Blocks[x + 1, y].GetObjectID() == 78) Blocks[x + 1, y].SetObjectID(74);
                            if (Blocks[x + 1, y].GetObjectID() == 79) Blocks[x + 1, y].SetObjectID(73);
                            if (Blocks[x + 1, y].GetObjectID() == 81) Blocks[x + 1, y].SetObjectID(73);
                            if (Blocks[x + 1, y].GetObjectID() == 83) Blocks[x + 1, y].SetObjectID(76);
                            if (Blocks[x + 1, y].GetObjectID() == 85) Blocks[x + 1, y].SetObjectID(77);
                            if (Blocks[x + 1, y].GetObjectID() == 86) Blocks[x + 1, y].SetObjectID(80);
                            if (Blocks[x + 1, y].GetObjectID() == 87) Blocks[x + 1, y].SetObjectID(84);
                            if (Blocks[x + 1, y].GetObjectID() == 88) Blocks[x + 1, y].SetObjectID(84);
                            if (Blocks[x + 1, y].GetObjectID() == 89) Blocks[x + 1, y].SetObjectID(84);
                            if (Blocks[x + 1, y].GetObjectID() == 90) Blocks[x + 1, y].SetObjectID(84);
                        }
                        if (d)
                        {
                            if (Blocks[x, y + 1].GetObjectID() == 72) Blocks[x, y + 1].SetObjectID(70);
                            if (Blocks[x, y + 1].GetObjectID() == 75) Blocks[x, y + 1].SetObjectID(71);
                            if (Blocks[x, y + 1].GetObjectID() == 76) Blocks[x, y + 1].SetObjectID(73);
                            if (Blocks[x, y + 1].GetObjectID() == 80) Blocks[x, y + 1].SetObjectID(74);
                            if (Blocks[x, y + 1].GetObjectID() == 82) Blocks[x, y + 1].SetObjectID(74);
                            if (Blocks[x, y + 1].GetObjectID() == 83) Blocks[x, y + 1].SetObjectID(79);
                            if (Blocks[x, y + 1].GetObjectID() == 84) Blocks[x, y + 1].SetObjectID(77);
                            if (Blocks[x, y + 1].GetObjectID() == 86) Blocks[x, y + 1].SetObjectID(78);
                            if (Blocks[x, y + 1].GetObjectID() == 87) Blocks[x, y + 1].SetObjectID(85);
                            if (Blocks[x, y + 1].GetObjectID() == 88) Blocks[x, y + 1].SetObjectID(85);
                            if (Blocks[x, y + 1].GetObjectID() == 89) Blocks[x, y + 1].SetObjectID(85);
                            if (Blocks[x, y + 1].GetObjectID() == 90) Blocks[x, y + 1].SetObjectID(85);
                        }
                    }
                }
            }
        }   
    }
}
