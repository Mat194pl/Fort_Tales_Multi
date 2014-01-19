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
                spb.Draw(Tex, new Vector2(X, Y), new Rectangle(500, 0, 70, 70), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                if (Selected)
                {
                    spb.Draw(Tex, new Rectangle(X + 1, Y + 1, 60, 60), new Rectangle(400, 0, 50, 50), Color.White);                    
                }
                if (Hover)
                {
                    spb.Draw(Tex, new Rectangle(X + 1, Y + 1, 60, 60), new Rectangle(350, 0, 50, 50), Color.White);                   
                }
                if (Id == 1)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(250, 50, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "BUILD", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "BUILD", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 2)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(0, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "HOUSE", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "HOUSE", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 3)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(0, 50, 100, 100), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f * 0.5f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "BARRACKS", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "BARRACKS", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 4)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(0, 200, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "WALL", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "WALL", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 5)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(0, 150, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "ROAD", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "ROAD", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 6)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(200, 100, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "GATE", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "GATE", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 7)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(100, 100, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "FARM", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "FARM", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 8)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(150, 100, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "MINE", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "MINE", new Vector2(X + 8, Y + 50), Color.White);
                }
                if (Id == 9)
                {
                    spb.Draw(Tex, new Vector2(X + 8, Y + 2), new Rectangle(200, 0, 50, 50), Color.White, MathHelper.Pi * (0), new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    spb.DrawString(spf, "Watchtower", new Vector2(X + 9, Y + 51), Color.Black);
                    spb.DrawString(spf, "Watchtower", new Vector2(X + 8, Y + 50), Color.White);
                }
            }
        }
    }
}
