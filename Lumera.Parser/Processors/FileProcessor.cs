using System.Diagnostics;

namespace Lumera.Parser.Processors
{
    public class FileProcessor
    {
        private readonly Dictionary<string, IBatchProcessor> _processors = new();
        public void Register(string filePattern, IBatchProcessor processor)
        {
            if (_processors.ContainsKey(filePattern))
            {
                throw new ArgumentException($"File pattern {filePattern} already has a registered handler");
            }
            _processors[filePattern] = processor;
        }
        public bool ProcessFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(nameof(path));
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            using (var processor = GetProcessorForFile(path))
            {
                if (processor != null)
                {
                    return File.ReadAllLines(path).FirstOrDefault(line => !processor.ProcessLine(line)) is null;
                }
            }
            return false;
        }

        private IBatchProcessor? GetProcessorForFile(string path)
        {
            return _processors.FirstOrDefault(t => path.EndsWith(t.Key)).Value;
        }
    }
}