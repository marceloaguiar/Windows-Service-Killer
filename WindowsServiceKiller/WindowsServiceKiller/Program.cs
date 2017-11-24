using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceKiller
{
    class Program
    {
        private enum ExitCode
        {
            Success = 0,
            InvalidUsage = -1
        }

        static int Main(string[] args)
        {
            var processName = "";
            int timeOut = 60;

            try
            {
                if (args.Length == 0)
                {
                    Console.Write("Invalid Arguments");
                    return (int)ExitCode.InvalidUsage;
                }
                processName = args[0];
                if (args.Length > 1)
                {                    
                    timeOut = Convert.ToInt16(args[1]);
                }

                if (timeOut > 600)
                    timeOut = 600;

                StopAndKillProcess(processName, timeOut);
                return (int)ExitCode.Success;
            }
            catch (Exception ex)
            {
                Console.Write($"Fail : {ex.Message}");
                return (int)ExitCode.InvalidUsage;
            }
        }

        private static void StopAndKillProcess(string processName,int timeOutToKill)
        {
            ServiceController sc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Equals(processName));

            if (sc != null)
            {
                if (!sc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    sc.Stop();
                }

                Process[] procs = Process.GetProcessesByName(processName);
                int count = 0;
                while (( procs.Length != 0) || count >= timeOutToKill)
                {
                    procs = Process.GetProcessesByName(processName);
                    System.Threading.Thread.Sleep(1000);
                    count++;
                }

                if (procs.Length > 0)
                {
                    foreach (Process proc in procs)
                    {
                        proc.Kill();
                    }
                }
                System.Threading.Thread.Sleep(5000);
            }
            else
            {
                Console.Write("Process stoped or not found!");
            }
        }
    }
}
