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
        public string Status => IsRunning ? "������" : "��ֹͣ";
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
                RaiseLog("��������������");
                return Task.CompletedTask;
            }

            if (_stoppingCts != null)
                _stoppingCts.Dispose();
            _stoppingCts = new CancellationTokenSource();

            _isRunning = true;
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            RaiseLog("����������");

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                RaiseLog("������ֹͣ");
                return;
            }

            try
            {
                _stoppingCts.Cancel();
                RaiseLog("����ֹͣ����...");
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite));
            }

            _isRunning = false;
            RaiseLog("������ֹͣ");
        }

        public async Task RestartAsync()
        {
            RaiseLog("������������...");
            await StopAsync();
            await StartAsync();
            RaiseLog("����������");
        }

        protected void RaiseLog(string message)
        {
            _logger.LogInformation(message);
            OnLog?.Invoke(message);
        }

        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
