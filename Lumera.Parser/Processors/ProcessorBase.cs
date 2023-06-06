using System.Diagnostics.CodeAnalysis;

namespace Lumera.Parser.Processors
{
    public abstract class ProcessorBase : IBatchProcessor
    {
        private bool _errors = false;
        public bool ProcessLine(string line)
        {
            try
            {
                OnProcessLine(line);
            }
            catch (Exception)
            {
                _errors = true;
                throw;
            }
            return !_errors;
        }
        public void Dispose()
        {
            if (!_errors)
            {
                OnDisposed();
            }
            _errors = false;
        }

        protected void EnsureNotNull([NotNull] object? value, string message)
        {
            if (value == null)
            {
                throw new InvalidDataException(message);
            }
        }
        protected void EnsureNull(object? value, string message)
        {
            if (value != null)
            {
                throw new InvalidDataException(message);
            }
        }

        protected abstract void OnProcessLine(string line);
        protected abstract void OnDisposed();
    }
}
