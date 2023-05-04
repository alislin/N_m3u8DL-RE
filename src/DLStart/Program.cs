using DLStart;
using System.Diagnostics;
using System.Text.Json;

var targetName = "";
var configFile = "config.json";

// 检查config.json是否存在
var config = new ConfigInfo { SavePath = "Downloads", AppPath = "core" };
if (!File.Exists(configFile))
{
    Console.WriteLine("config.json不存在，创建默认配置文件");
    File.WriteAllText(configFile, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

}
// load file config.json ,convert to  ConfigInfo object

config = JsonSerializer.Deserialize<ConfigInfo>(File.ReadAllText(configFile));

var app = Path.Combine(config.AppPath ?? "core", "N_m3u8DL-RE.exe");
// 检查app 是否存在

if (!File.Exists(app))
{
    Console.WriteLine($"找不到 {app} 文件，请检查配置文件");
    return;
}


Console.WriteLine("");
Console.WriteLine("");
Console.WriteLine("1. 输入需要保存的路径(默认使用配置路径)");
Console.WriteLine("2. 输入需要保存的名称，该名称将作为文件夹名称");
Console.WriteLine("3. 按照提示输入m3u8链接地址（如果使用task.txt作为输入，直接回车）");

Console.WriteLine("-----------------------------");
Console.WriteLine("");
Console.WriteLine($"输入需要保存的路径(默认使用配置路径 [{config.SavePath}]):");
// 等待输入，将输入的字串作为保存路径，如果为空使用默认路径
string savePath = Console.ReadLine();
if (string.IsNullOrEmpty(savePath))
{
    savePath = config.SavePath;
}

while (string.IsNullOrEmpty(targetName))
{
    Console.WriteLine("");
    Console.WriteLine("输入需要保存的名称，该名称将作为文件夹名称:");
    // 等待输入，将输入的字串作为targetName,如果没有则提示再次输入
    targetName = Console.ReadLine();
}
var path = Path.Combine(savePath, targetName);

var index = 1;
// when input is not empty string,set index add 1 and input string to filename,continue wait input
var link = "123";
var task = new DLTask(app);
var taskHistory = $"task_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{DateTime.Now.Microsecond}.txt";
while (!string.IsNullOrEmpty(link))
{
    Console.WriteLine("");
    if (index == 1)
    {
        Console.WriteLine("如果使用task.txt作为输入，直接回车");
    }
    Console.WriteLine($"输入选集 m3u8 链接，当前第 {index} 集（直接回车结束添加）：");
    link = Console.ReadLine()?.Trim() ?? "";
    // 检查link是否符合链接格式
    if (string.IsNullOrEmpty(link))
    {
        if (index == 1)
        {
            var taskFile = "task.txt";
            // 检查taskFile是否存在
            if (File.Exists(taskFile))
            {
                // 加载task.txt文件，按照每行进行输入
                var lines = File.ReadAllLines(taskFile);
                foreach (var item in lines)
                {
                    if (!string.IsNullOrWhiteSpace(item.Trim()))
                    {
                        var arg = $"{item.Trim()} --save-name {targetName}_{index.ToString("00")} --save-dir {path}";
                        task.Add(arg);
                    }
                    index++;
                }
            }
            else
            {
                System.Console.WriteLine($"{taskFile} 不存在");
            }
        }
        break;
    }
    var para = $"{link} --save-name {targetName}_{index.ToString("00")} --save-dir {path}";
    task.Add(para);
    File.AppendAllText(taskHistory, $"{link}\r\n");
    index++;
}
task.Run();
Console.WriteLine("任务执行完成");


