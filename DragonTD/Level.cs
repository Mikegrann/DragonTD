using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Level : DrawableGameComponent
    {
        //REMEMBER! Y,X
        public HexEntity[,] Map;

        public Level(Game game) : base(game)
        {

        }
    }
}
