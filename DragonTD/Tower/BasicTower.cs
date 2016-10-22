using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DragonTD.Tower
{
    class BasicTower : ProjectileTower
    {
        public BasicTower(Game game, Level level, Point position, Texture2D towerTexture, Texture2D projectileTexture) : base(game, level, position, towerTexture)
        {
            LevelStats.Add(new TowerStats(128f, 5));
            LevelStats.Add(new TowerStats(192f, 10));
            LevelStats.Add(new TowerStats(256f, 20));
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            
        }
    }
}
