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
        
        //WOW, never thought I'd have to cast null before.
        public Tower(Game game, Level level, Point position, TowerType type) : base(game, level, position, (AnimatedSprite)null, false)
        {
            TType = type;
            this.Cost = GetTowerStats().Cost;
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
