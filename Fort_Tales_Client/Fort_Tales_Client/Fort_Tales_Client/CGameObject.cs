using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fort_Tales_Client
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
}
