using ILogger = Serilog.ILogger;

namespace Lastly
{
    public interface IPing
    {
        public string Ping();
    }

    public sealed class Ping : IPing, IDisposable
    {
        private readonly ILogger _logger;

        public Ping(ILogger logger)
        {
            logger.Information("---- ping constructor!");
            _logger = logger;
        }

        public void Dispose()
        {
            Thread.Sleep(3000);
            _logger.Information("---- ping dispose!");
        }

        public ValueTask DisposeAsync()
        {
            Thread.Sleep(1000);
            _logger.Information("---- ping dispose async!");
            return ValueTask.CompletedTask;
        }

        string IPing.Ping()
        {
            _logger.Information("ping");
            return "pong";
        }
    }
}