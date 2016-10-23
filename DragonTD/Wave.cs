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

        public void AddEnemies(int EnemyCount, Enemy.EnemyType EnemyType, float Delay, float Separation)
        {
            EnemyDepictions.Add(new EnemyWave(EnemyCount, EnemyType, Delay, Separation));
        }
    }

    class EnemyWave {
        public int Count;
        public Enemy.EnemyType Type;
        public float Delay, Separation;

        public EnemyWave(int count, Enemy.EnemyType type, float delay, float separation)
        {
            Count = count;
            Type = type;
            Delay = delay;
            Separation = separation;
        }
    }
}
