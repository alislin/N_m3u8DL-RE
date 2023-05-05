using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DLStart
{
    internal class DLTask
    {
        private string App;
        private string? tempPath;
        private string? TaskAfter;
        private bool SyncFlag;
        private List<(string link, string name, string path)> taskList = new List<(string link, string name, string path)>();
        private List<Task> tasks = new List<Task>();

        public bool Running { get; private set; }

        public DLTask(string app, string? tempPath = null, string? taskAfter = null, bool syncFlag = false)
        {
            App = app;
            if (!string.IsNullOrWhiteSpace(tempPath))
            {
                this.tempPath = $" --tmp-dir \"{tempPath}\"";

            }
            TaskAfter = taskAfter;
            SyncFlag = syncFlag;
        }

        public void Add(string link, string name, string path)
        {
            lock (taskList)
            {
                taskList.Add((link, name, path));
            }
        }

        public void Run()
        {
            if (Running)
            {
                return;
            }

            while (HasTask())
            {
                (string link, string name, string path) arg = ("[[END]]", "", "");
                lock (taskList)
                {
                    arg = taskList[0];
                }
                var para = $"{arg.link} --save-name \"{arg.name}\" --save-dir \"{arg.path}\" {tempPath}";
                Start(para);
                After(arg.name, arg.path);
                lock (taskList)
                {
                    taskList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 等待执行完成
        /// </summary>
        public void Wait()
        {
            Console.WriteLine("等待后续任务完成……");
            while (tasks.Any(x => !x.IsCompleted))
            {
                Task.Delay(1000).Wait();
            }
        }

        private bool HasTask()
        {
            var result = false;
            lock (taskList)
            {
                result = taskList.Any();
            }
            return result;
        }

        private void Start(string para)
        {
            // 启动app并使用arg作为参数
            var processInfo = new ProcessStartInfo
            {
                Arguments = para,
                FileName = App,
                //UseShellExecute = true,
            };
            var p = Process.Start(processInfo);
            p.WaitForExit();

        }

        private void After(string name, string path)
        {
            if (string.IsNullOrWhiteSpace(TaskAfter))
            {
                return;
            }
            var task = Task.Run(() =>
            {
                var file = $"{name}.mp4";
                var proc = TaskAfter.Replace("{targetName}", name).Replace("{targetFile}", file).Replace("{targetPath}", path);
                var proc_match = Regex.Match(proc, @"(.*?) (.*)");
                if (!proc_match.Success)
                {
                    return;
                }
                var processInfo = new ProcessStartInfo
                {
                    FileName = proc_match.Groups[1].Value,
                    Arguments = proc_match.Groups[2].Value,
                    UseShellExecute = true,
                };
                var p = Process.Start(processInfo);
                p.WaitForExit();
            });
            tasks.Add(task);
            if (SyncFlag)
            {
                task.Wait();
            }
        }
    }
}
