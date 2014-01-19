using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort_Tales_Client
{
    class CButton
    {
        public bool Visible;
        public int X;
        public int Y;
        public int Height;
        public int Width;
        public bool Hover;
        public string Text;
        public int Category;
        public int ID;
        public bool IsPlayedSong;

        public CButton(string txt, int x, int y, int w, int h, int c, int id)
        {
            Text = txt;
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Category = c;
            Visible = false;
            Hover = false;
            IsPlayedSong = false;
            ID = id;
        }

        public bool IsOver(int x, int y)
        {
            if (Visible)
            {
                if (new Rectangle(x, y, 1, 1).Intersects(new Rectangle(X, Y, Width, Height)))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
