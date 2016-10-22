using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD.Tower
{
    abstract class ProjectileTower : Tower
    {
        Projectile Bullet;

        public ProjectileTower(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture)
        {
            UpgradeLevel = 0;
        }
    }
}
