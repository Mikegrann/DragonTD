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
        public Spawn Start;
        public Treasure Goal;

        public int Width, Height;

        public List<Enemy> EnemyList;
        public List<Projectile> ProjectileList;

        public int Money;

        public Random rand;

        public Tower.Tower Building;

        public Level(Game game) : base(game)
        {
            Width = 16;
            Height = 9;

            EnemyList = new List<Enemy>();
            ProjectileList = new List<Projectile>();
            WaveManager wm = new WaveManager(game, this);

            rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            InitializeMap();

            /* Temporary Test Map - TODO: Remove */
            AddWall(new Point(3, 0));
            AddWall(new Point(4, 1));
            AddWall(new Point(4, 2));
            AddWall(new Point(4, 3));
            AddWall(new Point(3, 4));
            AddWall(new Point(2, 2));
            PlaceHexEntity(new ProjectileTower(game, this, new Point(2, 4), Tower.Tower.TowerType.Basic));
            wm.StartWave(Start, Goal, Map);
            /* End Temporary Setup */
        }

        // Randomize starting map
        public void InitializeMap()
        {
            List<Texture2D> ObstacleTextures = new List<Texture2D>();
            ObstacleTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/tree1"));
            ObstacleTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/tree2"));
            ObstacleTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/pond"));
            ObstacleTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/rock1"));
            ObstacleTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/rock2"));

            Texture2D testHex = Game.Content.Load<Texture2D>("textures/outlinehex");

            //create empty map.
            Map = new HexEntity[Height, Width];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //fill in map with passable null hexes
                    Map[y, x] = new HexEntity(Game, this, new Point(x, y), testHex, true);
                }
            }

            // Randomize spawn/treasure
            Start = new Spawn(Game, this, new Point(rand.Next(0, Width / 3), rand.Next(0, Height - 1)), testHex); // Left third
            Goal = new Treasure(Game, this, new Point(rand.Next(Width * 2 / 3, Width - 1), rand.Next(0, Height - 1)), Game.Content.Load<Texture2D>("textures/treasure")); // Right third

            PlaceHexEntity(Start);
            PlaceHexEntity(Goal);

            // Randomize obstacles, up to (but maybe less than) maxObstacles
            int maxObstacles = 10;
            for (int i = 0; i < maxObstacles; i++)
            {
                Point location = new Point(rand.Next(0, Width - 1), rand.Next(0, Height - 1));
                PlaceHexEntity(new Obstacle(Game, this, location, ObstacleTextures[rand.Next(0, ObstacleTextures.Count - 1)]));

                if (WaveManager.CreatePath(Start, Goal, Map).Count == 0) // Obstacle blocks the path
                {
                    PlaceHexEntity(new HexEntity(Game, this, location, testHex, true)); // Reset to null hex
                }
            }
        }

        public bool InBounds(HexEntity hex)
        {
            return ((hex.Position.Y >= 0 && hex.Position.Y < Height) && (hex.Position.X >= 0 && hex.Position.X < Width));
        }
        public bool IsPlaceable(HexEntity hex)
        {
            if (!InBounds(hex))
            {
                return false; // Out of Bounds
            }

            HexEntity current = Map[hex.Position.Y, hex.Position.X];
            if (current == Start || current == Goal)
            {
                return false; // Overwrites spawn or treasure room
            }

            Map[hex.Position.Y, hex.Position.X] = hex;
            if (WaveManager.CreatePath(Start, Goal, Map).Count == 0)
            {
                Map[hex.Position.Y, hex.Position.X] = current;
                return false; // Cuts off the Path
            }
            else
            {
                Map[hex.Position.Y, hex.Position.X] = current;
                return true;
            }
        }

        public bool PlaceHexEntity(HexEntity hex)
        {
            //if out of bounds, return false;
            if (InBounds(hex))
                Map[hex.Position.Y, hex.Position.X] = hex;
            else
                return false;
            return true;
        }
        public bool PlaceHexEntity(HexEntity hex, Point p)
        {
            hex.Position = p;
            hex.ScreenPosition = HexEntity.CalculateScreenPosition(p);
            return PlaceHexEntity(hex);
        }

        // Randomizes Wall Texture
        public void AddWall(Point p)
        {
            List<Texture2D> WallTextures = new List<Texture2D>();
            WallTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/wall1"));
            WallTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/wall2"));
            WallTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/wall3"));
            WallTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/wall4"));
            WallTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/wall5"));
            WallTextures.Add(Game.Content.Load<Texture2D>("textures/obstacles/wall6"));
            
            PlaceHexEntity(new Obstacle(Game, this, p, WallTextures[rand.Next(0, WallTextures.Count - 1)]));
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
                    else // Killed
                    {
                        Money += e.Stats.MoneyDropped;
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

            DrawPath();

            foreach (Enemy e in EnemyList)
            {
                e.Draw(gameTime);
            }

            foreach (Projectile p in ProjectileList)
            {
                p.Draw(gameTime);
            }

            if (Building != null)
            {
                Building.Draw(gameTime);
            }
        }

        public void DrawPath()
        {
            List<HexEntity> Path = WaveManager.CreatePath(Start, Goal, Map);
            for (int i = 0; i < Path.Count - 1; i++)
            {
                Vector2 diff = Path[i + 1].ScreenPosition - Path[i].ScreenPosition;
                float rot = (float)Math.PI / 2.0f + (float)Math.Atan2(diff.Y, diff.X);
                Texture2D tex = Game.Content.Load<Texture2D>("textures/arrow");

                Game.Services.GetService<SpriteBatch>().Draw(tex, Path[i].ScreenPosition + diff / 2.0f, null, Color.White, rot, new Vector2(tex.Width / 2, tex.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
