using System.Diagnostics;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class LoadApplicationCommand : ControllerCommand
{
    public override void Execute(IMessage message)
    {
        string body = message.Body.ToString();
        string AppPath = Application.dataPath + "/../../Games/" + body + "\\" + body + ".exe";
        AppPath = AppPath.Replace('\\', '/');

        //UnityEngine.Debug.Log(AppPath);
        if (File.Exists(AppPath))
        {
            ApplicationLoad_OnLoad(AppPath);
        }
    }

    public void ApplicationLoad_OnLoad(string appName)
    {
        try
        {
            //启动外部程序
            //Process proc = Process.Start(appName);
            //if (proc.Start())
            //{
            //    bool istop = SetForegroundWindow(proc.MainWindowHandle);
            //    UnityEngine.Debug.Log(String.Format("设为最前端 {0} ", istop));
            //}
            Process proc = new Process();
            proc.StartInfo.FileName = appName;
            if (proc.Start())
            {
                bool istop = SetForegroundWindow(proc.MainWindowHandle);
                UnityEngine.Debug.Log(String.Format("设为最前端 {0} ", istop));
            }
            if (proc != null)
            {
                //监视进程退出
                proc.EnableRaisingEvents = true;
                //指定退出事件方法
                proc.Exited += (object sender, EventArgs e) =>
                {

                    UnityEngine.Debug.Log(String.Format("外部程序 {0} 已经退出！", appName));
                };
            }
        }
        catch (ArgumentException ex)
        {
            UnityEngine.Debug.Log(String.Format("外部程序 {0} ", ex.ToString()));
            //Console.WriteLine(String.Format("外部程序 不存在！", appName));
            //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public static void ApplicationLoad_OnExit(string appName)
    {
        try
        {
            //启动外部程序
            Process[] proc = Process.GetProcesses();//获取已开启的所有进程
            for (int i = 0; i < proc.Length; i++)
            {
                if (proc[i].ProcessName.ToString().ToLower() == appName)
                {
                    proc[i].Kill();
                }
            }
        }
        catch (ArgumentException ex)
        {
            //Console.WriteLine(String.Format("外部程序 不存在！", appName));
            //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    protected virtual void Proc_Exited(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log(String.Format("外部程序 {0} 已经退出！{1}", e.ToString(), sender));
    }
}
