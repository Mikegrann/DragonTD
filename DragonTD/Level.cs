﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Level : DrawableGameComponent
    {
        //REMEMBER! Y,X
        public HexEntity[,] Map;

        int Width, Height;

        public Level(Game game) : base(game)
        {
            Width = 8;
            Height = 8;
            //create empty map.
            Map = new HexEntity[Height, Width];
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    //fill in map with passable null hexes
                    Map[y, x] = new HexEntity(game, new Point(x, y), null, true);
                }
            }
        }
    }
}