using System.Net;
using System.Net.Sockets;
using SuperSocket.Client;
using SuperSocket.ProtoBase;
using SupersocketCient;

var clients = new List<IEasyClient<StringPackageInfo, StringPackageInfo>>();

var sessionCount = 0;

while (true)
{
    Console.WriteLine("请输入连接数：");

    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        Console.WriteLine("输入错误");
        continue;
    }

    if (int.TryParse(input, out sessionCount))
        break;

    Console.WriteLine("输入错误");
}

var random = Random.Shared;
var ports = new[]
    { 5000, 5001, 5002, 5003, 5004, 5005, 5006, 5007, 5008, 5009, 6000, 6001, 6002, 6003, 6004, 6005 };

for (int i = 0; i < sessionCount; i++)
{
    var client =
        new EasyClient<StringPackageInfo, StringPackageInfo>(new CommandLinePipelineFilter
            {
                Decoder = new StringPackageDecoder()
            }, new StringPacketEncode())
            .AsClient();

    clients.Add(client);

    client.Closed += OnClosed;
    client.PackageHandler += OnPackageHandler;

    var server = new DnsEndPoint("81.71.162.222", ports[random.Next(0, ports.Length)],
        AddressFamily.InterNetwork);

    Console.WriteLine($"连接中:{server}....");

    var connected = await client.ConnectAsync(server);

    if (connected == false)
    {
        Console.WriteLine($"连接结果失败...");
        Console.ReadKey();
        return;
    }

    Console.WriteLine($"连接成功登陆中...");

    client.StartReceive();

    await client.SendAsync(new StringPackageInfo
    {
        Key = "login",
        Parameters = "wujun wujun520.".Split(' '),
    });

    while (true)
    {
        await Task.Delay(TimeSpan.FromSeconds(20));

        await client.SendAsync(new StringPackageInfo
        {
            Key = "heartBeat",
            Parameters = new[] { DateTime.Now.ToLongDateString() }
        });
    }
}


Console.ReadKey();

void OnClosed(object? sender, EventArgs args)
{
    Console.WriteLine("断开连接");
}

ValueTask OnPackageHandler(EasyClient<StringPackageInfo> sender, StringPackageInfo package)
{
    Console.WriteLine($"key:{package.Key} body:{string.Join(" ", package.Body)}");
    return ValueTask.CompletedTask;
}