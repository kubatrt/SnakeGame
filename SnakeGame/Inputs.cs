using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    internal class Inputs
    {
        // mapa/słownik dostępnych klawiszy
        private static Hashtable keyTable = new Hashtable();

        // jeżeli jest klawisz, zwróć jego stan
        public static bool KeyPressed(Keys key)
        {
            if (keyTable[key] == null)
                return false;

            return (bool)keyTable[key];
        }

        // zmień stan klawisza, true - wciśnięty
        public static void ChangeState(Keys key, bool state)
        {
            keyTable[key] = state;
        }

    }
}
