using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Fort_Tales
{
    class CRoadFinder
    {
        Dijistra d;
        CBlock[,] Blocks;

        public CRoadFinder(ref CBlock[,] blocks)
        {
            d = new Dijistra(ref blocks);
            Blocks = blocks;
        }

        public bool FindRoad(Vector2 Start, Vector2 End, CUnit u, ref CPlayer player)
        {
            if (u != null)
            {
                int ax = -1;
                int ay = -1;
                try
                {
                    if (player.Units[u.ID].Way.Count != 0)
                    {
                        ax = (int)u.Way.First().X;
                        ay = (int)u.Way.First().Y;
                    }
                    List<Vector2> w = new List<Vector2>();
                    d.ClearMap();
                    d.ClearWay();
                    u.Obstale = 0;

                    //Console.WriteLine(Start.X.ToString() + " " + Start.Y + " " + End.X + " " + End.Y);
                    d.FindWay((int)Start.X, (int)Start.Y, (int)End.X, (int)End.Y, ref player);
                    //Console.WriteLine("Ended FindWay");
                    if (d.foundway)
                    {
                        if (d.smlPoint.X != -1 && d.smlPoint.Y != -1)
                        {
                            ////Console.WriteLine(d.smlPoint.X.ToString() + " " + d.smlPoint.Y.ToString());
                            d.CreateWay((int)Start.X, (int)Start.Y, (int)d.smlPoint.X, (int)d.smlPoint.Y, ax, ay, ref player);
                            //Console.WriteLine("Way created");
                            for (int i = 0; i < d.Way.Count; i++)
                            {
                                w.Add(new Vector2(d.Way.ElementAt(i).X, d.Way.ElementAt(i).Y));

                            }
                            u.Way = w;
                            d.smlPoint = new Point(-1, -1);
                            //Blocks[(int)u.Way.Last().X, (int)u.Way.Last().Y].Occupied = true;
                            return true;
                        }
                        else
                        {
                            d.CreateWay((int)Start.X, (int)Start.Y, (int)End.X, (int)End.Y, ax, ay, ref player);
                            for (int i = 0; i < d.Way.Count; i++)
                            {
                                w.Add(new Vector2(d.Way.ElementAt(i).X, d.Way.ElementAt(i).Y));

                            }
                            u.Way = w;
                            //Blocks[(int)u.Way.Last().X, (int)u.Way.Last().Y].Occupied = true;
                            return true;
                        }
                    }
                    Console.WriteLine("Error");
                    return false;
                }
                catch (ArgumentOutOfRangeException arg)
                {
                    Console.WriteLine("Zły indekx w tablicy");
                }
            }
            return false;
        }
    }
}
