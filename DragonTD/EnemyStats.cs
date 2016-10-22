using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    class EnemyStats
    {
        public int Health, Shields;
        public float Speed;

        public EnemyStats(int health, int shields, int speed)
        {
            Health = health;
            Shields = shields;
            Speed = speed;
        }
    }
}
