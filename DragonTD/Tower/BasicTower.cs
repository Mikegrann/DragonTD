using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DragonTD.Tower
{
    class BasicTower : ProjectileTower
    {
        public BasicTower(Game game, Level level, Point position, Texture2D towerTexture, Texture2D projectileTexture) : base(game, level, position, towerTexture)
        {
            LevelStats.Add(new TowerStats(128f, 0.5f, 1.5f, 1));
            LevelStats.Add(new TowerStats(192f, 0.3f, 1.5f, 2));
            LevelStats.Add(new TowerStats(256f, 0.2f, 1.5f, 3));
        }
    }
}
