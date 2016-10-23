using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DragonTD.Tower;

namespace DragonTD
{
    class Level : DrawableGameComponent
    {
        //REMEMBER! Y,X
        public HexEntity[,] Map;

        public int Width, Height;

        public List<Enemy> EnemyList;
        public List<Projectile> ProjectileList;

        public int Money;

        public Level(Game game) : base(game)
        {
            Texture2D testHex = game.Content.Load<Texture2D>("textures/testhex");
            Width = 8;
            Height = 8;

            EnemyList = new List<Enemy>();
            ProjectileList = new List<Projectile>();

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

            PlaceHexEntity(new Obstacle(game, this, new Point(3, 0), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 1), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 2), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 3), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(3, 4), game.Content.Load<Texture2D>("textures/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(2, 2), game.Content.Load<Texture2D>("textures/wall")));

            PlaceHexEntity(new ProjectileTower(game, this, new Point(2, 4), ProjectileTower.ProjectileTowerType.Basic));
            PlaceHexEntity(new Spawn(game, this, new Point(3, 1), testHex));
            PlaceHexEntity(new Treasure(game, this, new Point(6, 3), testHex));

            WaveManager wm = new WaveManager(game, this);
            wm.StartWave((Spawn)Map[1, 3], (Treasure)Map[3, 6], Map);
        }

        public bool PlaceHexEntity(HexEntity hex)
        {
            //if out of bounds, return false;
            if ((hex.Position.Y < 0 || hex.Position.Y >= Height) || (hex.Position.X < 0 || hex.Position.X >= Width))
                return false;
            else
                Map[hex.Position.Y, hex.Position.X] = hex;
            return true;
        }
        public bool PlaceHexEntity(HexEntity hex, Point p)
        {
            hex.Position = p;
            hex.ScreenPosition = HexEntity.CalculateScreenPosition(p);
            return PlaceHexEntity(hex);
        }

        public void AddProjectile(Projectile p)
        {
            ProjectileList.Add(p);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (HexEntity h in Map)
            {
                h.Update(gameTime);
            }

            for (int i = EnemyList.Count - 1; i >= 0; i--)
            {
                Enemy e = EnemyList[i];
                e.Update(gameTime);

                if (e.Dead)
                {
                    if (e.GetDistanceFromGoal() <= 0.0) // Reached end
                    {
                        Money -= e.Stats.TreasureStolen;
                        if (Money < 0)
                        {
                            Money = 0;
                        }
                    }
                    EnemyList.RemoveAt(i);
                }
            }

            for (int i = ProjectileList.Count - 1; i >= 0; i--)
            {
                Projectile p = ProjectileList[i];
                p.Update(gameTime);

                foreach (Enemy e in EnemyList)
                {
                    // TODO: Implement variable bounding boxes
                    if (Util.Distance(p.Position, e.ScreenPosition) < 16f)
                    {
                        p.ApplyEffect(e);

                        // Apply also to nearby enemies
                        if (p.Stats.SplashRadius > 0.0f)
                        {
                            foreach (Enemy e2 in EnemyList)
                            {
                                if (Util.Distance(e.ScreenPosition, e2.ScreenPosition) < p.Stats.SplashRadius && !e.Equals(e2))
                                {
                                    p.ApplyEffect(e);
                                }
                            }
                        }

                        if (--p.MultiHit <= 0)
                        {
                            p.Dead = true;
                            break;
                        }
                    }
                }
                
                if (p.Dead)
                {
                    ProjectileList.RemoveAt(i);
                }
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

            foreach (Projectile p in ProjectileList)
            {
                p.Draw(gameTime);
            }
        }

    }
}
