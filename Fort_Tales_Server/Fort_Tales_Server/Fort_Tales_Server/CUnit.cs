using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Fort_Tales
{
    class CUnit
    {
        public int x;
        public int y;
        public int dx;
        public int dy;
        public int ID;
        public List<Vector2> Way;
        CRoadFinder RoadFinder;
        CBlock[,] Blocks;
        public int pX;
        public int pY;
        public int speed;
        public int rspeed;
        public int nspeed;
        public float realX;
        public float realY;
        public int Obstale;
        public bool moving;
        public int Hp;
        public int MaxHp;
        public int Type;

        public CUnit(int px, int py, int pID, int hp, ref CRoadFinder road, ref CBlock[,] blocks)
        {
            x = px;
            y = py;
            pX = 0;
            pY = 0;
            MaxHp = hp;
            Hp = MaxHp;
            realX = px * 50;
            realY = py * 50;
            ID = pID;
            nspeed = 2;
            rspeed = 4;
            speed = nspeed;
            RoadFinder = road;
            Blocks = blocks;
            Way = new List<Vector2>();
            Obstale = 0;
            moving = false;
        }

        public CUnit(int px, int py)
        {
            x = px;
            y = py;
            dx = x;
            dy = y;
        }

        public bool IsNear(int px, int py, int d)
        {
            if (Math.Sqrt((x - px) * (x - px) + (y - py) * (y - py)) <= d)
            {
                return true;
            }

            return false;
        }

        public void Move(ref CPlayer player, ref CRoadBuilder RoadBuilder)
        {

            for (int i = 1; i <= speed; i++)
            {

                if (Way.Count != 0)
                {
                    moving = true;
                    dx = (int)Way.Last().X;
                    dy = (int)Way.Last().Y;
                    
                    int nx;
                    int ny;
                    if (Way.Count > 1)
                    {
                        nx = (int)Way[1].X;
                        ny = (int)Way[1].Y;
                    }
                    else
                    {
                        nx = dx;
                        ny = dy;
                    }
                    //Console.WriteLine(nx.ToString() + " " + ny.ToString());
                    if (x != dx || y != dy)
                    {
                        if (Blocks[(int)Way.First().X, (int)Way.First().Y].IsObstale() || player.IsUnitSelected(nx, ny))
                        {

                            Blocks[dx, dy].Occupied = false;
                            if (player.IsUnitSelected(nx, ny))
                            {
                                int id = player.GetUnitID(nx, ny);
                                
                                CUnit c = player.Units.Find(k => k.ID == id);
                                if (c != null)
                                {
                                    if (c.moving == false)
                                    {
                                        Console.WriteLine("Stop");
                                        moving = false;
                                        Way.RemoveAt(Way.Count - 1);
                                    }
                                    else
                                    {
                                        

                                    }
                                }

                            }
                            else
                            {
                                Way.Clear();
                                Obstale = 1;
                                RoadFinder.FindRoad((new Vector2(x, y)), new Vector2(dx, dy), this, ref player);
                            }
                        }
                        else
                        {
                            ////Console.WriteLine((realX).ToString() + " " + (realY).ToString() + " " + Way.First().X.ToString() + " " + Way.First().Y.ToString());
                            Blocks[dx, dy].Occupied = true;
                            if (realX / 50 != (int)Way.First().X || realY / 50 != (int)Way.First().Y)
                            {
                                if (realX > (int)Way.First().X * 50) realX--;
                                if (realX < (int)Way.First().X * 50) realX++;
                                if (realY > (int)Way.First().Y * 50) realY--;
                                if (realY < (int)Way.First().Y * 50) realY++;
                                ////Console.WriteLine("Mini");
                            }
                            else
                            {
                                ////Console.WriteLine("Max");
                                ////Console.WriteLine((realX / 50).ToString() + " " + (realY / 50).ToString() + " " + Way.First().X.ToString() + " " + Way.First().Y.ToString());
                                x = (int)Way.First().X;
                                y = (int)Way.First().Y;
                                realX = x * 50;
                                realY = y * 50;
                                if (RoadBuilder.IsRoad(x, y))
                                {
                                    speed = rspeed;
                                }
                                else
                                {
                                    speed = nspeed;
                                }
                                Way.RemoveAt(0);
                            }
                        }
                    }
                }
                else
                {
                    moving = false;
                }
            }
        }
    }
}