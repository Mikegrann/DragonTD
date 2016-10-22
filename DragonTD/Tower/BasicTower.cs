using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DragonTD.Tower
{
    class BasicTower : ProjectileTower
    {
        TowerStats L1Stats = new TowerStats(128f, 5);
        TowerStats L2Stats = new TowerStats(192f, 10);
        TowerStats L3Stats = new TowerStats(256f, 20);
        
        Texture2D TowerTexture;

        public BasicTower(Game game, Point position, Texture2D towerTexture, Texture2D projectileTexture) : base(game, position, towerTexture)
        {
            
        }
    }
}
