using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;
using Lidgren.Network.Xna;

namespace Fort_Tales_Client
{
    class CInputWindow
    {
        private Keys[] PressedKeys;
        private string Text { get; set; }
        public bool IsEnter;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public bool Active;

        public CInputWindow(int x, int y, int w, int h)
        {
            IsEnter = false;
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Active = false;
        }

        public void AddKey(KeyboardState k)
        {
            PressedKeys = k.GetPressedKeys();
            if (PressedKeys.Count() != 0)
            {
                switch (PressedKeys[0])
                {
                    case Keys.D0:
                        Text += "0";
                        break;

                    case Keys.D1:
                        Text += "1";
                        break;

                    case Keys.D2:
                        Text += "2";
                        break;

                    case Keys.D3:
                        Text += "3";
                        break;

                    case Keys.D4:
                        Text += "4";
                        break;

                    case Keys.D5:
                        Text += "5";
                        break;

                    case Keys.D6:
                        Text += "6";
                        break;

                    case Keys.D7:
                        Text += "7";
                        break;

                    case Keys.D8:
                        Text += "8";
                        break;

                    case Keys.D9:
                        Text += "9";
                        break;

                    case Keys.NumPad0:
                        Text += "0";
                        break;

                    case Keys.NumPad1:
                        Text += "1";
                        break;

                    case Keys.NumPad2:
                        Text += "2";
                        break;

                    case Keys.NumPad3:
                        Text += "3";
                        break;

                    case Keys.NumPad4:
                        Text += "4";
                        break;

                    case Keys.NumPad5:
                        Text += "5";
                        break;

                    case Keys.NumPad6:
                        Text += "6";
                        break;

                    case Keys.NumPad7:
                        Text += "7";
                        break;

                    case Keys.NumPad8:
                        Text += "8";
                        break;

                    case Keys.NumPad9:
                        Text += "9";
                        break;

                    case Keys.A:
                        Text += "a";
                        break;

                    case Keys.B:
                        Text += "b";
                        break;

                    case Keys.C:
                        Text += "c";
                        break;

                    case Keys.D:
                        Text += "d";
                        break;

                    case Keys.E:
                        Text += "e";
                        break;

                    case Keys.F:
                        Text += "f";
                        break;

                    case Keys.G:
                        Text += "g";
                        break;

                    case Keys.H:
                        Text += "h";
                        break;

                    case Keys.I:
                        Text += "i";
                        break;

                    case Keys.J:
                        Text += "j";
                        break;

                    case Keys.K:
                        Text += "k";
                        break;

                    case Keys.L:
                        Text += "l";
                        break;

                    case Keys.M:
                        Text += "m";
                        break;

                    case Keys.N:
                        Text += "n";
                        break;

                    case Keys.O:
                        Text += "o";
                        break;

                    case Keys.P:
                        Text += "p";
                        break;

                    case Keys.Q:
                        Text += "q";
                        break;

                    case Keys.R:
                        Text += "r";
                        break;

                    case Keys.S:
                        Text += "s";
                        break;

                    case Keys.T:
                        Text += "t";
                        break;

                    case Keys.U:
                        Text += "u";
                        break;

                    case Keys.V:
                        Text += "v";
                        break;

                    case Keys.W:
                        Text += "w";
                        break;

                    case Keys.X:
                        Text += "x";
                        break;

                    case Keys.Y:
                        Text += "y";
                        break;

                    case Keys.Z:
                        Text += "z";
                        break;

                    case Keys.OemPeriod:
                        Text += ".";
                        break;
                    
                    case Keys.Back:
                        if (Text.Length > 0)
                        {
                            Text = Text.Remove(Text.Length - 1);                         
                        }
                        break;
                    
                    case Keys.Enter:
                        IsEnter = true;
                        break;
                }
            }
        }

        public void Clear()
        {
            PressedKeys = null;
            Text = null;
            IsEnter = false;
        }

        public string ReturnText()
        {
            return Text;
        }
    }
}
