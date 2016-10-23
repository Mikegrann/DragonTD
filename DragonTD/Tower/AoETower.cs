using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DragonTD.Tower
{
    class AoETower : Tower
    {
        public AoETower(Game game, Level level, Point position, TowerType type) : base(game, level, position, type)
        {
            UpgradeLevel = 0;
            Texture = GetTowerTexture(game, TType, UpgradeLevel);
        }

        /// <summary>
        /// Looks for candidate enemies and shoots a projectile
        /// at the best candidate (if any found).
        /// </summary>
        /// <param name="gameTime">dt argument for update loop</param>
        public override void Update(GameTime gameTime)
        {
            List<Enemy.Enemy> targets = FindEnemy(Level.EnemyList);

            float RotationTarget;
            if (targets.Count > 0)
            {
                RotationTarget = (float)Math.PI / 2f +
                    (float)System.Math.Atan2(targets[0].ScreenPosition.Y - ScreenPosition.Y,
                    targets[0].ScreenPosition.X - ScreenPosition.X);

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
                if (targets.Count > 0)
                {
                    foreach(Enemy.Enemy e in targets)
                    {
                        ApplyEffect(e);
                    }
                    FiringCooldown = GetTowerStats().FireRate;
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
        public List<Enemy.Enemy> FindEnemy(List<Enemy.Enemy> EnemyList)
        {
            List<Enemy.Enemy> candidates = new List<Enemy.Enemy>();
            foreach (Enemy.Enemy e in EnemyList)
            {
                float dist = Util.Distance(ScreenPosition, e.ScreenPosition);

                if (dist < GetTowerStats(TType, UpgradeLevel).Range)
                {
                    candidates.Add(e);
                }
            }
            return candidates;
        }

        public void ApplyEffect(Enemy.Enemy Other)
        {
            // Apply Basic
            Other.Stats.Health -= ((AoETowerStats)GetTowerStats()).Damage;

            Other.SpeedDebuff = ((AoETowerStats)GetTowerStats()).SpeedDebuff;
            Other.SpeedDebuffTimer = ((AoETowerStats)GetTowerStats()).SpeedDebuffTime;
        }
    }
}
