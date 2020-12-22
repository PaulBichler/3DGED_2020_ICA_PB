using System;

namespace GDGame
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new Main())
                game.Run();
        }
    }
}