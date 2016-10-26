using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonTD
{
    class Localization
    {
        public Localization()
        {
            LoadLocalization("en-US");
        }
        public Localization(string language)
        {
            LoadLocalization(language);
        }

        public void LoadLocalization(string filename)
        {

        }

        public string Get(string key)
        {
            //return local if it's in the dictionary, or else return the key that was sent.
            if( dictionary.ContainsKey(key))
                return dictionary[key];
            return key;
        }

        Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            {"range", "Range" },
            { "lightningDamage", "Lightning Damage"},
            {"slowDebuff", "Speed Reduction" },

            {"Tower.Basic", "Standard Dragon"},
            {"Tower.Basic.Desc", "Your everyday, run-of-the-mill, bog standard, red dragon."},

            {"Tower.Poison", "Poison Dragon"},
            {"Tower.Poison.Desc", "Does a ticking poison damage."},

            {"Tower.Piercing", "Piercing Dragon"},
            {"Tower.Piercing.Desc", "This dragon's fireballs pierce through enemies."},

            {"Tower.Sniper", "Sniper Dragon"},
            {"Tower.Sniper.Desc", "\"Watch me do a 360 no-scope.\""},

            {"Tower.Explosive", "Explosive Dragon"},
            {"Tower.Explosive.Desc", "Fires an explosive fireball!"},

            {"Tower.Freeze", "Frost Dragon"},
            {"Tower.Freeze.Desc", "Cool as ice. Makes movement difficult for enemies."},

            {"Tower.Lightning", "Lightning Dragon"},
            {"Tower.Lightning.Desc", "This dragon has harnessed the power of lightning! Very frightening!"},

        };
    }
}
