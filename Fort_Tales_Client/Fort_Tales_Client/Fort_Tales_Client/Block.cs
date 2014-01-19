using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales_Client
{
    class Block
    {
        public int X;
        public int Y;
        public int TerrainType;
        public int PlayerID;
        public bool Buildable;
        public Block(int x, int y)
        {
            X = x * 50;
            Y = y * 50;
        }
    }
}
