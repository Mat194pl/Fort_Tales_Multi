using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Fort_Tales
{
    class CMap
    {
        private CBlock[,] Blocks = new CBlock[100, 300];

        public CMap()
        {
            CBlock[,] Blocks = new CBlock[100, 300];
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 300; j++)
                {
                    //Blocks[i, j].Init(i, j);
                }
            }
        }
    }
}
