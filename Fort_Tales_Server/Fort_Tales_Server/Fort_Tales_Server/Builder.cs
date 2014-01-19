using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class CBuilder
    {
        CBlock[,] Blocks;

        public CBuilder(ref CBlock[,] blocks)
        {
            Blocks = blocks;
        }

        public void BuildFarm(int x, int y, ref CPlayer player)
        {
            if (Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref player) && /*Blocks[x, y].city != 0*/ Blocks[x, y].buildable && Blocks[x, y].GetPlayerID() == player.ID)
            {
                if (player.Resources >= 50)
                {
                    Random f = new System.Random(DateTime.Now.Millisecond);
                    int l = f.Next(120, 124);
                    Blocks[x, y].SetObjectID(l);
                    Blocks[x, y].SetPlayerID(player.ID);
                    int id;
                    if (player.Objects.Count == 0) id = 0;
                    else
                        id = player.Objects.Last().Id;
                    player.Objects.Add(new CPlayerObject(ref player, x, y, 120, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
                    player.Resources -= 50;
                }
            }
        }

        public void BuildMine(int x, int y, ref CPlayer player)
        {
            if (Blocks[x, y].Terrain_type == 1 && Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref player) && /*Blocks[x, y].city != 0*/ Blocks[x, y].buildable && Blocks[x, y].GetPlayerID() == player.ID)
            {
                if (player.Resources >= 50)
                {
                    Blocks[x, y].SetObjectID(130);
                    Blocks[x, y].SetPlayerID(player.ID);
                    int id;
                    if (player.Objects.Count == 0) id = 0;
                    else
                        id = player.Objects.Last().Id;
                    player.Objects.Add(new CPlayerObject(ref player, x, y, 130, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
                    player.Resources -= 50;
                }
            }
        }

        public void BuildGate(int x, int y, ref CPlayer player)
        {
            if (Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref player) && /*Blocks[x, y].city != 0*/ Blocks[x, y].buildable && Blocks[x, y].GetPlayerID() == player.ID)
            {
                if (x > 0 && x < Blocks.GetLength(0) - 1 && y > 0 && y < Blocks.GetLength(1) - 1)
                {
                    if ((Blocks[x - 1, y].IsWall() && Blocks[x + 1, y].IsWall() || Blocks[x, y - 1].IsWall() && Blocks[x, y + 1].IsWall()))
                    {
                        if (player.Resources >= 50)
                        {
                            if (Blocks[x - 1, y].IsWall() && Blocks[x + 1, y].IsWall())
                            {
                                Blocks[x, y].SetObjectID(111);
                                Blocks[x, y].SetPlayerID(player.ID);
                                int id;
                                if (player.Objects.Count == 0) id = 0;
                                else
                                    id = player.Objects.Last().Id;
                                player.Objects.Add(new CPlayerObject(ref player, x, y, 110, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
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
                            if (Blocks[x, y - 1].IsWall() && Blocks[x, y + 1].IsWall())
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
                                
                                Blocks[x, y].SetObjectID(110);
                                Blocks[x, y].SetPlayerID(player.ID);
                                int id;
                                if (player.Objects.Count == 0) id = 0;
                                else
                                    id = player.Objects.Last().Id;
                                player.Objects.Add(new CPlayerObject(ref player, x, y, 110, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
                            }
                        }
                    }
                }
            }
        }

        public void BuildHouse(int x, int y, ref CPlayer player)
        {

            if (Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref player) && /*Blocks[x, y].city != 0*/ Blocks[x, y].buildable && Blocks[x, y].GetPlayerID() == player.ID)
            {
                //CCities c = player.Cities.Find(k => k.ID == Blocks[x, y].city);
                //int index = player.Cities.FindIndex(k => k.ID == Blocks[x, y].city);
                //if (c != null)
                //{
                    //if (c.Resources >= 50)
                    if (player.Resources >= 50)
                    {
                        Random f = new System.Random(DateTime.Now.Millisecond);
                        int l = f.Next(50, 54);
                        Blocks[x, y].SetObjectID(l);
                        Blocks[x, y].SetPlayerID(player.ID);
                        player.MaxPopulation += 5;
                        int id;
                        if (player.Objects.Count == 0) id = 0;
                        else
                            id = player.Objects.Last().Id;

                        player.Objects.Add(new CPlayerObject(ref player, x, y, l, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
                        //Console.WriteLine("c.Resources -= -50; " + Blocks[x, y].city.ToString());
                        //c.Resources -= 50;
                        player.Resources -= 50;
                       // Console.WriteLine(c.Resources.ToString());
                        //c = player.Cities.Find(k => k.ID == Blocks[x, y].city);
                        //Console.WriteLine(c.Resources.ToString());
                    }
                /*}
                else
                {
                    Console.WriteLine(Blocks[x, y].city.ToString());
                    foreach (CCities s in player.Cities)
                    {
                        Console.WriteLine(s.ID + " " + s.Resources.ToString());
                    }
                }*/
            }
        }

        public void BuildBarracks(int x, int y, ref CPlayer player)
        {
            if (Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref player) && Blocks[x + 1, y].GetObjectID() == 0 && !Blocks[x + 1, y].IsUnitOnBlocks(ref player) && Blocks[x, y + 1].GetObjectID() == 0 && !Blocks[x, y + 1].IsUnitOnBlocks(ref player) && Blocks[x + 1, y + 1].GetObjectID() == 0 && !Blocks[x + 1, y + 1].IsUnitOnBlocks(ref player) && Blocks[x, y].GetPlayerID() == player.ID/* && Blocks[x, y].city != 0*/)
            {
                //CCities c = player.Cities.Find(k => k.ID == Blocks[x, y].city);


                //if (c.Resources >= 500)
                if (player.Resources >= 500)
                {
                    Blocks[x, y].SetObjectID(60);
                    Blocks[x + 1, y].SetObjectID(61);
                    Blocks[x, y + 1].SetObjectID(62);
                    Blocks[x + 1, y + 1].SetObjectID(63);
                    Blocks[x, y].SetPlayerID(player.ID);
                    int id;
                    if (player.Objects.Count == 0) id = 0;
                    else
                    id = player.Objects.Last().Id;
                    player.Objects.Add(new CPlayerObject(ref player, x, y, 60, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
                    //c.Resources -= 500;
                    player.Resources -= 500;
                }
            }
        }

        /*public void BuildMagazine(int x, int y, ref CPlayer player)
        {
            if (Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref player) && Blocks[x + 1, y].GetObjectID() == 0 && !Blocks[x + 1, y].IsUnitOnBlocks(ref player) && Blocks[x, y + 1].GetObjectID() == 0 && !Blocks[x, y + 1].IsUnitOnBlocks(ref player) && Blocks[x + 1, y + 1].GetObjectID() == 0 && !Blocks[x + 1, y + 1].IsUnitOnBlocks(ref player) && (Blocks[x, y].buildable == true /*|| Blocks[x,y].city != 0//) )
            //{
                //if (player.Cities.Count != 0)
                //{
                    //CCities c = player.Cities.Find(p => p.ID == Blocks[x, y].city);
                    //if (c != null)
                    //{
                       /* Blocks[x, y].SetObjectID(110);
                        int id;
                        if (player.Objects.Count == 0) id = 0;
                        else
                            id = player.Objects.Last().Id + 1;
                        player.Objects.Add(new CPlayerObject(x, y, 110, 500, true, id/*, Blocks[x, y].city*///));
                        /*player.Magazines.Add(new CMagazine(1000, id, ref player, ref Blocks));
                        CMagazine M = player.Magazines.Find(i => i.ID == id);
                        //Console.WriteLine(M.ID + " Searching magazines");
                        int city = c.ID;
                        M.MakeCity(city);
                        c.Resources -= 300;
                        Console.WriteLine(c.Resources.ToString());

                    }
                    else
                    {
                        int city = player.Cities.Last().ID + 1;
                        Blocks[x, y].SetObjectID(110);
                        int id;
                        if (player.Objects.Count == 0) id = 0;
                        else
                            id = player.Objects.Last().Id + 1;
                        player.Objects.Add(new CPlayerObject(x, y, 110, 500, true, id/*, city*///));
                        /*player.Magazines.Add(new CMagazine(1000, id, ref player, ref Blocks));
                        CMagazine M = player.Magazines.Find(i => i.ID == id);
                        player.Cities.Add(new CCities(city, 500));
                        M.MakeCity(city);
                    }
                }
                else
                {
                    player.Cities.Add(new CCities(1, 1000));
                    Blocks[x, y].SetObjectID(110);
                    int id;
                    if (player.Objects.Count == 0)
                    {
                        id = 0;
                    }
                    else
                    {
                        id = player.Objects.Last().Id + 1;
                    }
                    
                    player.Objects.Add(new CPlayerObject(x, y, 110, 500, true, id/*, Blocks[x, y].city*///));
                    /*player.Magazines.Add(new CMagazine(1000, id, ref player, ref Blocks));
                    CMagazine M = player.Magazines.Find(i => i.ID == id);
                    M.MakeCity(1);
                }
            }
        }*/
        public bool BuildWatchtower(int x, int y, ref CPlayer player)
        {
            Console.WriteLine("Let build watchtower");
            if (Blocks[x, y].GetObjectID() == 0 /*&& !Blocks[x, y].IsUnitOnBlocks(ref player)*/ && (Blocks[x, y].GetPlayerID() == -1 || Blocks[x, y].GetPlayerID() == player.ID))
            {
                //Console.WriteLine("First step");
                Blocks[x, y].SetObjectID(100);
                int id;
                if (player.Objects.Count == 0) id = 0;
                else
                    id = player.Objects.Last().Id;
                player.Objects.Add(new CPlayerObject(ref player, x, y, 100, 500, true, id + 1, 10000/*, Blocks[x, y].city*/));
                Blocks[x, y].SetPlayerID(player.ID);
                for (int i = -20; i <= 20; i++)
                {
                    for (int j = -20; j <= 20; j++)
                    {
                        if (x + i >= 0 && x + i < Blocks.GetLength(0) && y + j >= 0 && y + j < Blocks.GetLength(1))
                        {
                            if (Math.Sqrt((i) * (i) + (j) * (j)) < 6)
                            {
                                ////Console.WriteLine((x + i).ToString() + " " + (y + j).ToString() + " do " + x.ToString() + " " + y.ToString() + " " + Math.Sqrt((i) * (i) + (j) * (j)).ToString());
                                if (Blocks[x + i, y + j].GetPlayerID() == -1)
                                {
                                    Blocks[x + i, y + j].SetPlayerID(player.ID);
                                    Blocks[x + i, y + j].buildable = true;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            Console.WriteLine("Error");
            return false;
        }

        public void Remove(ref CPlayerObject p, ref CPlayer player)
        {
            int x = p.GetX();
            int y = p.GetY();
            
            Blocks[x, y].Clear();
            player.Objects.Remove(p);

        }
    }
}
