using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLStart
{
    internal class DLTask
    {
        private string App;
        private List<string> taskList = new List<string>();

        public bool Running { get; private set; }

        public DLTask(string app)
        {
            App = app;
        }

        public void Add(string para)
        {
            lock (taskList)
            {
                taskList.Add(para);
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
                string arg = "[[END]]";
                lock (taskList)
                {
                    arg = taskList[0];
                }
                Start(arg);
                lock (taskList)
                {
                    taskList.RemoveAt(0);
                }
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


    }
}
