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
        string TSTaskName = "TSTask.exe";
        string ServerName = ".";
        bool RecTask = false;
        bool Verbose = false;
        bool Version = false;

        string[] ArgList = System.Environment.GetCommandLineArgs();
        string VersionInfo =
            System.Diagnostics.FileVersionInfo.GetVersionInfo(
            System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion;

        List<string> comList = new List<string>();
        if (ArgList == null || ArgList.Length <= 1)
        {
            Console.WriteLine("Error: 引数がありません。 ");
            Console.WriteLine("オプション： ");
            Console.WriteLine("PID        (-p) : 接続する TSTask の PID (プロセス ID) ");
            Console.WriteLine("TaskID     (-t) : 接続する TSTask の TaskID (タスク ID) ");
            Console.WriteLine("TSTaskName (-m) : 接続する TSTask のファイル名 (初期値: TSTask.exe)");
            Console.WriteLine("ServerName (-n) : 接続する TSTask が稼働している PC のホスト名 (初期値: ローカル PC)");
            Console.WriteLine("Command    (-c) : TSTask に送信するコマンド (list を指定で TSTask の ID を表示) ");
            Console.WriteLine("Option     (-o) : TSTask に送信するコマンドのオプションプロパティ (複数指定する場合は | で区切る) ");
            Console.WriteLine("RecTask    (-r) : TSTask の代わりに RecTask に接続する");
            Console.WriteLine("Details    (-d) : 受け取ったコマンドの詳細を表示する");
            Console.WriteLine("Version    (-v) : バージョンを表示する");
            return;
        }

        int i = 0;
        for (i = 0; i <= ArgList.Length - 1; i++)
        {
            string s1 = ArgList[i];
            if (s1.IndexOf("-") == 0)
            {
                i = i + 1;
                if (s1 == "-r")
                {
                    RecTask = true;
                    TSTaskName = "RecTask.exe";
                    comList.Add(s1);
                    i = i - 1;
                }
                else if (s1 == "-d")
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
                if (Verbose) Console.WriteLine("PID        (-p) : " + Convert.ToString(PID) + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-t") == 0)
            {
                TaskID = Convert.ToInt32(comList[i].Replace("-t ", ""));
                if (Verbose) Console.WriteLine("TaskID     (-t) : " + Convert.ToString(TaskID) + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-m") == 0)
            {
                TSTaskName = comList[i].Replace("-m ", "");
                if (Verbose) Console.WriteLine("TSTaskName (-m) : " + TSTaskName + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-n") == 0)
            {
                ServerName = comList[i].Replace("-n ", "");
                if (Verbose) Console.WriteLine("ServerName (-n) : " + ServerName + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-c") == 0)
            {
                Command = comList[i].Replace("-c ", "");
                if (Verbose) Console.WriteLine("Command    (-c) : " + Command + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-r") == 0)
            {
                if (Verbose) Console.WriteLine("RecTask    (-r) : " + RecTask + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-o") == 0)
            {
                Option = comList[i].Replace("-o ", "")
                                    // プロパティ内のダブルクォートをシングルクオートでも指定できるように
                                    // なぜか PowerShell だと正しく入れ子のダブルクオートを渡せないため
                                   .Replace("'", "\"")
                                    // | を改行コードに変換
                                   .Replace("|", "\r\n");
                if (Verbose) Console.WriteLine("Option     (-o) : " + Option + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-d") == 0)
            {
                Console.WriteLine("Details    (-d) : " + Verbose + "");
                continue;
            }
            if (comList[i].ToLower().IndexOf("-v") == 0)
            {
                if (Verbose) Console.WriteLine("Version    (-v) : " + Version + "");
                continue;
            }
        }

        if (Version) {
            Console.WriteLine("TSTaskCentreEx version " + VersionInfo);
            return;
        }

        switch (Command.ToLower())
        {
            case "list":
                {
                    Process[] ps = Process.GetProcessesByName(TSTaskName.Replace(".exe", ""));
                    foreach (var p in ps)
                    {
                        SharedMemory sm = new SharedMemory(Convert.ToUInt32(p.Id), RecTask);
                        if (sm.TaskID > 0)
                            Console.WriteLine("PID:" + p.Id.ToString("d08") + " TaskID:" + sm.TaskID.ToString("d02") + "");
                    }

                    break;
                }

            default:
                {
                    if (PID == -1 & TaskID == 0)
                    {
                        Console.WriteLine("Error: PID もしくは TaskID が指定されていません。");
                        Console.WriteLine("PID        (-p) : 接続する TSTask の PID (プロセス ID) ");
                        Console.WriteLine("TaskID     (-t) : 接続する TSTask の TaskID (タスク ID) ");
                        break;
                    }
                    if (Command == "")
                    {
                        Console.WriteLine("Error: コマンドが指定されていません。");
                        Console.WriteLine("Command    (-c) : TSTask に送信するコマンド (list を指定で TSTask の ID を表示) ");
                        break;
                    }
                    if (TaskID == 0)
                    {
                        SharedMemory sm = new SharedMemory(Convert.ToUInt32(PID), RecTask);
                        TaskID = Convert.ToInt32(sm.TaskID);
                    }
                    if (TaskID > 0)
                    {
                        TSTaskCommandSender rtcs = new TSTaskCommandSender(TaskID, RecTask);
                        Console.WriteLine(rtcs.Send(Command + "\r\n" + Option, ServerName));
                    }

                    break;
                }
        }
    }
}
