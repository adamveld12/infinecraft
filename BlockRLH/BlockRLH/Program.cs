using System;

namespace BlockRLH
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BlockRLHGame game = new BlockRLHGame())
            {
                game.Run();
            }
        }
    }
#endif
}

