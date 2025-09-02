using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServiceBlazorControlTest.Services
{
    public class ControllableBackgroundService : BaseBackgroundService
    {
        public ControllableBackgroundService(ILogger<ControllableBackgroundService> logger)
            : base(logger)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            EnqueueLog("后台服务开始执行");

            while (!stoppingToken.IsCancellationRequested)
            {
                // 模拟后台任务执行
                EnqueueLog("后台服务正在运行...");
                LastActivity = DateTime.Now;
                await Task.Delay(5000, stoppingToken); // 每5秒执行一次
            }

            EnqueueLog("后台服务执行已停止");
        }
    }
}
