using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Wave : GameComponent
    {
        List<Tuple<int, Enemy.EnemyType>> enemyDepictions;

        public Wave(Game game) : base(game)
        {

        }

        public void addWave(int EnemyCount, Enemy.EnemyType EnemyType)
        {
            enemyDepictions.Add(new Tuple<int, Enemy.EnemyType>(EnemyCount, EnemyType));
        }
    }
}
