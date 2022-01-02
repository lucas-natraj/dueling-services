using ILogger = Serilog.ILogger;

namespace Lastly
{
    public class Hosted : BackgroundService
    {
        private readonly ILogger _logger;

        public Hosted(ILogger logger)
        {
            _logger = logger;
            _logger.Information("---- hosted constructor!");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("---- hosted starting!");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.Information("hosted canceled!");
                }
                catch (Exception e)
                {
                    _logger.Error(e, "hosted error!");
                }
            }

            _logger.Information("---- hosted done!");
        }
    }
}