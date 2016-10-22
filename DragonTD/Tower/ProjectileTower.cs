using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Tower
{
    abstract class ProjectileTower : Tower
    {
        Projectile Bullet;

        public ProjectileTower(Game game, Point position, Texture2D texture) : base(game, position, texture)
        {
            Level = 0;
        }
    }
}
