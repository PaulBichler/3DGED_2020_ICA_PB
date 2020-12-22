namespace GD_IntroToMonoGame
{
    /// <summary>
    ///
    /// </summary>
    /// <example>
    ///  Pair<int, string> p = new Pair<int, string>(1, "ssdf");
    /// </example>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="S"></typeparam>
    public class Pair<F, S>
    {
        private F first;
        private S second;

        public F First { get; set; }

        public Pair(F first, S second)
        {
            this.first = first;
            this.second = second;
        }
    }
}