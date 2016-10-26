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

        internal Localization local;

        public TowerType TType;

        //TODO: initialize dictionary at game.LoadContent.
        public static Dictionary<TowerType, List<TowerStats>> AllTowerStats = new Dictionary<TowerType, List<TowerStats>>()
        {
            { TowerType.Basic, new List<TowerStats>     { new ProjectileTowerStats(193f, 0.5f, 50, 256f, 3),
                                                          new ProjectileTowerStats(257f, 0.3f, 75, 256f, 5),
                                                          new ProjectileTowerStats(321f, 0.2f, 100, 256f, 7) } },

            { TowerType.Freeze, new List<TowerStats>    { new AoETowerStats(193f, 1f, 60, 0, 0.75f, 1f),
                                                          new AoETowerStats(257f, 1f, 95, 0, 0.5f, 1.5f),
                                                          new AoETowerStats(321f, 1f, 140, 0, 0.25f, 2f) } },

            { TowerType.Lightning, new List<TowerStats> { new AoETowerStats(193f, 1.5f, 80, 3),
                                                          new AoETowerStats(257f, 0.8f, 120, 3),
                                                          new AoETowerStats(321f, 0.3f, 200, 3) } },

            { TowerType.Poison, new List<TowerStats>    { new ProjectileTowerStats(385f, 0.8f, 70, 192f, 0, 2, 1, 1.0f),
                                                          new ProjectileTowerStats(513f, 0.75f, 130, 192f, 0, 2, 2, 1.5f),
                                                          new ProjectileTowerStats(641f, 0.7f, 200, 256f, 0, 2, 3, 2.0f) } },

            { TowerType.Piercing, new List<TowerStats>  { new ProjectileTowerStats(513f, 1.5f, 110, 512f, 1, 2, multi: 2),
                                                          new ProjectileTowerStats(641f, 1.1f, 140, 512f, 1, 3, multi: 4),
                                                          new ProjectileTowerStats(769f, 1.0f, 200, 512f, 2, 4, multi: 8) } },

            { TowerType.Sniper, new List<TowerStats>    { new ProjectileTowerStats(769f, 3.5f, 120, 1024f, 10),
                                                          new ProjectileTowerStats(1025f, 3.0f, 210, 1280f, 12),
                                                          new ProjectileTowerStats(1025f, 2.5f, 330, 1280f, 14) } },

            { TowerType.Explosive, new List<TowerStats> { new ProjectileTowerStats(513f, 2.5f, 90, 192f, 3, splash:193f),
                                                          new ProjectileTowerStats(641f, 2.1f, 130, 192f, 5, splash:193f),
                                                          new ProjectileTowerStats(769f, 2.0f, 180, 192f, 7, splash:193f) } },
        };
        
        //WOW, never thought I'd have to cast null before.
        public Tower(Game game, Level level, Point position, TowerType type) : base(game, level, position, (AnimatedSprite)null, false)
        {
            local = game.Services.GetService<Localization>();
            TType = type;
            this.Cost = GetTowerStats().Cost;
        }

        public TowerStats GetTowerStats()
        {
            return GetTowerStats(TType, UpgradeLevel);
        }

        //returns if this tower can be upgraded or not.
        public bool CanUpgrade()
        {
            return (UpgradeLevel < AllTowerStats[TType].Count - 1) ;
        }

        //returns how much it cost to just upgrade the tower.
        public int Upgrade()
        {
            if(CanUpgrade())
            {
                int c = CostToUpgrade;
                Cost += CostToUpgrade;
                UpgradeLevel++;
                return c;
            }
            return 0;
        }

        //returns how much it costs to upgrade.
        public int CostToUpgrade
        {
            get
            {
                if (CanUpgrade())
                    return AllTowerStats[TType][UpgradeLevel + 1].Cost;
                return 0;
            }
        }

        //to be implemented by subclasses.
        public virtual List<string> GetTowerStatsStrings(List<string> info)
        {
            if (info == null)
                info = new List<string>();
            info.Add(local.Get("Tower."+TType.ToString()));
            info.Add(local.Get("cost") + ": " + GetTowerStats().Cost);
            info.Add(local.Get("upgradeLevel") + ": " + (UpgradeLevel + 1));
            info.Add(local.Get("range") + ": " + GetTowerStats().Range);
            info.Add(local.Get("fireRate") + ": " + GetTowerStats().FireRate);

            return info;
        }

        // TODO: Set ProjectileTowerStats (maybe implement reads from config files?)
        public static TowerStats GetTowerStats(TowerType type, int level)
        {
            return AllTowerStats[type][level];
        }


        static string TTexDir = "Textures/Dragons/";
        static float TAnimTime = 0.15f;
        // TODO: Set TowerTextures
        public static AnimatedSprite GetTowerTexture(Game game, TowerType type, int level)
        {
            switch (type)
            {
                default:
                case TowerType.Basic:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal12"), game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal3478"), game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal56"), game.Content.Load<Texture2D>(TTexDir + "Red Dragon/Redfinal3478") }, Color.White, TAnimTime);
                case TowerType.Freeze:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "White Dragon/Whitefinal12"), game.Content.Load<Texture2D>(TTexDir + "White Dragon/Whitefinal3478"), game.Content.Load<Texture2D>(TTexDir + "White Dragon/Whitefinal56"), game.Content.Load<Texture2D>(TTexDir + "White Dragon/Whitefinal3478") }, Color.White, TAnimTime);
                case TowerType.Explosive:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Black Dragon/Blackfinal12"), game.Content.Load<Texture2D>(TTexDir + "Black Dragon/Blackfinal3478"), game.Content.Load<Texture2D>(TTexDir + "Black Dragon/Blackfinal56"), game.Content.Load<Texture2D>(TTexDir + "Black Dragon/Blackfinal3478") }, Color.White, TAnimTime);
                case TowerType.Lightning:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Yellow Dragon/Yellowfinal12"), game.Content.Load<Texture2D>(TTexDir + "Yellow Dragon/Yellowfinal3478"), game.Content.Load<Texture2D>(TTexDir + "Yellow Dragon/Yellowfinal56"), game.Content.Load<Texture2D>(TTexDir + "Yellow Dragon/Yellowfinal3478") }, Color.White, TAnimTime);
                case TowerType.Piercing:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Blue Dragon/Bluefinal12"), game.Content.Load<Texture2D>(TTexDir + "Blue Dragon/Bluefinal3478"), game.Content.Load<Texture2D>(TTexDir + "Blue Dragon/Bluefinal56"), game.Content.Load<Texture2D>(TTexDir + "Blue Dragon/Bluefinal3478") }, Color.White, TAnimTime);
                case TowerType.Poison:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Green Dragon/Greenfinal12"), game.Content.Load<Texture2D>(TTexDir + "Green Dragon/Greenfinal3478"), game.Content.Load<Texture2D>(TTexDir + "Green Dragon/Greenfinal56"), game.Content.Load<Texture2D>(TTexDir + "Green Dragon/Greenfinal3478") }, Color.White, TAnimTime);
                case TowerType.Sniper:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(TTexDir + "Purple Dragon/Purplefinal12"), game.Content.Load<Texture2D>(TTexDir + "Purple Dragon/Purplefinal3478"), game.Content.Load<Texture2D>(TTexDir + "Purple Dragon/Purplefinal56"), game.Content.Load<Texture2D>(TTexDir + "Purple Dragon/Purplefinal3478") }, Color.White, TAnimTime);

            }
        }

        static string PTexDir = "Textures/Projectiles/";
        static float PAnimTime = 0.1f;
        public static AnimatedSprite GetProjectileTexture(Game game, TowerType type)
        {
            switch (type)
            {
                default:
                case TowerType.Basic:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(PTexDir + "Red Fireball/Grann1"), game.Content.Load<Texture2D>(PTexDir + "Red Fireball/Grann2"), game.Content.Load<Texture2D>(PTexDir + "Red Fireball/Grann3") }, Color.White, PAnimTime);
                case TowerType.Explosive:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(PTexDir + "Black Ball/ball") }, Color.White, PAnimTime);
                case TowerType.Piercing:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(PTexDir + "Blue Beam/Beam1"), game.Content.Load<Texture2D>(PTexDir + "Blue Beam/Beam2"), game.Content.Load<Texture2D>(PTexDir + "Blue Beam/Beam3") }, Color.White, PAnimTime);
                case TowerType.Poison:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(PTexDir + "Green Acidball/Acid1"), game.Content.Load<Texture2D>(PTexDir + "Green Acidball/Acid2"), game.Content.Load<Texture2D>(PTexDir + "Green Acidball/Acid3") }, Color.White, PAnimTime);
                case TowerType.Sniper:
                    return new AnimatedSprite(new Texture2D[] { game.Content.Load<Texture2D>(PTexDir + "Purple Pew/Pew1"), game.Content.Load<Texture2D>(PTexDir + "Purple Pew/Pew2"), game.Content.Load<Texture2D>(PTexDir + "Purple Pew/Pew3") }, Color.White, PAnimTime);
            }
        }
    }
    
}
