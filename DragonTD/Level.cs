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

            EnemyList = new List<Enemy>();

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

            PlaceHexEntity(new Obstacle(game, this, new Point(0, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(1, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(2, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(3, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(5, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(6, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 1), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 2), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 3), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 4), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 5), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 6), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(0, 7), game.Content.Load<Texture2D>("textures/wall")));

            PlaceHexEntity(new Obstacle(game, this, new Point(0, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(1, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(2, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(3, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(5, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(6, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 7), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 1), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 2), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 3), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 4), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 5), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 6), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(7, 7), game.Content.Load<Texture2D>("textures/wall")));

            PlaceHexEntity(new Obstacle(game, this, new Point(4, 1), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 2), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 3), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(3, 4), game.Content.Load<Texture2D>("textures/wall")));

            Map[1, 3] = new Spawn(game, this, new Point(3, 1), testHex);
            Map[3, 6] = new Treasure(game, this, new Point(6, 3), testHex);
            WaveManager wm = new WaveManager(game, this);
            wm.StartWave((Spawn)Map[1, 3], (Treasure)Map[3, 6], Map);
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
            foreach (Enemy e in EnemyList)
            {
                e.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (HexEntity h in Map)
            {
                h.Draw(gameTime);
            }

            foreach (Enemy e in EnemyList)
            {
                e.Draw(gameTime);
            }
        }

    }
}
