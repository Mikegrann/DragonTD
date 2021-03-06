﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Tower
{
    class ProjectileTower : Tower
    {
        /// <summary>
        /// Default = fire at first target found in list
        /// Closest = fire at nearest target to tower
        /// Furthest = fire at target furthest along on the path
        /// Lightest = fire at target with least health
        /// Heaviest = fire at target with most health
        /// </summary>
        public enum TargetingMode { Default, Flying, Closest, Furthest, Lightest, Heaviest };

        public TargetingMode TargetType;

        public AnimatedSprite ProjectileTexture;

        public ProjectileTower(Game game, Level level, Point position, TowerType type) : base(game, level, position, type)
        {
            UpgradeLevel = 0;
            TargetType = TargetingMode.Default;
            Texture = GetTowerTexture(game, TType, UpgradeLevel);
            ProjectileTexture = GetProjectileTexture(game, TType);
        }

        public override List<string> GetTowerStatsStrings(List<string> info)
        {
            if (info == null)
                info = new List<string>();

            base.GetTowerStatsStrings(info);

            info.Add(local.Get("targeting") + ": " + local.Get("TargetType."+TargetType.ToString()));

            switch (TType)
            {
                case TowerType.Basic:
                    info.Add(local.Get("damage") + ": " + ((ProjectileTowerStats)GetTowerStats()).BasicDamage);
                    break;
                case TowerType.Poison:
                    info.Add(local.Get("poisonDamage") + ": " + ((ProjectileTowerStats)GetTowerStats()).PoisonDamage);
                    info.Add(local.Get("poisonDuration") + ": " + ((ProjectileTowerStats)GetTowerStats()).PoisonDuration);
                    break;
                case TowerType.Piercing:
                    info.Add(local.Get("damage") + ": " + ((ProjectileTowerStats)GetTowerStats()).BasicDamage);
                    info.Add(local.Get("piercingDamage") + ": " + ((ProjectileTowerStats)GetTowerStats()).PiercingDamage);
                    info.Add(local.Get("multiHit") + ": " + ((ProjectileTowerStats)GetTowerStats()).MultiHit);
                    break;
                case TowerType.Sniper:
                    info.Add(local.Get("damage") + ": " + ((ProjectileTowerStats)GetTowerStats()).BasicDamage);
                    break;
                case TowerType.Explosive:
                    info.Add(local.Get("splashDamage") + ": " + ((ProjectileTowerStats)GetTowerStats()).BasicDamage);
                    break;
                default:
                    break;
            }
            
            return info;
        }

        /// <summary>
        /// Looks for candidate enemies and shoots a projectile
        /// at the best candidate (if any found).
        /// </summary>
        /// <param name="gameTime">dt argument for update loop</param>
        public override void Update(GameTime gameTime)
        {
            Enemy.Enemy target = FindEnemy(Level.EnemyList);

            float RotationTarget;
            if (target != null)
            {
                RotationTarget = (float)Math.PI / 2f + 
                    (float)System.Math.Atan2(target.ScreenPosition.Y - ScreenPosition.Y, 
                    target.ScreenPosition.X - ScreenPosition.X);

                float RotationAmount = (RotationTarget - Rotation);
                if (RotationAmount > Math.PI) { RotationAmount -= 2f * (float)Math.PI; }
                if (RotationAmount < -Math.PI) { RotationAmount += 2f * (float)Math.PI; }

                RotationAmount /= 8.0f;

                // Clamp to no more than 30deg at any given time
                float maxRot = (float)Math.PI / 180f * 30f;
                if (RotationAmount > maxRot) { RotationAmount = maxRot; }
                if (RotationAmount < -maxRot) { RotationAmount = -maxRot; }

                Rotation += RotationAmount;
            }

            if (FiringCooldown <= 0)
            {
                if (target != null)
                {
                    CreateProjectile(target);
                    FiringCooldown = GetTowerStats(TType, UpgradeLevel).FireRate;
                }
            }
            else
            {
                FiringCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Look for an enemy based on the current type of
        /// targeting desired.
        /// </summary>
        /// <param name="EnemyList">All enemies on the map</param>
        /// <returns>Target Enemy (null if none)</returns>
        public Enemy.Enemy FindEnemy(List<Enemy.Enemy> EnemyList)
        {
            Enemy.Enemy candidate = null;

            foreach(Enemy.Enemy e in EnemyList)
            {
                float dist = Util.Distance(ScreenPosition, e.ScreenPosition);

                if (dist < GetTowerStats(TType, UpgradeLevel).Range)
                {
                    if (candidate == null) candidate = e;

                    switch (TargetType)
                    {
                        case TargetingMode.Default:
                            return e;
                        case TargetingMode.Flying:
                            if (e.GetType() == typeof(Enemy.FlyingEnemy))
                            {
                                return e;
                            }
                            break;
                        case TargetingMode.Closest:
                            if (dist < Util.Distance(ScreenPosition, candidate.ScreenPosition))
                            {
                                candidate = e;
                            }
                            break;
                        case TargetingMode.Furthest:
                            if (e.GetDistanceFromGoal() < candidate.GetDistanceFromGoal())
                            {
                                candidate = e;
                            }
                            break;
                        case TargetingMode.Lightest:
                            if (e.Stats.Health < candidate.Stats.Health)
                            {
                                candidate = e;
                            }
                            break;
                        case TargetingMode.Heaviest:
                            if (e.Stats.Health > candidate.Stats.Health)
                            {
                                candidate = e;
                            }
                            break;
                    }
                }
            }

            return candidate;
        }

        public void CreateProjectile(Enemy.Enemy target)
        {
            Level.AddProjectile(new Projectile(Game, ProjectileTexture, null, (ProjectileTowerStats)GetTowerStats(TType, UpgradeLevel), 
                ScreenPosition + 60f * new Vector2((float)Math.Cos(Rotation - Math.PI / 2.0), (float)Math.Sin(Rotation - Math.PI / 2.0)), target.ScreenPosition));
        }
    }
}
