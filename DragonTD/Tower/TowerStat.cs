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

        /// <summary>
        /// Stats for all towers.
        /// </summary>
        /// <param name="range">Range in Screen Pixels</param>
        /// <param name="basicDamage">One time, basic damage. Deflected by armor.</param>
        /// <param name="piercingDamage">One time damage. Ignores armor.</param>
        /// <param name="poisonDamage">Ticking Damage. Ignores armor.</param>
        /// <param name="poisonDuration">Duration of Poision in seconds.</param>
        public TowerStats(float range, int basicDamage = 0, int piercingDamage = 0, int poisonDamage = 0, float poisonDuration = 0f)
        {
            Range = range;
            BasicDamage = basicDamage;
            PiercingDamage = piercingDamage;
            PoisonDamage = poisonDamage;
            PoisonDuration = poisonDuration;
        }
    }
}
