using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class CWallBuilder
    {
        CBlock[,] Blocks;
        public int[] Walls_types = { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
        

        public CWallBuilder(ref CBlock[,] block)
        {
            Blocks = block;
            //Walls_types = {20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40};
        }

        public bool IsUnitOnBlocks(int x, int y, CPlayer player)
        {
            ////Console.WriteLine(x.ToString() + " " + y.ToString());
            for (int i = 0; i < player.Units.Count; i++)
            {
                if (player.Units[i].x == x && player.Units[i].y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public void BuildWall(int x, int y, CPlayer player)
        {
            if (Blocks[x, y].GetObjectID() == 0 && !IsUnitOnBlocks(x, y, player) && Blocks[x, y].buildable/*Blocks[x, y].city != 0*/)
            {
                int id;
                if (player.Objects.Count == 0) id = 0;
                else
                id = player.Objects.Last().Id;
                player.Objects.Add(new CPlayerObject(ref player,x, y, 20, 1000, true, id + 1, 10000/*, Blocks[x, y].city*/));
                //player.Resources -= 200;
                bool a = false;
                bool b = false;
                bool c = false;
                bool d = false;
                if (x < (Blocks.GetLength(0) - 1))
                {
                    if (SearchNeighbours(x + 1, y, Walls_types))
                    {
                        c = true;
                    }
                }
                if (x > 0)
                {
                    if (SearchNeighbours(x - 1, y, Walls_types))
                    {
                        a = true;
                    }
                }
                if (y < (Blocks.GetLength(1) - 1))
                {
                    if (SearchNeighbours(x, y + 1, Walls_types))
                    {
                        d = true;
                    }
                }
                if (y > 0)
                {
                    if (SearchNeighbours(x, y - 1, Walls_types))
                    {
                        b = true;
                    }
                }
                Blocks[x, y].SetPlayerID(player.ID);
                if (!a && !b && !c && !d) Blocks[x, y].SetObjectID(20);
                if (a && !b && !c && !d) Blocks[x, y].SetObjectID(21);
                if (!a && b && !c && !d) Blocks[x, y].SetObjectID(22);
                if (!a && !b && c && !d) Blocks[x, y].SetObjectID(23);
                if (!a && !b && !c && d) Blocks[x, y].SetObjectID(24);
                if (a && b && !c && !d) Blocks[x, y].SetObjectID(25);
                if (!a && b && c && !d) Blocks[x, y].SetObjectID(26);
                if (!a && !b && c && d) Blocks[x, y].SetObjectID(27);
                if (a && !b && !c && d) Blocks[x, y].SetObjectID(28);
                if (a && !b && c && !d) Blocks[x, y].SetObjectID(29);
                if (!a && b && !c && d) Blocks[x, y].SetObjectID(30);
                if (a && b && c && !d) Blocks[x, y].SetObjectID(33);
                if (!a && b && c && d) Blocks[x, y].SetObjectID(34);
                if (a && !b && c && d) Blocks[x, y].SetObjectID(35);
                if (a && b && !c && d) Blocks[x, y].SetObjectID(36);
                if (a && b && c && d) Blocks[x, y].SetObjectID(37);

                if (a || b || c || d)
                {
                    if (a)
                    {
                        if (Blocks[x - 1, y].GetObjectID() == 20) Blocks[x - 1, y].SetObjectID(23);
                        if (Blocks[x - 1, y].GetObjectID() == 21) Blocks[x - 1, y].SetObjectID(29);
                        if (Blocks[x - 1, y].GetObjectID() == 22) Blocks[x - 1, y].SetObjectID(26);
                        //if (Blocks[x - 1, y].GetObjectID() == 23) Blocks[x - 1, y].SetObjectID(21);
                        if (Blocks[x - 1, y].GetObjectID() == 24) Blocks[x - 1, y].SetObjectID(27);
                        if (Blocks[x - 1, y].GetObjectID() == 25) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 26) Blocks[x - 1, y].SetObjectID(21);
                        //if (Blocks[x - 1, y].GetObjectID() == 27) Blocks[x - 1, y].SetObjectID(33);
                        if (Blocks[x - 1, y].GetObjectID() == 28) Blocks[x - 1, y].SetObjectID(35);
                        //if (Blocks[x - 1, y].GetObjectID() == 29) Blocks[x - 1, y].SetObjectID(35);
                        if (Blocks[x - 1, y].GetObjectID() == 30) Blocks[x - 1, y].SetObjectID(34);
                        //if (Blocks[x - 1, y].GetObjectID() == 31) Blocks[x - 1, y].SetObjectID(33);
                        if (Blocks[x - 1, y].GetObjectID() == 32) Blocks[x - 1, y].SetObjectID(34);
                        //if (Blocks[x - 1, y].GetObjectID() == 33) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 34) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 35) Blocks[x - 1, y].SetObjectID(33);
                        if (Blocks[x - 1, y].GetObjectID() == 36) Blocks[x - 1, y].SetObjectID(37);
                        //if (Blocks[x - 1, y].GetObjectID() == 37) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 38) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 39) Blocks[x - 1, y].SetObjectID(33);
                        //if (Blocks[x - 1, y].GetObjectID() == 40) Blocks[x - 1, y].SetObjectID(33);
                    }
                    if (b)
                    {
                        if (Blocks[x, y - 1].GetObjectID() == 20) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 21) Blocks[x, y - 1].SetObjectID(28);
                        if (Blocks[x, y - 1].GetObjectID() == 22) Blocks[x, y - 1].SetObjectID(30);
                        if (Blocks[x, y - 1].GetObjectID() == 23) Blocks[x, y - 1].SetObjectID(27);
                        //if (Blocks[x, y - 1].GetObjectID() == 24) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 25) Blocks[x, y - 1].SetObjectID(36);
                        if (Blocks[x, y - 1].GetObjectID() == 26) Blocks[x, y - 1].SetObjectID(34);
                        //if (Blocks[x, y - 1].GetObjectID() == 27) Blocks[x, y - 1].SetObjectID(24);
                        //if (Blocks[x, y - 1].GetObjectID() == 28) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 29) Blocks[x, y - 1].SetObjectID(35);
                        //if (Blocks[x, y - 1].GetObjectID() == 30) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 31) Blocks[x, y - 1].SetObjectID(35);
                        //if (Blocks[x, y - 1].GetObjectID() == 32) Blocks[x, y - 1].SetObjectID(24);
                        if (Blocks[x, y - 1].GetObjectID() == 33) Blocks[x, y - 1].SetObjectID(37);
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
                        if (Blocks[x + 1, y].GetObjectID() == 20) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 21) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 22) Blocks[x + 1, y].SetObjectID(25);
                        if (Blocks[x + 1, y].GetObjectID() == 23) Blocks[x + 1, y].SetObjectID(29);
                        if (Blocks[x + 1, y].GetObjectID() == 24) Blocks[x + 1, y].SetObjectID(28);
                        //if (Blocks[x + 1, y].GetObjectID() == 25) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 26) Blocks[x + 1, y].SetObjectID(33);
                        if (Blocks[x + 1, y].GetObjectID() == 27) Blocks[x + 1, y].SetObjectID(35);
                        //if (Blocks[x + 1, y].GetObjectID() == 28) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 29) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 30) Blocks[x + 1, y].SetObjectID(36);
                        //if (Blocks[x + 1, y].GetObjectID() == 31) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 32) Blocks[x + 1, y].SetObjectID(36);
                        //if (Blocks[x + 1, y].GetObjectID() == 33) Blocks[x + 1, y].SetObjectID(21);
                        if (Blocks[x + 1, y].GetObjectID() == 34) Blocks[x + 1, y].SetObjectID(37);
                        //if (Blocks[x + 1, y].GetObjectID() == 35) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 36) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 37) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 38) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 39) Blocks[x + 1, y].SetObjectID(21);
                        //if (Blocks[x + 1, y].GetObjectID() == 40) Blocks[x + 1, y].SetObjectID(21);
                    }
                    if (d)
                    {
                        if (Blocks[x, y + 1].GetObjectID() == 20) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 21) Blocks[x, y + 1].SetObjectID(25);
                        //if (Blocks[x, y + 1].GetObjectID() == 22) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 23) Blocks[x, y + 1].SetObjectID(26);
                        if (Blocks[x, y + 1].GetObjectID() == 24) Blocks[x, y + 1].SetObjectID(30);
                        //if (Blocks[x, y + 1].GetObjectID() == 25) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 26) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 27) Blocks[x, y + 1].SetObjectID(34);
                        if (Blocks[x, y + 1].GetObjectID() == 28) Blocks[x, y + 1].SetObjectID(36);
                        if (Blocks[x, y + 1].GetObjectID() == 29) Blocks[x, y + 1].SetObjectID(33);
                        //if (Blocks[x, y + 1].GetObjectID() == 30) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 31) Blocks[x, y + 1].SetObjectID(33);
                        //if (Blocks[x, y + 1].GetObjectID() == 32) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 33) Blocks[x, y + 1].SetObjectID(22);
                        //if (Blocks[x, y + 1].GetObjectID() == 34) Blocks[x, y + 1].SetObjectID(22);
                        if (Blocks[x, y + 1].GetObjectID() == 35) Blocks[x, y + 1].SetObjectID(37);
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
                        if (SearchNeighbours(x + 1, y, Walls_types))
                        {
                            c = true;
                        }
                    }
                    if (x > 0)
                    {
                        if (SearchNeighbours(x - 1, y, Walls_types))
                        {
                            a = true;
                        }
                    }
                    if (y < (Blocks.GetLength(1) - 1))
                    {
                        if (SearchNeighbours(x, y + 1, Walls_types))
                        {
                            d = true;
                        }
                    }
                    if (y > 0)
                    {
                        if (SearchNeighbours(x, y - 1, Walls_types))
                        {
                            b = true;
                        }
                    }
                    if (a || b || c || d)
                    {
                        if (a)
                        {
                            if (Blocks[x - 1, y].GetObjectID() == 23) Blocks[x - 1, y].SetObjectID(20);
                            if (Blocks[x - 1, y].GetObjectID() == 26) Blocks[x - 1, y].SetObjectID(22);
                            if (Blocks[x - 1, y].GetObjectID() == 27) Blocks[x - 1, y].SetObjectID(24);
                            if (Blocks[x - 1, y].GetObjectID() == 29) Blocks[x - 1, y].SetObjectID(21);
                            if (Blocks[x - 1, y].GetObjectID() == 31) Blocks[x - 1, y].SetObjectID(21);
                            if (Blocks[x - 1, y].GetObjectID() == 33) Blocks[x - 1, y].SetObjectID(25);
                            if (Blocks[x - 1, y].GetObjectID() == 34) Blocks[x - 1, y].SetObjectID(30);
                            if (Blocks[x - 1, y].GetObjectID() == 35) Blocks[x - 1, y].SetObjectID(28);
                            if (Blocks[x - 1, y].GetObjectID() == 37) Blocks[x - 1, y].SetObjectID(36);
                            if (Blocks[x - 1, y].GetObjectID() == 38) Blocks[x - 1, y].SetObjectID(36);
                            if (Blocks[x - 1, y].GetObjectID() == 39) Blocks[x - 1, y].SetObjectID(36);
                            if (Blocks[x - 1, y].GetObjectID() == 40) Blocks[x - 1, y].SetObjectID(36);
                        }
                        if (b)
                        {
                            if (Blocks[x, y - 1].GetObjectID() == 24) Blocks[x, y - 1].SetObjectID(20);
                            if (Blocks[x, y - 1].GetObjectID() == 27) Blocks[x, y - 1].SetObjectID(23);
                            if (Blocks[x, y - 1].GetObjectID() == 28) Blocks[x, y - 1].SetObjectID(21);
                            if (Blocks[x, y - 1].GetObjectID() == 30) Blocks[x, y - 1].SetObjectID(22);
                            if (Blocks[x, y - 1].GetObjectID() == 32) Blocks[x, y - 1].SetObjectID(22);
                            if (Blocks[x, y - 1].GetObjectID() == 34) Blocks[x, y - 1].SetObjectID(26);
                            if (Blocks[x, y - 1].GetObjectID() == 35) Blocks[x, y - 1].SetObjectID(29);
                            if (Blocks[x, y - 1].GetObjectID() == 36) Blocks[x, y - 1].SetObjectID(25);
                            if (Blocks[x, y - 1].GetObjectID() == 37) Blocks[x, y - 1].SetObjectID(33);
                            if (Blocks[x, y - 1].GetObjectID() == 38) Blocks[x, y - 1].SetObjectID(33);
                            if (Blocks[x, y - 1].GetObjectID() == 39) Blocks[x, y - 1].SetObjectID(33);
                            if (Blocks[x, y - 1].GetObjectID() == 40) Blocks[x, y - 1].SetObjectID(33);
                        }
                        if (c)
                        {
                            if (Blocks[x + 1, y].GetObjectID() == 21) Blocks[x + 1, y].SetObjectID(20);
                            if (Blocks[x + 1, y].GetObjectID() == 25) Blocks[x + 1, y].SetObjectID(22);
                            if (Blocks[x + 1, y].GetObjectID() == 28) Blocks[x + 1, y].SetObjectID(24);
                            if (Blocks[x + 1, y].GetObjectID() == 29) Blocks[x + 1, y].SetObjectID(23);
                            if (Blocks[x + 1, y].GetObjectID() == 31) Blocks[x + 1, y].SetObjectID(23);
                            if (Blocks[x + 1, y].GetObjectID() == 33) Blocks[x + 1, y].SetObjectID(26);
                            if (Blocks[x + 1, y].GetObjectID() == 35) Blocks[x + 1, y].SetObjectID(27);
                            if (Blocks[x + 1, y].GetObjectID() == 36) Blocks[x + 1, y].SetObjectID(30);
                            if (Blocks[x + 1, y].GetObjectID() == 37) Blocks[x + 1, y].SetObjectID(34);
                            if (Blocks[x + 1, y].GetObjectID() == 38) Blocks[x + 1, y].SetObjectID(34);
                            if (Blocks[x + 1, y].GetObjectID() == 39) Blocks[x + 1, y].SetObjectID(34);
                            if (Blocks[x + 1, y].GetObjectID() == 40) Blocks[x + 1, y].SetObjectID(34);
                        }
                        if (d)
                        {
                            if (Blocks[x, y + 1].GetObjectID() == 22) Blocks[x, y + 1].SetObjectID(20);
                            if (Blocks[x, y + 1].GetObjectID() == 25) Blocks[x, y + 1].SetObjectID(21);
                            if (Blocks[x, y + 1].GetObjectID() == 26) Blocks[x, y + 1].SetObjectID(23);
                            if (Blocks[x, y + 1].GetObjectID() == 30) Blocks[x, y + 1].SetObjectID(24);
                            if (Blocks[x, y + 1].GetObjectID() == 32) Blocks[x, y + 1].SetObjectID(24);
                            if (Blocks[x, y + 1].GetObjectID() == 33) Blocks[x, y + 1].SetObjectID(29);
                            if (Blocks[x, y + 1].GetObjectID() == 34) Blocks[x, y + 1].SetObjectID(27);
                            if (Blocks[x, y + 1].GetObjectID() == 36) Blocks[x, y + 1].SetObjectID(28);
                            if (Blocks[x, y + 1].GetObjectID() == 37) Blocks[x, y + 1].SetObjectID(35);
                            if (Blocks[x, y + 1].GetObjectID() == 38) Blocks[x, y + 1].SetObjectID(35);
                            if (Blocks[x, y + 1].GetObjectID() == 39) Blocks[x, y + 1].SetObjectID(35);
                            if (Blocks[x, y + 1].GetObjectID() == 40) Blocks[x, y + 1].SetObjectID(35);
                        }
                    }
                }
            }
        }
    }
}
