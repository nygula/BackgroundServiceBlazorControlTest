using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServiceBase
{
    public abstract class BaseBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private Task _executingTask;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private bool _isRunning;
        private static readonly ConcurrentQueue<string> _logStore = new ConcurrentQueue<string>();
        private const int MaxLogCount = 1000;

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
                EnqueueLog("服务已在运行中");
                return Task.CompletedTask;
            }

            if (_stoppingCts != null)
                _stoppingCts.Dispose();
            _stoppingCts = new CancellationTokenSource();

            _isRunning = true;
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            EnqueueLog("服务已启动");

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                EnqueueLog("服务已停止");
                return;
            }

            try
            {
                _stoppingCts.Cancel();
                EnqueueLog("正在停止服务...");
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite));
            }

            _isRunning = false;
            EnqueueLog("服务已停止");
        }

        public async Task RestartAsync()
        {
            EnqueueLog("正在重启服务...");
            await StopAsync();
            await StartAsync();
            EnqueueLog("服务已重启");
        }

        public IReadOnlyList<string> GetLogs()
        {
            return _logStore.ToArray().Reverse().ToList();
        }

        protected void EnqueueLog(string message)
        {
            var log = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _logStore.Enqueue(log);
            while (_logStore.Count > MaxLogCount)
            {
                _logStore.TryDequeue(out _);
            }
            _logger.LogInformation(message);
            OnLog?.Invoke(log);
        }

        protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
