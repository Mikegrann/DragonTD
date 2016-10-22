using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DragonTD.Tower
{
    class BasicTower : ProjectileTower
    {
        TowerStats L1Stats = new TowerStats(128f, 5);
        TowerStats L2Stats = new TowerStats(192f, 10);
        TowerStats L3Stats = new TowerStats(256f, 20);

        public BasicTower(Game game, Level level, Point position, Texture2D towerTexture, Texture2D projectileTexture) : base(game, level, position, towerTexture)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            
        }
    }
}
