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
        public int Level = 0;

        ///<summary>
        ///Range in ScreenPosition
        ///</summary>
        public float Range;

        public Tower(Game game, Point position, Texture2D texture) : base(game, position, texture, false)
        {

        }
    }
    
}
