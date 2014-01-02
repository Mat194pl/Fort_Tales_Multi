using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort_Tales_Client
{
    class CMenuItem
    {
        private int X;
        private int Y;
        private int Width;
        private int Height;
        private Texture2D Tex;
        private int Id;
        public bool Hover { get; set; }
        public bool Selected { get; set; }
        public bool Visible { get; set; }

        public CMenuItem(int x, int y, int w, int h, ref Texture2D tex, int id)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Tex = tex;
            Id = id;
            Hover = false;
            Selected = false;
            Visible = false;
        }
        
        public bool IsIn(int x, int y)
        {            
            if (new Rectangle(x, y, 1, 1).Intersects(new Rectangle(X, Y, Width, Height)))
            {
                return true;
            }
            return false;
        }

        public int GetID()
        {
            return Id;
        }

        public void Draw(SpriteBatch spb, SpriteFont spf)
        {
            if (Tex != null)
            {
                if (Selected)
                {
                    spb.Draw(Tex, new Vector2(X, Y), new Rectangle(400, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);
                }
                if (Hover)
                {
                    spb.Draw(Tex, new Vector2(X, Y), new Rectangle(350, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1.0f, SpriteEffects.None, 0f);                   
                }
                if (Id == 1)
                {
                    spb.Draw(Tex, new Vector2(X + 5, Y), new Rectangle(250, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
                }
                if (Id == 2)
                {
                    spb.Draw(Tex, new Vector2(X + 5, Y), new Rectangle(0, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
                }
                if (Id == 3)
                {
                    spb.Draw(Tex, new Vector2(X + 5, Y), new Rectangle(0, 50, 100, 100), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 0.8f * 0.5f, SpriteEffects.None, 0f);
                }
                if (Id == 4)
                {
                    spb.Draw(Tex, new Vector2(X + 5, Y), new Rectangle(0, 200, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
                }
                if (Id == 5)
                {
                    spb.Draw(Tex, new Vector2(X + 5, Y), new Rectangle(0, 150, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
