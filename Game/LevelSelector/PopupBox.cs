using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RewindGame.Game.LevelSelector
{
    class PopupBox
    {
        public static void ShowEternalUnimplimentedBox()
        {
            MessageBox.Show("Currently there is not enough eternal levels for me to care to impliment this feature at the moment");
        }
        public static void NoConnectedLevel(string loc)
        {
            MessageBox.Show("There is no conencted level to the " + loc + " of this level");
        }
        public static string GetLevel()
        {
            MessageBox.Show("Please press ok on this box and then check the console (little purple rectangle on task bar)");
            Console.Clear();
            Console.WriteLine("Please type the level you wish to load eg(limbo15 cotton5 eterna2) WARNING! If you type the name of a level that does not exist the game WILL crash, I have no power over that");
            string userout = Console.ReadLine();
            Console.WriteLine("Great! teleporting you to: " + userout + " you may now click back on your game (remember do not close this window it will close the game)");
            return userout;
        }
    }
}
