using System;

namespace RewindGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new RewindGame())
                game.Run();
        }
    }
}
