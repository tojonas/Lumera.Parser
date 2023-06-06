namespace Lumera.Parser.Processors
{
    public interface IBatchProcessor : IDisposable
    {
        public bool ProcessLine(string line);
    }
}
