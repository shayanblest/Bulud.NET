namespace Bulud.Base
{
    public class ListResult<T> where T : class
    {
        public List<T>? Elements { get; set; }
        public int Count { get; set; }
    }
}
