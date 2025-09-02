# BackgroundServiceBase

ͨ�ÿɿغ�̨������࣬������ Blazor��ASP.NET Core �� .NET ��Ŀ��

## ��װ

ͨ�� NuGet ��װ�������󣩣�
```
dotnet add package BackgroundServiceBase
```

## �÷�ʾ��

1. �̳� `BaseBackgroundService`��ʵ����ĺ�̨�����߼���

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
        EnqueueLog("����ʼִ��");
        while (!stoppingToken.IsCancellationRequested)
        {
            // ��������߼�
            EnqueueLog("������������...");
            await Task.Delay(1000, stoppingToken);
        }
        EnqueueLog("������ֹͣ");
    }
}
```

2. �� DI ������ע����ķ����� Blazor/ASP.NET Core����

```csharp
builder.Services.AddSingleton<MyBackgroundService>();
```

3. ��ҳ��������ע�벢���Ʒ���

```csharp
@inject MyBackgroundService BackgroundService

<button @onclick="BackgroundService.StartAsync">����</button>
<button @onclick="BackgroundService.StopAsync">ֹͣ</button>
<button @onclick="BackgroundService.RestartAsync">����</button>

@foreach (var log in BackgroundService.GetLogs())
{
    <div>@log</div>
}
```

## ����
- ֧�ַ���������ֹͣ������
- ��־�Զ��洢�����1000������������ҳ��չʾ
- �̰߳�ȫ���ʺϸ߲�������
- �¼��ַ���֧����־ʵʱ����

## License
MIT

## �ֿ�
https://github.com/nygula/BackgroundServiceBlazorControlTest
