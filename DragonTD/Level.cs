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

        public Level(Game game) : base(game)
        {
            Width = 16;
            Height = 9;

            EnemyList = new List<Enemy>();
            ProjectileList = new List<Projectile>();

            InitializeMap();

            /* Temporary Test Map */
            PlaceHexEntity(new Obstacle(game, this, new Point(3, 0), game.Content.Load<Texture2D>("textures/obstacles/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 1), game.Content.Load<Texture2D>("textures/obstacles/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 2), game.Content.Load<Texture2D>("textures/obstacles/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(4, 3), game.Content.Load<Texture2D>("textures/obstacles/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(3, 4), game.Content.Load<Texture2D>("textures/obstacles/wall")));
            PlaceHexEntity(new Obstacle(game, this, new Point(2, 2), game.Content.Load<Texture2D>("textures/obstacles/wall")));

            PlaceHexEntity(new Obstacle(game, this, new Point(5, 4), game.Content.Load<Texture2D>("textures/obstacles/tree1")));
            PlaceHexEntity(new Obstacle(game, this, new Point(6, 5), game.Content.Load<Texture2D>("textures/obstacles/tree2")));

            PlaceHexEntity(new ProjectileTower(game, this, new Point(2, 4), ProjectileTower.ProjectileTowerType.Basic));

            WaveManager wm = new WaveManager(game, this);
            wm.StartWave(Start, Goal, Map);
        }

        // Randomize starting map
        public void InitializeMap()
        {
            Random rand = new Random();
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
