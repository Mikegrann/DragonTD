using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Wave : GameComponent
    {
        public List<Tuple<int, Enemy.EnemyType>> EnemyDepictions;

        public Wave(Game game) : base(game)
        {
            EnemyDepictions = new List<Tuple<int, Enemy.EnemyType>>();
        }

        public void AddEnemies(int EnemyCount, Enemy.EnemyType EnemyType)
        {
            EnemyDepictions.Add(new Tuple<int, Enemy.EnemyType>(EnemyCount, EnemyType));
        }
    }
}
