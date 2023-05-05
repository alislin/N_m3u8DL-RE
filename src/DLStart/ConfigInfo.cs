using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLStart
{
    internal class ConfigInfo
    {
        /// <summary>
        /// 保存路径
        /// </summary>
        public string? SavePath { get; set; }
        /// <summary>
        /// 下载器路径
        /// </summary>
        public string? AppPath { get; set; }
        /// <summary>
        /// 临时文件夹
        /// </summary>
        public string? TempPath { get; set; }
        /// <summary>
        /// 完成后执行
        /// </summary>
        public string? AfterTaskRun { get; set; }
        /// <summary>
        /// 同步处理
        /// </summary>
        public bool? AfterTaskRunSync { get; set; } = false;
        /// <summary>
        /// 变量说明（只读）
        /// </summary>
        public string Description => "AfterTaskRun var: targetName {targetName}, targetPath {targetPath}, targetFile {targetFile}";
    }
}
