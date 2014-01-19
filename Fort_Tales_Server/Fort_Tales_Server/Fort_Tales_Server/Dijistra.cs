using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales
{
    class Tile
    {
        public bool visited;
        public int cost;

        public Tile()
        {
            visited = false;
            cost = -1;
        }

    }

    class Point
    {
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Dijistra
    {
        public Tile[,] Map;
        public int[] Walls_types = {10, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 50, 51, 52, 53, 54 };
        public List<Point> Way = new List<Point>();
        public CBlock[,] Blocks;
        public bool foundway;
        public Point smlPoint = new Point(-1, -1); 
        public Dijistra(ref CBlock[,] blocks)        
        {
            Blocks = blocks;
            Map = new Tile[Blocks.GetLength(0), Blocks.GetLength(1)];
            for (int i = 0; i < Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < Blocks.GetLength(1); j++)
                {
                    Map[i, j] = new Tile();
                }
            }
        }
        public void ClearMap()
        {
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    Map[i, j].cost = -1;
                    Map[i, j].visited = false;
                    if (IsObstale(i, j, Walls_types))
                    {
                        Map[i, j].cost = -2;
                    }
                }
            }
        }

        private bool IsObstale(int x, int y, params int[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Blocks[x, y].GetObjectID() == a[i]) return true;
            }
            return false;
        }

        private bool IsRoad(int x, int y, params int[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Blocks[x, y].GetObjectID() == a[i]) return true;
            }
            return false;
        }

        public void ClearWay()
        {
            Way.Clear();
        }
        private Point[] dirs =
        {
            new Point(0, -1), // górny
            new Point(1, 0), // prawy
            new Point(0, 1), // dolny
            new Point(-1, 0), // lewy

            /*new Point(-1, -1), // lewy górny
            new Point(1, -1), // prawy górny
            new Point(1, 1), // prawy dolny
            new Point(-1, 1)*/ // lewy dolny
        };
        private bool IsOnMap(int ctx, int cty, int width, int height)
        {
            return (ctx >= 0 && ctx < width && cty >= 0 && cty < height);
        }
        private int dirsCount = 4;
        public int DirsCount
        {
            get
            {
                return dirsCount;
            }
            set
            {
                if (value >= 1 && value <= 8) // od 1 do 8 kierunków
                    dirsCount = value;
            }
        }
        public bool FindWay(int sx, int sy, int ex, int ey, ref CPlayer player)
        {
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    Map[i, j] = new Tile();
                }
            }
            Point v, w;
            Queue<Point> Q = new Queue<Point>();
            Q.Enqueue(new Point(sx, sy));
            Map[sx, sy].cost = 0;
            Map[sx, sy].visited = true;
            foundway = false;
            while (Q.Count > 0 && !foundway)
            {
                v = Q.Dequeue();
                for (int d = 0; d < dirsCount; d++)
                {
                    w = new Point(v.X + dirs[d].X, v.Y + dirs[d].Y);

                    if (IsOnMap(w.X, w.Y, Map.GetLength(0), Map.GetLength(1)) && !Map[w.X, w.Y].visited && !Blocks[w.X, w.Y].IsObstale() && !Blocks[w.X, w.Y].IsUnitOnBlocks(ref player) && !Blocks[w.X, w.Y].Occupied)
                    {
                        if (d >= 0 && d <= 3) // koszt po prostej
                            if (IsRoad(w.X, w.Y, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90))
                            {
                                Map[w.X, w.Y].cost = Map[v.X, v.Y].cost + 1;
                            }
                            else
                                Map[w.X, w.Y].cost = Map[v.X, v.Y].cost + 3;
                        else // koszt po skosie
                            Map[w.X, w.Y].cost = Map[v.X, v.Y].cost + 1;
                        ////Console.WriteLine("Wait1");
                        //System.Threading.Thread.Sleep(100);
                        // koszty po skosie i po prostej mogą się różnić. Dzięki temu algorytm 
                        // częściej będzie wybierać drogę po skosie (jeśli koszt po skosie będzie mniejszy niż po prostej)
                        // lub drogę po prostej (w przeciwnym wypadku)

                        // ... i oznacz pole jako odwiedzone
                        Map[w.X, w.Y].visited = true;

                        // Jeśli trafiono na punkt końcowy (cel)
                        if ((w.X == ex) && (w.Y == ey)) // wyjscie
                        {
                            foundway = true; // przerwie pętlę
                            Q.Clear(); // wyczyść kolejkę
                        }
                        else
                        {
                            // a jeśli nie, to dodaj punkt do listy
                            Q.Enqueue(w);
                        }
                    }
                }
            }
            if (foundway == false)
            {
                // jeśli nie odnaleziono drogi do celu, to podejdź jak najbliżej

                // najpierw znajdź pole znajdujące się najbliżej celu wśród tych, które odwiedziliśmy

                double smlDistance = -1; // najmniejsza odległość jaką dotychczas znaleziono
                // punkt, w którym znaleziono najmniejszą odległość

                for (int x = 0; x < Map.GetLength(0); x++)
                    for (int y = 0; y < Map.GetLength(1); y++)
                    {
                        // do obliczania odległości danego pola od celu wykorzystamy prosty wzór:
                        // sqrt( ox^2 + oy^2 ), gdzie ox to odległość obu pól w osi X
                        // gdzie oy to odległość obu pól w osi Y
                        if (Map[x, y].visited)
                        {
                            // jeśli pole zostało odwiedzone (czyli można do niego dojść)
                            double distance; // odległość
                            distance = Math.Sqrt(Math.Pow(ex - x, 2) + Math.Pow(ey - y, 2));

                            if (distance < smlDistance || smlDistance == -1)
                            {
                                // jeśli znaleziono mniejszą odległość od dotychczasowej lub jeszcze żadnej nie przypisano
                                // to zapisz dane tego pola...
                                smlDistance = distance;
                                smlPoint = new Point(x, y);
                                ////Console.WriteLine(smlPoint.X.ToString() + " " + smlPoint.Y.ToString());
                            }
                        }
                    }

                if (smlPoint.X != -1 && smlPoint.Y != -1)
                {
                    // jeśli znaleziono jakieś pole
                    //if (CreateWay(ref way, ref helpMap, sx, sy, smlPoint.X, smlPoint.Y, width, height))
                    foundway = true;
                }

                return true;
            }
            return false;
        }

        public bool CreateWay(int sx, int sy, int ex, int ey, int ax, int ay, ref CPlayer player)
        {
            Point v, w;

            // Krok 7: Budujemy drogę
            // Przypisz do v punkt końcowy (czyli ten na ktorym skonczylismy przeszukiwać mapę)
            v = new Point(ex, ey);
            // dodaj pole końcwoe do generowanej drogi
            Way.Insert(0, v);
            
            while (v.X != sx || v.Y != sy)
            {
                int smlCost = -1; // najmniejszy koszt jaki znaleziono wśród sąsiadów pola w
                int smlDir = -1; // kierunek w którym wystąpił najmniejszy koszt

                // Pętla po wszystkich kierunkach
                for (int d = 0; d < dirsCount; d++)
                {
                    w = new Point(v.X + dirs[d].X, v.Y + dirs[d].Y);

                    // jeśli punkt znajduję sie w obrębie mapy, nie jest z kolizją i ma mniejszy koszt 
                    // niż aktualnie znaleziony (lub jeszcze nie przypisany)
                    // oraz to pole zostało odwiedzone rpzez algorytm
                    if (IsOnMap(w.X, w.Y, Map.GetLength(0), Map.GetLength(1)) && (Map[w.X, w.Y].cost < smlCost || smlCost == -1) && !Blocks[w.X, w.Y].IsObstale()
                        && Map[w.X, w.Y].visited /*&& !Blocks[w.X, w.Y].Occupied/*&& !Blocks[w.X, w.Y].IsUnitOnBlocks(ref player)*/)
                    {
                        // to zapisz dane tego punktu (koszt oraz kierunek)
                        smlCost = Map[w.X, w.Y].cost;
                        smlDir = d;
                    }
                }

                // jeśli znaleziono jakiś kierunek
                if (smlDir != -1)
                {
                    // to przypisz do v tego sąsiada
                    v = new Point(v.X + dirs[smlDir].X, v.Y + dirs[smlDir].Y);
                    // i dodaj v na początek listy (dzięki temu nie będziemy potem musieli odwracać listy)
                    Way.Insert(0, v);
                    //Console.WriteLine(Way.Count.ToString() + " - " + Way[0].X.ToString() + " " + Way[0].Y.ToString() + "  V.X: " + v.X.ToString() + "  V.Y: " + v.Y.ToString() + " sx: " + sx.ToString() + " sy: " + sy.ToString());
                }
                else
                {
                    // w przeciwnym wypadku opuść metodę i zwróć false
                    // raczej nigdy nie powinno się przydarzyć, ale jeśli jednak to dzięki takiemu rozwiązaniu 
                    // algorytm się nie zapętla
                    return false;
                }
            }
            if (ax != -1 && ay != -1)
            {
                //Way.Insert(0, new Point(ax, ay));
            }
            return true;
        }


    }
}
