using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Obstacle : HexEntity
    {
        public Obstacle(Game game, Point position, Texture2D texture) : base(game, position, texture, false)
        {
        }
    }
}
