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
        public int BasicDamage { get; private set; }
        public int PiercingDamage { get; private set; }
        public int PoisonDamage { get; private set; }
        public float PoisonDuration { get; private set; }
        public float FireRate { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public int MultiHit;
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
        public TowerStats(float range, float fireRate, float projectileSpeed, 
            int basicDamage = 0, int piercingDamage = 0, int poisonDamage = 0, float poisonDuration = 0f,
            int multi = 1, float splash = 0f)
        {
            Range = range;
            BasicDamage = basicDamage;
            PiercingDamage = piercingDamage;
            PoisonDamage = poisonDamage;
            PoisonDuration = poisonDuration;
            FireRate = fireRate;
            ProjectileSpeed = projectileSpeed;
            MultiHit = multi;
            SplashRadius = splash;
        }
    }
}
