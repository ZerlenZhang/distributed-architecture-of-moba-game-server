using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ReadyGamerOne.Utility
{
#if UNITY_STANDALONE_WIN

    public static class WindowsUtil
    {
        #region Windows_MessageBox

        #region Private

        [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr handle, String message, String title, int type);

        #endregion

        public static int MessageBox(object obj, string title = "Message", int type = 0)
        {
            return MessageBox(IntPtr.Zero, obj.ToString(), title, type);
        }

        #endregion

        #region Filter

        /// <summary>
        /// 文件筛选工具
        /// 用于打开文件对话框，限制选择文件种类
        /// 使用实例：Filter.Build("PNG","Jpeg","GIF")
        /// </summary>
        public static string Filter(params string[] filters)
        {
            var str = "";
            foreach (var VARIABLE in filters)
            {
                str += VARIABLE + " files(*." + VARIABLE + ")\0*." + VARIABLE + ";\0";
            }

            return str;
        }

        #endregion

        #region 保存和打开文件对话框

        #region Private

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class FileDlg
        {
            public int structSize = 0;
            public IntPtr dlgOwner = IntPtr.Zero;
            public IntPtr instance = IntPtr.Zero;
            public String filter = null;
            public String customFilter = null;
            public int maxCustFilter = 0;
            public int filterIndex = 0;
            public String file = null;
            public int maxFile = 0;
            public String fileTitle = null;
            public int maxFileTitle = 0;
            public String initialDir = null;
            public String title = null;
            public int flags = 0;
            public short fileOffset = 0;
            public short fileExtension = 0;
            public String defExt = null;
            public IntPtr custData = IntPtr.Zero;
            public IntPtr hook = IntPtr.Zero;
            public String templateName = null;
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class OpenFileDlg : FileDlg
        {
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class SaveFileDlg : FileDlg
        {
        }

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName([In, Out] OpenFileDlg ofd);

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern bool GetSaveFileName([In, Out] SaveFileDlg ofd);

        #endregion

        /// <summary>
        /// 打开文件对话框
        /// </summary>
        /// <param name="filter">类型筛选</param>
        /// <param name="path">默认打开路径</param>
        /// <param name="title">对话框标题</param>
        /// <returns></returns>
        public static string OpenFileDialog(string filter, string path, string title = "打开")
        {
            OpenFileDlg pth = new OpenFileDlg();
            pth.structSize = Marshal.SizeOf(pth);
            pth.filter = filter;
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = path; // default path  
            pth.title = "打开项目";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if (GetOpenFileName(pth))
            {
                string filepath = pth.file; //选择的文件路径;  
                return filepath;
            }

            return "";
        }

        public static string OpenFileDialog(string filter)
        {
            return OpenFileDialog(filter, Application.persistentDataPath);
        }

        /// <summary>
        /// 保存文件对话框
        /// </summary>
        /// <param name="saveFilter">保存文件的后缀名，一定要是.png,.mp4之类</param>
        /// <param name="defaultPath">默认打开的路径</param>
        /// <returns></returns>
        public static string SaveFileDialog(string saveFilter, string defaultPath)
        {
            var pth = new SaveFileDlg();
            pth.structSize = Marshal.SizeOf(pth);
            pth.filter = saveFilter;
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = defaultPath; // default path  
            pth.title = "保存为";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

            if (GetSaveFileName(pth))
            {
                return pth.file + pth.filter;
            }

            return "";
        }

        public static string SaveFileDialog(string saveFilter)
        {
            return SaveFileDialog(saveFilter, Application.persistentDataPath);
        }

        #endregion

        #region 打开指定目录

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="absPath">文件夹的绝对路径,最后带不带'\'都行</param>
        public static void OpenFolderInExplorer(string absPath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                RunCmdNoErr("explorer.exe", absPath);
            else if (Application.platform == RuntimePlatform.OSXEditor)
                RunCmdNoErr("open", absPath.Replace("\\", "/"));
        }

        #endregion

        #region 命令行
        
        /// <summary>
        /// 开线程执行命令行
        /// </summary>
        /// <param name="cmd">可执行文件的本地路径或全路径，带不带.exe都可，本地路径的话，workingPath一定要传参数</param>
        /// <param name="args">命令行参数</param>
        /// <param name="workingDir">工作目录</param>
        /// <param name="input">用户输入</param>
        /// <param name="endCall">结束命令行时的回调</param>
        public static void RunCmd(string cmd, string args, string workingDir = "", string[] input = null,
            Action endCall = null)
        {
            if (!Path.IsPathRooted(cmd))
                cmd = workingDir + cmd;
            if (!Path.IsPathRooted(cmd))
                Debug.LogWarning("cmd路径不是跟路径，可能出错");

            Debug.Log("FinalCmd:  " + cmd);

            WindowsUtil.command = cmd;
            WindowsUtil.args = args;
            WindowsUtil.workPath = workingDir;
            WindowsUtil.input = input;
            WindowsUtil.endCall = endCall;
            new Thread(RunCmdThread).Start();
        }


        /// <summary>
        /// 运行命令,不返回stderr版本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>命令的stdout输出</returns>
        public static string RunCmdNoErr(string cmd, string args, string workingDri = "")
        {
            var p = CreateCmdProcess(cmd, args, workingDri);
            var res = p.StandardOutput.ReadToEnd();
            p.Close();
            return res;
        }

        /// <summary>
        /// 运行命令,不返回stderr版本
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="input">StandardInput</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>命令的stdout输出</returns>
        public static string RunCmdNoErr(string cmd, string args, string[] input, string workingDri = "")
        {
            var p = CreateCmdProcess(cmd, args, workingDri);
            if (input != null && input.Length > 0)
            {
                for (int i = 0; i < input.Length; i++)
                    p.StandardInput.WriteLine(input[i]);
            }

            var res = p.StandardOutput.ReadToEnd();
            p.Close();
            return res;
        }

        #region Private

        /// <summary>
        /// 构建Process对象，并执行
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <param name="workingDri">工作目录</param>
        /// <returns>Process对象</returns>
        private static Process CreateCmdProcess(string cmd, string args, string workingDir = "")
        {
            var en = System.Text.UTF8Encoding.UTF8;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                en = System.Text.Encoding.GetEncoding("gb2312");

            var pStartInfo = new ProcessStartInfo(cmd);
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = true;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.StandardErrorEncoding = en;
            pStartInfo.StandardOutputEncoding = en;

            if (!string.IsNullOrEmpty(workingDir))
                pStartInfo.WorkingDirectory = workingDir;

            pStartInfo.Arguments = args;
            return Process.Start(pStartInfo);
        }

        /// <summary>
        /// 运行命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="args">命令的参数</param>
        /// <returns>string[] res[0]命令的stdout输出, res[1]命令的stderr输出</returns>
        private static void RunCmdThread()
        {
            var p = CreateCmdProcess(command, args, workPath);
            if (input != null && input.Length > 0)
            {
                for (int i = 0; i < input.Length; i++)
                    p.StandardInput.WriteLine(input[i]);
            }

            var output = p.StandardOutput.ReadToEnd();
            p.Close();
            Debug.Log("命令执行结束，输出为：\n" + output);
            endCall?.Invoke();
        }




        private static string command;
        private static string args;
        private static string workPath;
        private static string[] input;
        private static Action endCall;        

        #endregion


        #endregion
    }

#endif
}