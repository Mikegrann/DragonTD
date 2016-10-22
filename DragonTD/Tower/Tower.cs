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

        ///<summary>
        ///Range in ScreenPosition
        ///</summary>
        public float Range;

        public Tower(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture, false)
        {

        }
    }
    
}
