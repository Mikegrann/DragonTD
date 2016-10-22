using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Obstacle : HexEntity
    {
        public Obstacle(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture, false)
        {
        }
    }
}
