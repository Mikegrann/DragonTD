using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DragonTD.Tower
{
    class BasicTower : ProjectileTower
    {
        public BasicTower(Game game, Level level, Point position) : base(game, level, position)
        {
            Texture = Game.Content.Load<Texture2D>("textures/towers/basic");
            ProjectileTexture = Game.Content.Load<Texture2D>("textures/projectiles/basic");
            LevelStats.Add(new TowerStats(128f, 0.5f, 128f, 1));
            LevelStats.Add(new TowerStats(192f, 0.3f, 128f, 2));
            LevelStats.Add(new TowerStats(256f, 0.2f, 128f, 3));
        }
    }
}
