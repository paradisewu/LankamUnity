using System.Diagnostics;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class ShutdownCommand : ControllerCommand
{
    public override void Execute(IMessage message)
    {
        CloseComputer();
    }

    void CloseComputer()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "/c " + "shutdown -s -t 00";
        p.Start();
    }

}
