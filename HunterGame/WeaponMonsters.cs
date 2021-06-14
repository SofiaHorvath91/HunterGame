using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HunterGame
{
    class WeaponMonsters
    {
        public string Weapon { get; set; }

        public List<string> PrimaryMonsters { get; set; }

        public List<string> SecondaryMonsters { get; set; }

        public WeaponMonsters(string weapon, List<string> primary, List<string> secondary)
        {
            Weapon = weapon;
            PrimaryMonsters = primary;
            SecondaryMonsters = secondary;
        }

        public override string ToString()
        {
            string primary = "";
            foreach (string monster in PrimaryMonsters)
            {
                primary += monster + "\n";

            }

            string secondary = "";
            foreach (string monster in SecondaryMonsters)
            {
                secondary += monster + "\n";

            }

            string result = Weapon + " : " + primary + " / " + secondary;

            return result;
        }
    }
}
