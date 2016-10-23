using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class Wave : GameComponent
    {
        public List<EnemyWave> EnemyDepictions;

        public Wave(Game game) : base(game)
        {
            EnemyDepictions = new List<EnemyWave>();
        }

        public void AddEnemies(int EnemyCount, Enemy.EnemyType EnemyType, float Delay)
        {
            EnemyDepictions.Add(new EnemyWave(EnemyCount, EnemyType, Delay));
        }
    }

    class EnemyWave {
        public int Count;
        public Enemy.EnemyType Type;
        public float Delay;

        public EnemyWave(int count, Enemy.EnemyType type, float delay)
        {
            Count = count;
            Type = type;
            Delay = delay;
        }
    }
}
