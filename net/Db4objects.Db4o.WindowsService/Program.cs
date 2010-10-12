using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Collections;
using System.Reflection;
using System.Security.Principal;

namespace Db4objects.Db4o.WindowsService {
	public class Program{
		internal static Arguments arguments;
		internal static ConsoleColor defaultConsoleColor;

		static void Main(string[] args) {
			arguments = new Arguments(args);
			defaultConsoleColor = Console.ForegroundColor;

			try {
				if (arguments.IsDefined("?") || arguments.IsDefined("h") || arguments.IsDefined("help")) {
					ShowBanner();
					ShowHelp();
					return;
				}

				if (arguments.IsDefined("i") || arguments.IsDefined("install")) {
					ShowBanner();
					Console.WriteLine("Installing service: " + ServiceDefinition.DisplayName);
					Console.WriteLine();
					Console.ForegroundColor = ConsoleColor.DarkGray;
					if (IsUserAdministrator())
						InstallService(false, args);
					else
						throw new ApplicationException("Needs to be run as Administrator.");
					
					if (arguments.IsDefined("r") || arguments.IsDefined("run")) {
						StartService();
					}
					return;
				}
				
				if (arguments.IsDefined("u") || arguments.IsDefined("uninstall")) {
					ShowBanner();
					Console.WriteLine("Uninstalling service: " + ServiceDefinition.DisplayName);
					Console.WriteLine();
					Console.ForegroundColor = ConsoleColor.DarkGray;
					if (IsUserAdministrator())
						InstallService(true, args);
					else
						throw new ApplicationException("Needs to be run as Administrator.");
					return;
				}

				if (arguments.IsDefined("r") || arguments.IsDefined("run")) {
					StartService();
					return;
				}

				RunService();
			}
			catch (ApplicationException ex) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				ShowHelp();
			}
			catch (Exception ex) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.ToString());
			}
			finally {
				Console.ForegroundColor = defaultConsoleColor;
			}
		}

		private static void StartService() {
			ShowBanner();
			Console.WriteLine("Starting service: " + ServiceDefinition.DisplayName);
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGray;
			if (IsUserAdministrator()) {
				var sc = new System.ServiceProcess.ServiceController();
				sc.ServiceName = "db4oNetService";
				switch(sc.Status){
					case ServiceControllerStatus.Running:
					case ServiceControllerStatus.StartPending:
						Console.WriteLine("Service is already running.");
						break;
					case ServiceControllerStatus.StopPending:
						Console.WriteLine("Service is stopping.");
						break;
					case ServiceControllerStatus.Stopped:
						sc.Start();
						try {
							sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
						}
						catch (System.ServiceProcess.TimeoutException) {
							throw new ApplicationException("Timeout waiting to start the service.");
						}
						break;
				}
			}
			else
				throw new ApplicationException("Needs to be run as Administrator.");
		}

		private static void ShowBanner() {
			var assemblyname = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("db4o Network Service, .NET Edition for Windows, version " + assemblyname.Version);
			Console.ForegroundColor = defaultConsoleColor;
			Console.WriteLine();
		}

		private static void ShowHelp() {
			var assemblyname = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.Write(assemblyname.Name);
			Console.ForegroundColor = defaultConsoleColor;
			Console.WriteLine(" [/I] [/R]");
			Console.WriteLine("{0," + assemblyname.Name.Length + "} [/U]", "");
			Console.WriteLine();
			Console.WriteLine("  /I\tInstall as a Windows Service");
			Console.WriteLine("  /U\tUninstall the Windows Service");
			Console.WriteLine("  /R\tRun the service");
		}

		private static void InstallService(bool undo, string[] args) {
			using (AssemblyInstaller inst = new AssemblyInstaller(Assembly.GetExecutingAssembly(), args)) {
				var state = new Hashtable();
				inst.UseNewContext = true;
				try {
					if (undo) {
						inst.Uninstall(state);
					}
					else {
						inst.Install(state);
						inst.Commit(state);
					}
				}
				catch {
					try {
						inst.Rollback(state);
					}
					catch { }
					throw;
				}
			} 
		}

		private static void RunService() {
			var service = new Db4oServiceHost();
			ServiceBase.Run(service);
		}

		private static bool IsUserAdministrator() {
			//bool value to hold our return value
			bool isAdmin;
			try {
				//get the currently logged in user
				WindowsIdentity user = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(user);
				isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (UnauthorizedAccessException) {
				isAdmin = false;
			}
			catch (Exception) {
				isAdmin = false;
			}
			return isAdmin;
		}
	}
}
