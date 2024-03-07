using DLStart;
using System.Diagnostics;
using System.Formats.Tar;
using System.Text.Json;
using System.Text.RegularExpressions;

var targetName = "";
var configFile = "config.json";

// 检查config.json是否存在
var config = new ConfigInfo { SavePath = "Downloads", AppPath = "core", AfterTaskRunSync = false, AfterTaskRun = "" };
if (!File.Exists(configFile))
{
    Console.WriteLine("config.json不存在，创建默认配置文件");
    File.WriteAllText(configFile, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

}
// load file config.json ,convert to  ConfigInfo object

config = JsonSerializer.Deserialize<ConfigInfo>(File.ReadAllText(configFile));

var app = Path.Combine(config?.AppPath ?? "core", "N_m3u8DL-RE.exe");
// 检查app 是否存在

if (!File.Exists(app))
{
    Console.WriteLine($"找不到 {app} 文件，请检查配置文件");
    return;
}

ConsoleHelper.Print(new string[]
                        {
                            "1. 输入需要保存的路径(默认使用配置路径)",
                            "2. 输入需要保存的名称，该名称将作为文件夹名称",
                            "3. 按照提示输入m3u8链接地址（如果使用task.txt作为输入，直接回车）",
                            "-----------------------------",
                        }, 2
                    );

//// 等待输入，将输入的字串作为保存路径，如果为空使用默认路径
string savePath = ConsoleHelper.Read(new string[] { $"输入需要保存的路径(默认使用配置路径 [{config?.SavePath}]):" }, 1)?.Trim() ?? "";
if (string.IsNullOrEmpty(savePath))
{
    savePath = config?.SavePath ?? "Downloads";
}

var taskFile_select_Flag = false;
var taskFile_name = "";

targetName = ConsoleHelper.Read(new string[] { "输入需要保存的名称，该名称将作为文件夹名称（直接回车选择任务导入）:" }, 1,
    x =>
    {
        if (string.IsNullOrWhiteSpace(x))
        {
            // 如果targetName为空，使用 task 文件作为输入。选择task文件
            // 获取task文件列表
            var di = new DirectoryInfo(".");
            var taskFile_list = new List<FileInfo>(di.GetFiles("task_*.task"));
            if (taskFile_list.Count == 0)
            {
                return false;
            }
            ConsoleHelper.Print(new string[] { "选择任务文件（直接回车返回名称输入）：" }, 1);
            var tips = taskFile_list.Select(f => $"{taskFile_list.IndexOf(f) + 1}) {f.Name}").ToArray();
            var m = ConsoleHelper.Read(tips, 0, null, ConsoleColor.Green);
            if (string.IsNullOrWhiteSpace(m))
            {
                return false;
            }
            var task_index = ConsoleHelper.GetInt(m);
            if (task_index > 0 && task_index <= taskFile_list.Count)
            {
                taskFile_select_Flag = true;
                taskFile_name = taskFile_list[task_index - 1].FullName;
                return true;
            }
            return false;
        }
        return true;

    });

var path = "";

var session = "";
if (taskFile_select_Flag && !string.IsNullOrWhiteSpace(taskFile_name))
{
    var lines = new List<string>();
    // 使用task文件加载
    lines.AddRange(File.ReadAllLines(taskFile_name));
    ConsoleHelper.Print(new string[]
    {
        $"使用 [{taskFile_name}] 文件输入任务：",
    }, 1);
    ConsoleHelper.Print(lines.ToArray());

    targetName = lines[0].Trim();
    session = lines[1].Trim();


    lines.RemoveAt(0);
    lines.RemoveAt(0);

    path = Path.Combine(savePath, targetName);
    var task_sub = new DLTask(app, config?.TempPath, config?.AfterTaskRun, config?.AfterTaskRunSync);
    var task_index = 1;
    foreach (var item in lines)
    {
        var link_match = Regex.Match(item, @"(\d+),(\.+?),(.*)");
        task_index = ConsoleHelper.GetInt(link_match.Groups[1].Value);
        var item_link = link_match.Groups[3].Value.Trim();
        var targetFile = $"{targetName}.{session}e{task_index.ToString("00")}";
        targetFile = $"{targetName}.{session}e{task_index.ToString("00")}";
        if (!string.IsNullOrWhiteSpace(link_match.Groups[2].Value))
        {
            targetFile = link_match.Groups[3].Value.Trim();
        }
        if (!string.IsNullOrWhiteSpace(item_link))
        {
            //var arg = $"{item.Trim()} --save-name {targetFile} --save-dir {path}";
            task_sub.Add(item_link, targetFile, path);
        }
        task_index++;
    }

    task_sub.Run();
    task_sub.Wait();
    Console.WriteLine("任务执行完成");

    return;
}


session = ConsoleHelper.Read(new string[] { "输入第几季（直接回车跳过）:" }, 1, null, ConsoleColor.Yellow) ?? "1";
session = ConsoleHelper.GetInt(session, 1).ToString("00");

path = Path.Combine(savePath, targetName);

var index = 1;
var startIndex = index;
// when input is not empty string,set index add 1 and input string to filename,continue wait input
var link = "123";
var task = new DLTask(app, config?.TempPath, config?.AfterTaskRun, config?.AfterTaskRunSync);
var taskHistory = $"task_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{targetName}_{DateTime.Now.Microsecond}.task";
var taskFile_Flag = true;
while (!string.IsNullOrEmpty(link))
{
    var targetFile = $"{targetName}.{session}e{index.ToString("00")}";

    link = ConsoleHelper.Read(new string[] {
        taskFile_Flag?"如果使用task.txt作为输入，直接回车":"",
        $"输入选集 m3u8 链接，当前第 {index} 集（直接回车结束添加，输入数字改变集数）：",
    }, 1, null, ConsoleColor.Yellow)?.Trim() ?? "";

    // check link is number type
    if (int.TryParse(link, out var k))
    {
        if (k > 0)
        {
            index = k;
            continue;
        }
    }
    // 检查link是否符合链接格式
    if (string.IsNullOrEmpty(link))
    {
        if (taskFile_Flag)
        {
            var taskFile = "task.txt";
            // 检查taskFile是否存在
            if (File.Exists(taskFile))
            {
                // 加载task.txt文件，按照每行进行输入
                var lines = File.ReadAllLines(taskFile);
                foreach (var item in lines)
                {
                    targetFile = $"{targetName}.{session}e{index.ToString("00")}";
                    if (!string.IsNullOrWhiteSpace(item.Trim()))
                    {
                        //var arg = $"{item.Trim()} --save-name {targetFile} --save-dir {path}";
                        task.Add(item.Trim(), targetFile, path);
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
    //var para = $"{link} --save-name {targetFile} --save-dir {path}";
    task.Add(link, targetFile, path);
    if (taskFile_Flag)
    {
        File.AppendAllText(taskHistory, $"{targetName}\r\n");
        File.AppendAllText(taskHistory, $"{session}\r\n");
    }
    File.AppendAllText(taskHistory, $"{index},{link}\r\n");
    index++;
    taskFile_Flag = false;
}
task.Run();
task.Wait();
Console.WriteLine("任务执行完成");


