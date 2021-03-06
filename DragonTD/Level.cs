﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DragonTD.Tower;
using DragonTD.Enemy;

namespace DragonTD
{
    class Level : DrawableGameComponent
    {
        //REMEMBER! Y,X
        public HexEntity[,] Map;
        public Spawn Start;
        public Treasure Goal;

        public int Width, Height;

        public Vector2 ScreenSize { get; private set; }
        public Vector2 ScreenOffset { get; private set; }

        public List<Enemy.Enemy> EnemyList;
        public List<Projectile> ProjectileList;
        public List<AoEEffect> EffectList;
        WaveManager WM;

        public List<EnemyWave> NextWave { get { return WM.NextWave; } }
        public int CurrentWaveNumber { get { return WM.WaveNumber; } }
        public int WaveCount { get { return WM.Waves.Count; } }

        public float SimSpeed;

        public int Money = 100000;

        public Random rand;

        public HexEntity Building;

        Texture2D emptyHexTexture;

        public Level(Game game) : base(game)
        {
            InitializeLevel(game, new Point(16, 9));
            InitializeMap();
        }
        
        public Level(Game game, Point mapSize) : base(game)
        {
            InitializeLevel(game, mapSize);
            InitializeMap();
        }

        private void InitializeLevel(Game game, Point mapSize)
        {
            Width = mapSize.X;
            Height = mapSize.Y;

            SimSpeed = 1.0f;

            EnemyList = new List<Enemy.Enemy>();
            ProjectileList = new List<Projectile>();
            EffectList = new List<AoEEffect>();
            WM = new WaveManager(game, this);

            rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        }

        // Randomize starting map
        public void InitializeMap()
        {
            emptyHexTexture = Game.Content.Load<Texture2D>("Textures/UI/outlinehex");

            //calculate screensize of map.
            ScreenSize = new Vector2 ((emptyHexTexture.Width * Width) + (emptyHexTexture.Width / 2f), ((emptyHexTexture.Height*0.75f)*Height) + (emptyHexTexture.Height*0.25f));
            ScreenOffset = new Vector2(emptyHexTexture.Width / 2f, emptyHexTexture.Height / 2f);

            //create empty map.
            Map = new HexEntity[Height, Width];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //fill in map with passable null hexes
                    Map[y, x] = new HexEntity(Game, this, new Point(x, y), emptyHexTexture, true);
                }
            }

            // Randomize spawn/treasure
            Start = new Spawn(Game, this, new Point(rand.Next(0, Width / 3), rand.Next(0, Height - 1))); // Left third
            Goal = new Treasure(Game, this, new Point(rand.Next(Width * 2 / 3, Width - 1), rand.Next(0, Height - 1))); // Right third

            PlaceHexEntity(Start);
            PlaceHexEntity(Goal);

