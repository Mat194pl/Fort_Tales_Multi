using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using System.Net;

namespace Fort_Tales
{
    class CGameObject
    {
        public int X;
        public int Y;
        public int Object_ID;
        public int TexX;
        public int TexY;
        public int Height;
        public int Width;        

        public CGameObject(int x, int y, int id, int texx, int texy, int height, int width)
        {
            X = x;
            Y = y;
            Object_ID = id;
            TexX = texx;
            TexY = texy;
            Height = height;
            Width = width;
        }
    }

    struct SCamera
    {
        public int X;
        public int Y;
    }

    class CPlayer
    {
        public List<CPlayerObject> Objects = new List<CPlayerObject>();
        public List<CUnit> Units = new List<CUnit>();
        public List<CGameObject> SeenObjects = new List<CGameObject>();
        public string Name;
        public SCamera Camera;
        public Vector2 Mouse;
        public IPEndPoint IP_internal;
        public IPEndPoint IP_external;
        //public CBlock Blocks;
        public int Resources { get; set; }
        public int MaxResources { get; set; }
        public int Food { get; set; }
        public int FoodIncome { get; set; }
        public int MaxFood { get; set; }
        public int Money { get; set; } 
        public int MaxPopulation { get; set; }
        public int Population { get; set; }
        public int ResourcesIncome { get; set; }
        public bool IsMapLoaded;
        public int MapLastX { get; set; }
        public int MapLastY { get; set; }
        //private int Resources;
        //public int Gold;
        public int ID { get; set; }
        private bool playing;
        public int MapCount;
        public bool NeedMap;
        public int WindowWidth;
        public int WindowHeight;


        public CPlayer()
        {
            playing = false;
        }

        public CPlayer(string name, int id)
        {
            Name = name;
            MaxPopulation = 0;
            Population = 0;
            Resources = 100000;
            MaxResources = 100000;
            Food = 1000;
            MaxFood = 1000;
            ID = id;
            Camera.X = 0;
            Camera.Y = 0;
            playing = true;
        }

        public void SetPlayerID(int id)
        {
            ID = id;
        }

        public int GetResources(/*int city*/)
        {           
            return Resources;
        }

        public bool IsPlaying()
        {
            return playing;
        }

        public void addResources(int res)
        { 
            Resources += res;
        }

        public bool IsUnitSelected(int x, int y)
        {
            ////Console.WriteLine(x.ToString() + " " + y.ToString());
            int index = Units.FindIndex(i => i.x == x && i.y == y);
            if (index >= 0)
            {
                return true;
            }
            return false;
        }

        public bool IsUnitSelected(int id)
        {
            ////Console.WriteLine(x.ToString() + " " + y.ToString());
            int index = Units.FindIndex(i => i.ID == id);
            if (index >= 0)
            {
                return true;
            }
            return false;
        }

        public int GetObjectID(int x, int y)
        {
            int index = Objects.FindIndex(i => i.x == x && i.y == y);
            {
                if (index >= 0)
                    return index;
            }
            return -1;
        }

        public int GetUnitID(int x, int y)
        {
            int index = Units.FindIndex(i => i.x == x && i.y == y);
            {
                if (index >= 0)
                    return index;
            }
            return -1;
        }

        public void RemoveUnit(int x, int y)
        {
            int i = this.GetUnitID(x, y);
            Units.RemoveAt(this.GetUnitID(x, y));
            if (Units.Count != 0)
            {
                foreach (CUnit u in Units)
                {
                    if (u.ID >= i) u.ID--;
                }
            }
        }

        public void AddUnit(int x, int y, ref CBlock[,] Blocks, ref CRoadFinder RoadFinder)
        {
            int id;
            if (Units.Count == 0) id = 0;
            else
                id = Units.Last().ID + 1;
            /*if (Blocks[x, y].GetObjectID() == 0 && !Blocks[x, y].IsUnitOnBlocks(ref Units))*/ Units.Add(new CUnit(x, y, id, 500, ref RoadFinder, ref Blocks));
        }

        public void BarrackAddUnit(int selection, ref CBlock[,] Blocks, ref CRoadFinder RoadFinder)
        {
            if (selection >= 0)
            {
                int x = Objects[selection].x;
                int y = Objects[selection].y;
                int id;
                if (Units.Count == 0) id = 0;
                else
                    id = Units.Last().ID + 1;
                //Console.WriteLine(" Make unit" + " " + x.ToString() + " " + y.ToString() + " " + id.ToString());
                if (Blocks[x - 1, y].GetObjectID() == 0 && !Blocks[x - 1, y].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x - 1, y, id, 500, ref RoadFinder, ref Blocks));
                else
                    if (Blocks[x - 1, y - 1].GetObjectID() == 0 && !Blocks[x - 1, y - 1].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x - 1, y - 1, id, 500, ref RoadFinder, ref Blocks));
                    else
                        if (Blocks[x, y - 1].GetObjectID() == 0 && !Blocks[x, y - 1].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x, y - 1, id, 500, ref RoadFinder, ref Blocks));
                        else
                            if (Blocks[x + 1, y - 1].GetObjectID() == 0 && !Blocks[x + 1, y - 1].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x + 1, y - 1, id, 500, ref RoadFinder, ref Blocks));
                            else
                                if (Blocks[x + 2, y - 1].GetObjectID() == 0 && !Blocks[x + 2, y - 1].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x + 2, y - 1, id, 500, ref RoadFinder, ref Blocks));
                                else
                                    if (Blocks[x + 2, y].GetObjectID() == 0 && !Blocks[x + 2, y].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x + 2, y, id, 500, ref RoadFinder, ref Blocks));
                                    else
                                        if (Blocks[x + 2, y + 1].GetObjectID() == 0 && !Blocks[x + 2, y + 1].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x + 2, y + 1, id, 500, ref RoadFinder, ref Blocks));
                                        else
                                            if (Blocks[x + 2, y + 2].GetObjectID() == 0 && !Blocks[x + 2, y + 2].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x + 2, y + 2, id, 500, ref RoadFinder, ref Blocks));
                                            else
                                                if (Blocks[x + 1, y + 2].GetObjectID() == 0 && !Blocks[x + 1, y + 2].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x + 1, y + 2, id, 500, ref RoadFinder, ref Blocks));
                                                else
                                                    if (Blocks[x, y + 2].GetObjectID() == 0 && !Blocks[x, y + 2].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x, y + 2, id, 500, ref RoadFinder, ref Blocks));
                                                    else
                                                        if (Blocks[x - 1, y + 2].GetObjectID() == 0 && !Blocks[x - 1, y + 2].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x - 1, y + 2, id, 500, ref RoadFinder, ref Blocks));
                                                        else
                                                            if (Blocks[x - 1, y + 1].GetObjectID() == 0 && !Blocks[x - 1, y + 1].IsUnitOnBlocks(ref Units)) Units.Add(new CUnit(x - 1, y + 1, id, 500, ref RoadFinder, ref Blocks));
            }
        }

        public void Update()
        {
            foreach (CPlayerObject p in Objects)
            {
                if (p.objectId == 50 || p.objectId == 51 || p.objectId == 52 || p.objectId == 53)
                {
                   
                    //CCities c = Cities.Find(x => x.ID == p.city);
                    //if (c != null)
                    //{
                    Money += 5;
                        //Console.WriteLine("Add " + p.city);
                    //}
                    /*foreach (CCities s in Cities)
                    {
                        Console.WriteLine(s.ID + " " + s.Resources + p.city);
                    }*/
                }
            }
        }
    }
}
