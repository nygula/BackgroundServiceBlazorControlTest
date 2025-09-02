using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServiceBlazorControlTest.Services
{
    public class ControllableBackgroundService : BaseBackgroundService
    {
        private static readonly List<string> _logStore = new List<string>();
        private static readonly object _logLock = new object();
        private const int MaxLogCount = 1000;

        public ControllableBackgroundService(ILogger<ControllableBackgroundService> logger)
            : base(logger)
        {
        }

        public IReadOnlyList<string> GetLogs()
        {
            lock (_logLock)
            {
                return new List<string>(_logStore);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RaiseLog("后台服务开始执行");

            while (!stoppingToken.IsCancellationRequested)
            {
                // 模拟后台任务执行
                RaiseLog("后台服务正在运行...");
                LastActivity = DateTime.Now;
                await Task.Delay(5000, stoppingToken); // 每5秒执行一次
            }

            RaiseLog("后台服务执行已停止");
        }

        protected override void RaiseLog(string message)
        {
            lock (_logLock)
            {
                _logStore.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
                if (_logStore.Count > MaxLogCount)
                    _logStore.RemoveRange(MaxLogCount, _logStore.Count - MaxLogCount);
            }
            RaiseLogToLoggerAndEvent(message);
        }
    }
}
