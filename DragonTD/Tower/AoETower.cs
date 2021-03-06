﻿using System;
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

        public override List<string> GetTowerStatsStrings(List<string> info)
        {
            if (info == null)
                info = new List<string>();

            base.GetTowerStatsStrings(info);
            
            switch(TType)
            {
                case TowerType.Lightning:
                    info.Add(local.Get("lightningDamage") + ": " + ((AoETowerStats)GetTowerStats()).Damage);
                    break;
                case TowerType.Freeze:
                    info.Add(local.Get("slowDebuff") + ": " + ((AoETowerStats)GetTowerStats()).SpeedDebuff);
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
                    // Spawn effect drawing
                    Level.EffectList.Add(new AoEEffect(Game, GetEffect(TType), ScreenPosition));

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

        

        public static AoEEffect.EffectType GetEffect(TowerType type)
        {
            switch (type)
            {
                default:
                case TowerType.Freeze:
                    return AoEEffect.EffectType.Freeze;
                case TowerType.Lightning:
                    return AoEEffect.EffectType.Lightning;
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
            if (Other.Stats.Shields > 0) {
                Other.Stats.Shields -= (int)(1.5 * ((AoETowerStats)GetTowerStats()).Damage); // Double effectiveness on shields

                // Roll over extra damage from hits that break shields
                if (Other.Stats.Shields < 0) {
                    Other.Stats.Health += Other.Stats.Shields;
                    Other.Stats.Shields = 0;
                }
            }
            else {
                Other.Stats.Health -= ((AoETowerStats)GetTowerStats()).Damage;
            }

            //if the existing debuff is worse, keep it!
            Other.SpeedDebuff = Math.Min(((AoETowerStats)GetTowerStats()).SpeedDebuff, Other.SpeedDebuff);
            //if the existing debuff is longer, keep it!
            Other.SpeedDebuffTimer = Math.Max(((AoETowerStats)GetTowerStats()).SpeedDebuffTime, Other.SpeedDebuffTimer);
        }
    }
}
