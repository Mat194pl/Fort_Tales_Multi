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
            if (Blocks[x, y].GetObjectID() == 0 /*&& !Blocks[x, y].IsUnitOnBlocks(ref player)*/ && (Blocks[x, y].GetPlayerID() == 0 || Blocks[x, y].GetPlayerID() == player.ID))
            {
                Console.WriteLine("First step");
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
                                if (Blocks[x + i, y + j].GetPlayerID() == 0)
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
