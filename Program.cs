using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

static class Program
{
    public static void Main(string[] args)
    {
        int Pid = -1;
        int TaskID = 0;
        string Command = "";
        string Option = "";
        string Pcn = ".";

        List<string> comList = new List<string>();
        if (args == null)
        {
            Console.Write("引数がありません。\r\n");
            return;
        }
        if (args.Length == 0)
        {
            Console.Write("引数がありません。\r\n");
            return;
        }

        int i = 0;
        for (i = 0; i <= args.Length - 1; i++)
        {
            string s1 = args[i];
            if (s1.IndexOf("/") == 0)
            {
                i = i + 1;
                try
                {
                    string s2 = args[i];
                    if (s2.IndexOf("/") != 0)
                        comList.Add(s1 + " " + s2);
                    else
                        i = i - 1;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        for (i = 0; i <= comList.Count - 1; i++)
        {
            if (comList[i].ToLower().IndexOf("/p") == 0)
            {
                Pid = Convert.ToInt32(comList[i].Replace("/p ", ""));
                Console.Write("PID        (/p) : " + Convert.ToString(Pid) + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("/t") == 0)
            {
                TaskID = Convert.ToInt32(comList[i].Replace("/t ", ""));
                Console.Write("TaskID     (/t) : " + Convert.ToString(TaskID) + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("/c") == 0)
            {
                Command = comList[i].Replace("/c ", "");
                Console.Write("Command    (/c) : " + Command + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("/o") == 0)
            {
                Option = comList[i].Replace("/o ", "");
                Console.Write("Option     (/o) : " + Option + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("/n") == 0)
            {
                Pcn = comList[i].Replace("/n ", "");
                Console.Write("ServerName (/n) : " + Pcn + "\r\n");
                continue;
            }
        }

        switch (Command.ToLower())
        {
            case "list":
                {
                    Process[] ps = Process.GetProcessesByName("tstask");
                    foreach (var p in ps)
                    {
                        SharedMemory sm = new SharedMemory(Convert.ToUInt32(p.Id));
                        if (sm.TaskID > 0)
                            Console.Write("ID:" + p.Id.ToString("d08") + " TaskID:" + sm.TaskID.ToString("d02") + "\r\n");
                    }

                    break;
                }

            default:
                {
                    if (Pid == -1 & TaskID == 0)
                    {
                        Console.WriteLine("PIDもしくはTaskIDの指定がありません。");
                        break;
                    }
                    if (TaskID == 0)
                    {
                        SharedMemory sm = new SharedMemory(Convert.ToUInt32(Pid));
                        TaskID = Convert.ToInt32(sm.TaskID);
                    }
                    if (TaskID > 0)
                    {
                        TSTaskCommandSender rtcs = new TSTaskCommandSender(TaskID);
                        Console.Write(rtcs.Send(Command + "\r\n" + Option, Pcn));
                    }

                    break;
                }
        }
    }
}
