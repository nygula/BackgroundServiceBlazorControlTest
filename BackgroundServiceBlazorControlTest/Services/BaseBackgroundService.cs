using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServiceBlazorControlTest.Services
{
    public abstract class BaseBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private bool _isRunning;

        public bool IsRunning => _isRunning;
        public DateTime? LastActivity { get; protected set; }
        public string Status => IsRunning ? "运行中" : "已停止";
        public string StatusColor => IsRunning ? "green" : "red";

        public event Action<string>? OnLog;

        protected BaseBackgroundService(ILogger logger)
        {
            _logger = logger;
        }

        public Task StartAsync()
        {
            if (_isRunning)
            {
                RaiseLog("服务已在运行中");
                return Task.CompletedTask;
            }

            if (_stoppingCts != null)
                _stoppingCts.Dispose();
            _stoppingCts = new CancellationTokenSource();

            _isRunning = true;
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            RaiseLog("服务已启动");

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                RaiseLog("服务已停止");
                return;
            }

            try
            {
                _stoppingCts.Cancel();
                RaiseLog("正在停止服务...");
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite));
            }

            _isRunning = false;
            RaiseLog("服务已停止");
        }

        public async Task RestartAsync()
        {
            RaiseLog("正在重启服务...");
            await StopAsync();
            await StartAsync();
            RaiseLog("服务已重启");
        }

        protected void RaiseLog(string message)
        {
            _logger.LogInformation(message);
            OnLog?.Invoke(message);
        }

        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
