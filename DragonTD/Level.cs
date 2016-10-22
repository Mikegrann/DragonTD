using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Level : DrawableGameComponent
    {
        //REMEMBER! Y,X
        public HexEntity[,] Map;

        public int Width, Height;

        public List<Enemy> EnemyList;

        public Level(Game game) : base(game)
        {
            Texture2D testHex = game.Content.Load<Texture2D>("textures/testhex");
            Width = 8;
            Height = 8;
            //create empty map.
            Map = new HexEntity[Height, Width];
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    //fill in map with passable null hexes
                    Map[y, x] = new HexEntity(game, this, new Point(x, y), testHex, true);
                }
            }
            PlaceHexEntity(new Obstacle(game, this, Point.Zero, game.Content.Load<Texture2D>("textures/wall")));
        }

        public bool PlaceHexEntity(HexEntity hex)
        {
            //if out of bounds, return false;
            if ((hex.Position.Y < 0 && hex.Position.Y >= Height) && (hex.Position.X < 0 && hex.Position.X >= Width))
                return false;
            else
                Map[hex.Position.Y, hex.Position.X] = hex;
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (HexEntity h in Map)
            {
                h.Draw(gameTime);
            }
        }

    }
}
