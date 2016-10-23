using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonTD.Tower
{
    class TowerStats
    {
        public float Range { get; private set; }
        public float FireRate { get; private set; }
        public int Cost { get; private set; }

        public TowerStats (float range, float firerate, int cost)
        {
            Range = range;
            FireRate = firerate;
            Cost = cost;
        }
    }

    class AoETowerStats : TowerStats
    {
        public int Damage { get; private set; }
        public float SpeedDebuff { get; private set; }
        public float SpeedDebuffTime { get; private set; }

        public AoETowerStats(float range, float firerate, int cost, int damage = 0, float speedDebuff = 0, float speedDebuffTime = 0) : base(range, firerate, cost)
        {
            Damage = damage;
            SpeedDebuff = speedDebuff;
            SpeedDebuffTime = speedDebuffTime;
        }
    }

    class ProjectileTowerStats : TowerStats
    {
        public int BasicDamage { get; private set; }
        public int PiercingDamage { get; private set; }
        public int PoisonDamage { get; private set; }
        public float PoisonDuration { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public int MultiHit { get; private set; }
        public float SplashRadius { get; private set; }


        /// <summary>
        /// Stats for all towers.
        /// </summary>
        /// <param name="range">Range in Screen Pixels</param>
        /// <param name="basicDamage">One time, basic damage. Deflected by armor.</param>
        /// <param name="piercingDamage">One time damage. Ignores armor.</param>
        /// <param name="poisonDamage">Ticking Damage. Ignores armor.</param>
        /// <param name="poisonDuration">Duration of Poision in seconds.</param>
        /// <param name="fireRate">Seconds between attacks</param>
        /// <param name="projectileSpeed">Speed of projectile in pixels per second.</param>
        /// <param name="multi">Number of targets the projectile can hit.</param>
        public ProjectileTowerStats(float range, float fireRate, int cost, float projectileSpeed,
            int basicDamage = 0, int piercingDamage = 0, int poisonDamage = 0, float poisonDuration = 0f,
            int multi = 1, float splash = 0f) : base(range, fireRate, cost)
        {
            BasicDamage = basicDamage;
            PiercingDamage = piercingDamage;
            PoisonDamage = poisonDamage;
            PoisonDuration = poisonDuration;
            ProjectileSpeed = projectileSpeed;
            MultiHit = multi;
            SplashRadius = splash;
        }
    }
}
