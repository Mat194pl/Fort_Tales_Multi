using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fort_Tales;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort_Tales_Server
{
    class CMapBuilder
    {
        CBlock[,] Blocks;
        Texture2D MapTexture;

        public CMapBuilder(ref CBlock[,] blocks, Texture2D maptex)
        {
            Blocks = blocks;
            MapTexture = maptex;
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        public void GenerateMapFromTexture()
        {
            Color[,] ColorMap = TextureTo2DArray(MapTexture);
            for (int i = 0; i < ColorMap.GetLength(0); i++ )
            {
                for (int j = 0; j < ColorMap.GetLength(1); j++)
                {
                    Blocks[i, j] = new CBlock();
                    Blocks[i, j].Init(i, j);
                    if (ColorMap[i, j].R != 255 && ColorMap[i, j].G != 255 && ColorMap[i, j].B != 255)
                    {
                        Blocks[i, j].Occupied = true;
                        Blocks[i, j].SetObjectID(-1);
                        Blocks[i, j].buildable = false;
                        if (ColorMap[i, j].G == 100)
                        {
                            Blocks[i, j].Terrain_type = 1;
                            Blocks[i, j].Occupied = false;
                        }
                        if (ColorMap[i, j].G == 50)
                        {
                            Blocks[i, j].Terrain_type = 2;
                        }
                        if (ColorMap[i, j].R == 50)
                        {
                            Blocks[i, j].Terrain_type = 3;
                        }
                        if (ColorMap[i, j].B == 50)
                        {
                            Blocks[i, j].Terrain_type = 4;
                        }
                        if (ColorMap[i, j].B == 100)
                        {
                            Blocks[i, j].Terrain_type = 5;
                            Blocks[i, j].Occupied = false;
                        }
                        if (ColorMap[i, j].B == 150)
                        {
                            Blocks[i, j].Terrain_type = 6;
                            Blocks[i, j].Occupied = false;
                        }
                        if (ColorMap[i, j].G == 0 && ColorMap[i, j].R == 0 && ColorMap[i, j].B == 0)
                        {
                            Blocks[i, j].Terrain_type = -1;
                        }
                    }
                }
            }
        }
    }
}
