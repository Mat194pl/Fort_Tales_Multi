using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class CPlayerObject
    {
        public int x { get; set; }
        public int y { get; set; }
        public int objectId { get; set; }
        private int Hp { get; set; }
        private int MaxHp;
        public bool building { get; set; }
        public int Id { get; set; }
        public int Loyality { get; set; }
        private CPlayer player;
        public int BuildingTime;
        public bool Builded;
        private int BHp;
        private int BMaxHp;
        public int MaxBuildingTime;
        //public int city { get; set; }

        public CPlayerObject(ref CPlayer play,int px, int py, int pobjectId, int hp, bool pbuilding, int pId, int l/*, int pcity*/)
        {
            x = px;
            y = py;
            objectId = pobjectId;
            BHp = hp;
            BMaxHp = hp;
            building = pbuilding;
            Id = pId;
            Loyality = l;
            player = play;
            Builded = false;
            BuildingTime = 600;
            MaxBuildingTime = 600;
            Hp = 50;
            MaxHp = 50;
            //city = pcity;
        }

        public int GetHp()
        {
            return Hp;
        }

        public int GetMaxHp()
        {
            return MaxHp;
        }

        public void AttackThis(int a)
        {
            Hp -= a;
        }

        public void Update()
        {
            if (!Builded)
            {
                BuildingTime--;
                if (BuildingTime <= 0)
                {
                    int a = Hp;
                    Hp = BHp - (50 - a);
                    MaxHp = BMaxHp;
                    Console.WriteLine(Hp.ToString() + " " + MaxHp.ToString());
                    Builded = true;
                }
            }
        }

        public bool IsNear(int px, int py, int d)
        {
            if (Math.Sqrt((x - px) * (x - px) + (y - py) * (y - py)) <= d)
            {
                return true;
            }
            
            return false;
        }

        public CPlayer GetPlayer()
        {
            return player;
        }

        public int GetObjectID()
        {
            return objectId;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }

        public void ChangePlayer(ref CPlayer play1, ref CPlayer play2, ref CBlock[,] Blocks)
        {
            //Console.WriteLine("Changing");
            int id;
            if (play1.Objects.Count == 0) id = 0;
            else
                id = play1.Objects.Last().Id;
            
            play1.Objects.Add(new CPlayerObject(ref player, x, y, objectId, 500, true, id + 1, 2000/*, Blocks[x, y].city*/));
            if (objectId == 100)
            {
                for (int i = -20; i <= 20; i++)
                {
                    for (int j = -20; j <= 20; j++)
                    {
                        if (x + i >= 0 && x + i < Blocks.GetLength(0) && y + j >= 0 && y + j < Blocks.GetLength(1))
                        {
                            if (Math.Sqrt((i) * (i) + (j) * (j)) < 6)
                            {
                                ////Console.WriteLine((x + i).ToString() + " " + (y + j).ToString() + " do " + x.ToString() + " " + y.ToString() + " " + Math.Sqrt((i) * (i) + (j) * (j)).ToString());
                                //if (Blocks[x + i, y + j].GetPlayerID() == 0)
                                //{
                                    Blocks[x + i, y + j].SetPlayerID(play1.ID);
                                    Blocks[x + i, y + j].buildable = true;
                                //}
                               
                            }
                        }
                    }
                }
            }
            play2.Objects.Remove(this);
        }
    }
}
