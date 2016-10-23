using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Tower
{
    public enum TowerType { Basic = 0, Freeze = 1, Lightning = 2, Poison = 3, Piercing = 4, Sniper = 5, Explosive = 6 };
    abstract class Tower : HexEntity
    {
        /// <summary>
        /// Level of the tower
        /// </summary>
        public int UpgradeLevel = 0;

        /// <summary>
        /// Number of seconds until you can fire
        /// (0 = can fire now)
        /// </summary>
        public float FiringCooldown = 0;
        

        public TowerType TType;

        //TODO: initialize dictionary at game.LoadContent.
        public static Dictionary<TowerType, List<TowerStats>> AllTowerStats = new Dictionary<TowerType, List<TowerStats>>()
        {
            { TowerType.Basic, new List<TowerStats> { new ProjectileTowerStats(193f, 0.5f, 50, 256f, 1), new ProjectileTowerStats(257f, 0.3f, 75, 256f, 2), new ProjectileTowerStats(321f, 0.2f, 100, 256f, 3) } },
            { TowerType.Freeze, new List<TowerStats> { new AoETowerStats(193f, 1f, 50, 0, 0.75f, 1f), new AoETowerStats(257f, 0.5f, 75, 0, 0.5f, 1.5f), new AoETowerStats(321f, 0.5f, 100, 0, 0.25f, 2f) } }
        };
        
        public Tower(Game game, Level level, Point position, TowerType type) : base(game, level, position, null, false)
        {
            TType = type;
        }

        public TowerStats GetTowerStats()
        {
            return GetTowerStats(TType, UpgradeLevel);
        }

        // TODO: Set ProjectileTowerStats (maybe implement reads from config files?)
        public static TowerStats GetTowerStats(TowerType type, int level)
        {
            return AllTowerStats[type][level];
        }

        // TODO: Set TowerTextures
        public static Texture2D GetTowerTexture(Game game, TowerType type, int level)
        {
            switch (type)
            {
                default:
                case TowerType.Basic:
                    return game.Content.Load<Texture2D>("textures/towers/basic");
                //case TowerType.Freeze:
                //    return game.Content.Load<Texture2D>("textures/towers/freeze");
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
