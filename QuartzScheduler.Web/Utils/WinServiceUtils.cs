using DasMulli.Win32.ServiceUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Console;

namespace QuartzScheduler.Web.Utils
{
    public static class WinServiceUtils
    {
        public const string RunAsServiceFlag = "--run-as-service";
        public const string ServiceWorkingDirectoryFlag = "--working-directory";
        public const string RegisterServiceFlag = "--register-service";
        public const string PreserveWorkingDirectoryFlag = "--preserve-working-directory";
        public const string UnregisterServiceFlag = "--unregister-service";
        public const string InteractiveFlag = "--interactive";
        
        public const string ServiceName = "QuartzScheduler";
        public const string ServiceDisplayName = "QuartzScheduler";
        public const string ServiceDescription = "QuartzScheduler ASP.NET Core MVC Service running on .NET Core";


        public static void RunAsService(string[] args)
        {
            // easy fix to allow using default web host builder without changes
            var wdFlagIndex = Array.IndexOf(args, ServiceWorkingDirectoryFlag);
            if (wdFlagIndex >= 0 && wdFlagIndex < args.Length - 1)
            {
                var workingDirectory = args[wdFlagIndex + 1];
                Directory.SetCurrentDirectory(workingDirectory);
            }
            else
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            }
            var mvcService = new MvcWin32Service(args.Where(a => a != RunAsServiceFlag).ToArray());
            var serviceHost = new Win32ServiceHost(mvcService);
            serviceHost.Run();
        }

        public static void RunInteractive(string[] args)
        {
            var mvcService = new MvcWin32Service(args.Where(a => a != InteractiveFlag).ToArray());
            mvcService.Start(new string[0], () => { });
            WriteLine("Running interactively, press enter to stop.");
            Console.ReadLine();
            mvcService.Stop();
        }

        public static void RegisterService()
        {
            // Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
            var commandLineArgs = Environment.GetCommandLineArgs();

            var serviceArgs = commandLineArgs
                .Where(arg => arg != RegisterServiceFlag && arg != PreserveWorkingDirectoryFlag)
                .Select(EscapeCommandLineArgument)
                .Append(RunAsServiceFlag);

            var host = Process.GetCurrentProcess().MainModule.FileName;

            if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
            {
                // For self-contained apps, skip the dll path
                serviceArgs = serviceArgs.Skip(1);
            }

            if (commandLineArgs.Contains(PreserveWorkingDirectoryFlag))
            {
                serviceArgs = serviceArgs
                    .Append(ServiceWorkingDirectoryFlag)
                    .Append(EscapeCommandLineArgument(Directory.GetCurrentDirectory()));
            }

            var fullServiceCommand = host + " " + string.Join(" ", serviceArgs);

            // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
            // Note that when the service is already registered and running, it will be reconfigured but not restarted
            var serviceDefinition = new ServiceDefinitionBuilder(ServiceName)
                .WithDisplayName(ServiceDisplayName)
                .WithDescription(ServiceDescription)
                .WithBinaryPath(fullServiceCommand)
                .WithCredentials(Win32ServiceCredentials.LocalSystem)
                .WithAutoStart(true)
                .Build();

            new Win32ServiceManager().CreateOrUpdateService(serviceDefinition, startImmediately: true);

            WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }

        public static void UnregisterService()
        {
            new Win32ServiceManager()
                .DeleteService(ServiceName);

            WriteLine($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
        }

        public static void DisplayHelp()
        {
            WriteLine(ServiceDescription);
            WriteLine();
            WriteLine("This demo application is intened to be run as windows service. Use one of the following options:");
            WriteLine();
            WriteLine("  --register-service            Registers and starts this program as a windows service named \"" + ServiceDisplayName + "\"");
            WriteLine("                                All additional arguments will be passed to ASP.NET Core's WebHostBuilder.");
            WriteLine();
            WriteLine("  --preserve-working-directory  Saves the current working directory to the service configuration.");
            WriteLine("                                Set this wenn running via 'dotnet run' or when the application content");
            WriteLine("                                is not located nex to the application.");
            WriteLine();
            WriteLine("  --unregister-service          Removes the windows service creatd by --register-service.");
            WriteLine();
            WriteLine("  --interactive                 Runs the underlying asp.net core app. Useful to test arguments.");
        }


        public static string EscapeCommandLineArgument(string arg)
        {
            // http://stackoverflow.com/a/6040946/784387
            arg = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
            arg = "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
            return arg;
        }
    }
}
