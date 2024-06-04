namespace NetworkService.Models
{
    /// <summary>
    /// This class represents a mutable version of a 2-element Tuple class
    /// </summary>
    /// <typeparam name="TFirst"></typeparam>
    /// <typeparam name="TSecond"></typeparam>
    public class Pair<TFirst, TSecond>
    {
        public TFirst Item1 { get; set; }
        public TSecond Item2 { get; set; }

        public Pair()
        {

        }
        public Pair(TFirst first, TSecond second)
        {
            Item1 = first;
            Item2 = second;
        }
    }
}
