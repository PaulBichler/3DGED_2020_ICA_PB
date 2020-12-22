namespace GDLibrary.Constants
{
    /// <summary>
    /// Provides lerp speeds for camera movement
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed"/>
    public sealed class LerpSpeed
    {
        private static readonly float SpeedMultiplier = 2;
        public static readonly float VerySlow = 0.05f;
        public static readonly float Slow = SpeedMultiplier * VerySlow;
        public static readonly float Medium = SpeedMultiplier * Slow;
        public static readonly float Fast = SpeedMultiplier * Medium;
        public static readonly float VeryFast = SpeedMultiplier * Fast;
    }

    /// <summary>
    /// Constains the constants used by the game engine
    /// </summary>
    public class LibraryConstants
    {
        //add core game engine constants here...
    }
}