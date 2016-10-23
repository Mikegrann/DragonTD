using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Tower
{
    abstract class Tower : HexEntity
    {
        /// <summary>
        /// Cost to build
        /// </summary>
        public int Cost;

        /// <summary>
        /// Level of the tower
        /// </summary>
        public int UpgradeLevel = 0;

        /// <summary>
        /// Number of seconds until you can fire
        /// (0 = can fire now)
        /// </summary>
        public float FiringCooldown = 0;
        
        public enum TowerType { Basic = 0, Freeze = 1, Lightning = 2, Poison = 3, Piercing = 4, Sniper = 5, Explosive = 6 };

        //TODO: initialize dictionary at game.LoadContent.
        public static Dictionary<TowerType, List<TowerStats>> AllTowerStats = new Dictionary<TowerType, List<TowerStats>>()
        {
            { TowerType.Basic, new List<TowerStats> { new TowerStats(128f, 0.5f, 256f, 1), new TowerStats(192f, 0.3f, 256f, 2), new TowerStats(256f, 0.2f, 256f, 3) } }
        };
        
        public Tower(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture, false)
        {
            
        }

        // TODO: Set TowerStats (maybe implement reads from config files?)
        public static TowerStats GetTowerStats(TowerType type, int level)
        {
            switch (type)
            {
                default:
                case TowerType.Basic:
                    return AllTowerStats[TowerType.Basic][level];
            }
        }

        // TODO: Set TowerTextures
        public static Texture2D GetTowerTexture(Game game, TowerType type, int level)
        {
            switch (type)
            {
                default:
                case TowerType.Basic:
                    return game.Content.Load<Texture2D>("textures/towers/basic");
            }
        }
        public static Texture2D GetProjectileTexture(Game game, TowerType type)
        {
            switch (type)
            {
                default:
                case TowerType.Basic:
                    return game.Content.Load<Texture2D>("textures/projectiles/basic");
            }
        }
    }
    
}
