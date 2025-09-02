# BackgroundServiceBase

通用可控后台服务基类，适用于 Blazor、ASP.NET Core 等 .NET 项目。

## 安装

通过 NuGet 安装（发布后）：
```
dotnet add package BackgroundServiceBase
```

## 用法示例

1. 继承 `BaseBackgroundService`，实现你的后台任务逻辑：

```csharp
using BackgroundServiceBase;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

public class MyBackgroundService : BaseBackgroundService
{
    public MyBackgroundService(ILogger<MyBackgroundService> logger) : base(logger) { }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        EnqueueLog("服务开始执行");
        while (!stoppingToken.IsCancellationRequested)
        {
            // 你的任务逻辑
            EnqueueLog("服务正在运行...");
            await Task.Delay(1000, stoppingToken);
        }
        EnqueueLog("服务已停止");
    }
}
```

2. 在 DI 容器中注册你的服务（如 Blazor/ASP.NET Core）：

```csharp
builder.Services.AddSingleton<MyBackgroundService>();
```

3. 在页面或组件中注入并控制服务：

```csharp
@inject MyBackgroundService BackgroundService

<button @onclick="BackgroundService.StartAsync">启动</button>
<button @onclick="BackgroundService.StopAsync">停止</button>
<button @onclick="BackgroundService.RestartAsync">重启</button>

@foreach (var log in BackgroundService.GetLogs())
{
    <div>@log</div>
}
```

## 特性
- 支持服务启动、停止、重启
- 日志自动存储（最多1000条），可用于页面展示
- 线程安全，适合高并发场景
- 事件分发，支持日志实时推送

## License
MIT

## 仓库
https://github.com/nygula/BackgroundServiceBlazorControlTest
