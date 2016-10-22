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

        public List<TowerStats> LevelStats { get; protected set; }

        /// <summary>
        /// Number of frames until you can fire
        /// (0 = can fire now)
        /// </summary>
        public int FiringCooldown = 0;

        public Tower(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture, false)
        {
            
        }
    }
    
}
