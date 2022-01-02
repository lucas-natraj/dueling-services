using System.ServiceProcess;
using ILogger = Serilog.ILogger;

namespace Lastly
{
    public sealed class Monitor : IDisposable
    {
        private readonly string _serviceName;
        private readonly ILogger _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private string _status;
        private readonly Task _task;

        public Monitor(string serviceName, ILogger logger)
        {
            _serviceName = serviceName;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            _logger.Information("---- monitor constructor!");
            _task = Task.Run(() => Watch(token));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                _task.Wait(2000);
            }
            catch (Exception) 
            { 
                // swallow
            }
            _logger.Information("---- monitor dispose!");
        }

        private async Task Watch(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                ServiceController sc = new(_serviceName);
                var status = sc.Status.ToString();
                if (status != _status)
                {
                    _logger.Information("**** service status: {status}", status);
                    _status = status;
                }

                await Task.Delay(50, token);
            }
        }
    }
}