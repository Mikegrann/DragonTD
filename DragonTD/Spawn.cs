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

        public Spawn(Game game, Point position, Texture2D texture) : base(game, position, texture, true)
        {

        }

        // Uses the most recently created Path (saved)
        public Enemy CreateEnemy(EnemyStats Stats)
        {
            return new Enemy(this.Game, RecentPath, Stats, this.ScreenPosition);
        }

        // Creates a new Path for this and subsequent calls
        public Enemy CreateEnemy(List<HexEntity> Path, EnemyStats Stats)
        {
            RecentPath = Path;

            return new Enemy(this.Game, Path, Stats, this.ScreenPosition);
        }
    }
}
