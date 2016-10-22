using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Tower
{
    abstract class ProjectileTower : Tower
    {
        /// <summary>
        /// Default = fire at first target found in list
        /// Closest = fire at nearest target to tower
        /// Furthest = fire at target furthest along on the path
        /// Lightest = fire at target with least health
        /// Heaviest = fire at target with most health
        /// </summary>
        public enum TargetingMode { Default, Closest, Furthest, Lightest, Heaviest };

        public TargetingMode TargetType;

        public ProjectileTower(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture)
        {
            UpgradeLevel = 0;
            TargetType = TargetingMode.Default;
        }

        /// <summary>
        /// Looks for candidate enemies and shoots a projectile
        /// at the best candidate (if any found).
        /// </summary>
        /// <param name="gameTime">dt argument for update loop</param>
        public override void Update(GameTime gameTime)
        {
            if (FiringCooldown <= 0)
            {
                Enemy target = FindEnemy(Level.EnemyList);
                if (target != null)
                {
                    CreateProjectile(target);
                    FiringCooldown = LevelStats[UpgradeLevel].FireRate;
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
        public Enemy FindEnemy(List<Enemy> EnemyList)
        {
            Enemy candidate = null;

            foreach(Enemy e in EnemyList)
            {
                float dist = Util.Distance(ScreenPosition, e.ScreenPosition);

                if (dist < LevelStats[UpgradeLevel].Range)
                {
                    if (candidate == null) candidate = e;

                    switch (TargetType)
                    {
                        case TargetingMode.Default:
                            return e;
                        case TargetingMode.Closest:
                            if (dist < Util.Distance(ScreenPosition, candidate.ScreenPosition))
                            {
                                candidate = e;
                            }
                            break;
                        case TargetingMode.Furthest:
                            if (e.GetProgress() < candidate.GetProgress())
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

        public void CreateProjectile(Enemy target)
        {
            // TODO: construct projectile based on tower stats/type (maybe make subclasses implement?)
        }
    }
}
