using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Spawn : HexEntity
    {
        private List<HexEntity> RecentPath;

        public Spawn(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture, true)
        {

        }

        // Uses the most recently created Path (saved)
        public Enemy CreateEnemy(Enemy.EnemyType Type)
        {
            return new Enemy(this.Game, RecentPath, Enemy.GetEnemyStats(Type), this.ScreenPosition, Enemy.GetEnemyTexture(Game, Type));
        }

        // Creates a new Path for this and subsequent calls
        public Enemy CreateEnemy(List<HexEntity> Path, Enemy.EnemyType Type)
        {
            RecentPath = Path;

            return new Enemy(this.Game, Path, Enemy.GetEnemyStats(Type), this.ScreenPosition, Enemy.GetEnemyTexture(Game, Type));
        }
    }
}
