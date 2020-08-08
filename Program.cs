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
    public static void Main()
    {
        int PID = -1;
        int TaskID = 0;
        string Command = "";
        string Option = "";
        string ServerName = ".";
        bool Verbose = false;
        bool Version = false;

        string[] ArgList = System.Environment.GetCommandLineArgs();
        string VersionInfo =
            System.Diagnostics.FileVersionInfo.GetVersionInfo(
            System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion;

        List<string> comList = new List<string>();
        if (ArgList == null || ArgList.Length <= 1)
        {
            Console.Write("Error: 引数がありません。 \r\n");
            Console.Write("オプション： \r\n");
            Console.Write("PID        (-p) : 接続する TSTask の PID (プロセス ID) \r\n");
            Console.Write("TaskID     (-t) : 接続する TSTask の TaskID (タスク ID) \r\n");
            Console.Write("ServerName (-n) : 接続する TSTask が稼働している PC のホスト名 \r\n");
            Console.Write("Command    (-c) : TSTask に送信するコマンド (list を指定で TSTask の ID を表示) \r\n");
            Console.Write("Option     (-o) : TSTask に送信するコマンドのオプションプロパティ (\\n で改行できる) \r\n");
            Console.Write("Details    (-d) : 受け取ったコマンドの詳細を表示する\r\n");
            Console.Write("Version    (-v) : バージョンを表示する\r\n");
            return;
        }

        int i = 0;
        for (i = 0; i <= ArgList.Length - 1; i++)
        {
            string s1 = ArgList[i];
            if (s1.IndexOf("-") == 0)
            {
                i = i + 1;
                if (s1 == "-d")
                {
                    Verbose = true;
                    comList.Add(s1);
                    i = i - 1;
                }
                else if (s1 == "-v")
                {
                    Version = true;
                    comList.Add(s1);
                    i = i - 1;
                }
                else {
                    try
                    {
                        string s2 = ArgList[i];
                        if (s2.IndexOf("-") != 0)
                        {
                            comList.Add(s1 + " " + s2);
                        }
                        else
                        {
                            i = i - 1;
                        }
                    }
                    catch (Exception)
                    {
                        // 握りつぶす
                        break;
                    }
                }
            }
        }

        for (i = 0; i <= comList.Count - 1; i++)
        {
            if (comList[i].ToLower().IndexOf("-p") == 0)
            {
                PID = Convert.ToInt32(comList[i].Replace("-p ", ""));
                if (Verbose) Console.Write("PID        (-p) : " + Convert.ToString(PID) + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-t") == 0)
            {
                TaskID = Convert.ToInt32(comList[i].Replace("-t ", ""));
                if (Verbose) Console.Write("TaskID     (-t) : " + Convert.ToString(TaskID) + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-n") == 0)
            {
                ServerName = comList[i].Replace("-n ", "");
                if (Verbose) Console.Write("ServerName (-n) : " + ServerName + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-c") == 0)
            {
                Command = comList[i].Replace("-c ", "");
                if (Verbose) Console.Write("Command    (-c) : " + Command + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-o") == 0)
            {
                Option = comList[i].Replace("-o ", "")
                                    // プロパティ内のダブルクォートをシングルクオートでも指定できるように
                                    // なぜか PowerShell だと正しく入れ子のダブルクオートを渡せないため
                                   .Replace("'", "\"")
                                    // 改行コードを反映
                                   .Replace("\\r\\n", "\r\n")
                                   .Replace("\\n", "\r\n");
                if (Verbose) Console.Write("Option     (-o) : " + Option + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-d") == 0)
            {
                Console.Write("Details    (-d) : " + Verbose + "\r\n");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-v") == 0)
            {
                if (Verbose) Console.Write("Version    (-v) : " + Version + "\r\n");
                continue;
            }
        }

        if (Version) {
            Console.Write("TSTaskCentreEx version " + VersionInfo);
            return;
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
                            Console.Write("PID: " + p.Id.ToString("d08") + " TaskID: " + sm.TaskID.ToString("d02") + "\r\n");
                    }

                    break;
                }

            default:
                {
                    if (PID == -1 & TaskID == 0)
                    {
                        Console.WriteLine("Error: PID もしくは TaskID が指定されていません。");
                        Console.Write("PID        (-p) : 接続する TSTask の PID (プロセス ID) \r\n");
                        Console.Write("TaskID     (-t) : 接続する TSTask の TaskID (タスク ID) \r\n");
                        break;
                    }
                    if (Command == "")
                    {
                        Console.WriteLine("Error: コマンドが指定されていません。");
                        Console.Write("Command    (-c) : TSTask に送信するコマンド (list を指定で TSTask の ID を表示) \r\n");
                        break;
                    }
                    if (TaskID == 0)
                    {
                        SharedMemory sm = new SharedMemory(Convert.ToUInt32(PID));
                        TaskID = Convert.ToInt32(sm.TaskID);
                    }
                    if (TaskID > 0)
                    {
                        TSTaskCommandSender rtcs = new TSTaskCommandSender(TaskID);
                        Console.Write(rtcs.Send(Command + "\r\n" + Option, ServerName));
                    }

                    break;
                }
        }
    }
}
