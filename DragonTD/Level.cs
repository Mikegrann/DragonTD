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

        SpriteBatch spriteBatch;

        public Level(Game game) : base(game)
        {
            spriteBatch = game.Services.GetService<SpriteBatch>();
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
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            //for now, draw at 0.5 scale.
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(0.5f));
            foreach (HexEntity h in Map)
            {
                h.Draw(gameTime);
            }
            spriteBatch.End();
        }

    }
}
