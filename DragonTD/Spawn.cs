using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DragonTD.Enemy;

namespace DragonTD
{
    class Spawn : HexEntity
    {
        private List<HexEntity> RecentPath;
        private HexEntity RecentGoal;

        public Spawn(Game game, Level level, Point position, Texture2D texture) : base(game, level, position, texture, true)
        {

        }

        // Uses the most recently created Path (saved)
        public Enemy.Enemy CreateEnemy(Enemy.EnemyType Type)
        {
            if (Type == Enemy.EnemyType.Flying)
            {
                return new FlyingEnemy(this.Game, Enemy.Enemy.GetEnemyStats(Type), this.ScreenPosition, Enemy.Enemy.GetEnemyTexture(Game, Type), RecentGoal);
            }
            else
            {
                return new WalkingEnemy(this.Game, Enemy.Enemy.GetEnemyStats(Type), this.ScreenPosition, Enemy.Enemy.GetEnemyTexture(Game, Type), RecentPath);
            }
        }

        // Creates a new Path for this and subsequent calls
        public Enemy.Enemy CreateEnemy(Enemy.EnemyType Type, HexEntity Goal, List<HexEntity> Path)
        {
            RecentPath = Path;
            RecentGoal = Goal;

            return CreateEnemy(Type);
        }
    }
}