            // Randomize obstacles, up to (but maybe less than) maxObstacles
            int maxObstacles = 10;
            for (int i = 0; i < maxObstacles; i++)
            {
                Point location = new Point(rand.Next(0, Width - 1), rand.Next(0, Height - 1));
                PlaceHexEntity(new Obstacle(Game, this, location, (Obstacle.ObstacleType)rand.Next(1, 4)));

                if (WaveManager.CreatePath(Start, Goal, Map).Count == 0) // Obstacle blocks the path
                {
                    PlaceHexEntity(new HexEntity(Game, this, location, emptyHexTexture, true)); // Reset to null hex
                }
            }
        }

        public bool InBounds(HexEntity hex)
        {
            return ((hex.Position.Y >= 0 && hex.Position.Y < Height) && (hex.Position.X >= 0 && hex.Position.X < Width));
        }
        public bool InBounds(Point p)
        {
            return ((p.Y >= 0 && p.Y < Height) && (p.X >= 0 && p.X < Width));
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
            if (IsPlaceable(hex))
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

        public void UpgradeTower(Tower.Tower t)
        {
            Money -= t.CostToUpgrade;
            t.Upgrade();
        }

        public void SellHex(HexEntity hex)
        {
            Money += hex.SellPrice;
            PlaceHexEntity(new HexEntity(Game, this, hex.Position, emptyHexTexture, true));
        }

        // Randomizes Wall Texture
        public void AddWall(Point p)
        {
            PlaceHexEntity(new Obstacle(Game, this, p, Obstacle.ObstacleType.Wall));
        }

        public void AddProjectile(Projectile p)
        {
            ProjectileList.Add(p);
        }

        public bool IsWaveOngoing()
        {
            return WM.WaveOngoing;
        }

        public void StartNextWave()
        {
            WM.StartWave(Start, Goal, Map);
        }

        public override void Update(GameTime gameTime)
        {
            TimeSpan simSpan = new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks * SimSpeed ));
            GameTime simTime = new GameTime(gameTime.TotalGameTime, simSpan);

            WM.Update(simTime);

            //don't speed up when there is no wave. it makes the animations look weird while building.
            if (!WM.WaveOngoing)
                SimSpeed = 1f;

            foreach (HexEntity h in Map)
            {
                h.Update(simTime);
            }

            for (int i = EffectList.Count - 1; i >= 0; i--)
            {
                AoEEffect e = EffectList[i];
                e.Update(simTime);

                if (e.Done)
                {
                    EffectList.RemoveAt(i);
                }
            }

            for (int i = EnemyList.Count - 1; i >= 0; i--)
            {
                Enemy.Enemy e = EnemyList[i];
                e.Update(simTime);

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

            // Check for wave end
            if (EnemyList.Count == 0 && !WM.StillHasEnemiesToSpawn)
            {
                WM.WaveOngoing = false;
            }

            for (int i = ProjectileList.Count - 1; i >= 0; i--)
            {
                Projectile p = ProjectileList[i];
                p.Update(simTime);

                foreach (Enemy.Enemy e in EnemyList)
                {
                    // TODO: Implement variable bounding boxes
                    float hitboxSize = 32;
                    if (Util.Distance(p.Position, e.ScreenPosition) < hitboxSize)
                    {
                        p.ApplyEffect(e);

                        // Apply also to nearby enemies
                        if (p.Stats.SplashRadius > 0.0f)
                        {
                            EffectList.Add(new AoEEffect(Game, AoEEffect.EffectType.Explosion, p.Position));

                            foreach (Enemy.Enemy e2 in EnemyList)
                            {
                                if (Util.Distance(e.ScreenPosition, e2.ScreenPosition) < p.Stats.SplashRadius && !e.Equals(e2))
                                {
                                    p.ApplyEffect(e2);
                                }
                            }
                        }

                        if (p.MultiHit <= 0)
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
            TimeSpan simSpan = new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks * SimSpeed));
            GameTime simTime = new GameTime(gameTime.TotalGameTime, simSpan);

            foreach (AoEEffect e in EffectList)
            {
                e.Draw(simTime);
            }

            foreach (HexEntity h in Map)
            {
                h.Draw(simTime);
            }

            DrawPath();

            foreach (Enemy.Enemy e in EnemyList)
            {
                e.Draw(simTime);
            }

            foreach (Projectile p in ProjectileList)
            {
                p.Draw(simTime);
            }

            if (Building != null)
            {
                Building.Draw(simTime);
            }
        }

        public void DrawPath()
        {
            HexEntity Saved = null;
            if (Building != null && InBounds(Building))
            {
                Saved = Map[Building.Position.Y, Building.Position.X];
                Map[Building.Position.Y, Building.Position.X] = Building;
            }

            List<HexEntity> Path = WaveManager.CreatePath(Start, Goal, Map);
            for (int i = 0; i < Path.Count - 1; i++)
            {
                Vector2 diff = Path[i + 1].ScreenPosition - Path[i].ScreenPosition;
                float rot = (float)Math.PI / 2.0f + (float)Math.Atan2(diff.Y, diff.X);
                Texture2D tex = Game.Content.Load<Texture2D>("Textures/UI/arrow");

                Game.Services.GetService<SpriteBatch>().Draw(tex, Path[i].ScreenPosition + diff / 2.0f, null, Color.White, rot, new Vector2(tex.Width / 2, tex.Height / 2), 1f, SpriteEffects.None, 0f);
            }

            if (Building != null && InBounds(Building))
            {
                Map[Building.Position.Y, Building.Position.X] = Saved;
            }
        }
    }
}
