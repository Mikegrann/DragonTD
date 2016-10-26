using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace DragonTD
{
    class Obstacle : HexEntity
    {
        public enum ObstacleType { Wall, Lake, Rock, Tree}

        List<Texture2D> Textures = new List<Texture2D>();

        Random random;

        public ObstacleType Type { get; private set; }

        public Obstacle(Game game, Level level, Point position, ObstacleType type) : base(game, level, position, (AnimatedSprite)null, false)
        {
            Type = type;
            random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            switch (Type)
            {
                case ObstacleType.Wall:
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile1"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile2"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile3"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile4"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile5"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/BoneTiles/BoneTile6"));
                    break;
                case ObstacleType.Lake:
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/Obstacles/Lake"));
                    break;
                case ObstacleType.Rock:
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/Obstacles/Rock1"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/Obstacles/Rock2"));
                    break;
                default:
                case ObstacleType.Tree:
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/Obstacles/Tree1"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/Obstacles/Tree2"));
                    Textures.Add(Game.Content.Load<Texture2D>("Textures/Obstacles/Tree3orTurd1"));
                    break;
            }

            Texture = new AnimatedSprite(new Texture2D[] { Textures[random.Next(0, Textures.Count)] }, Color.White, 1f);
        }
    }
}
