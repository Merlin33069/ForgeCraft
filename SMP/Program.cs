using System;
using System.Reflection;
using System.Threading;

namespace SMP
{
    internal static class Program
    {
        private static Server Server;

        static Program()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException_Handler;
            AppDomain.CurrentDomain.ProcessExit += (CurrentDomain_ProcessExit);
        }

        public static bool RunningInMono()
        {
            return (Type.GetType("Mono.Runtime") != null);
        }

        [MTAThread]
        public static void Main(string[] args)
        {
            /*for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-port":
                        Settings.Default.Port = Convert.ToInt32(args[++i]);
                        break;
                    case "-ip":
                        Settings.Default.IPAddress = args[++i];
                        break;
                }
            }*/

            StartServer();
            StartInput();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (Server != null)
                Server.Stop();
        }

        private static void Exit()
        {
            Server.Stop();
            Server = null;
        }

        private static void StartServer()
        {
        	(Server = new Server()).Setup();
        }

        private static void StartInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input == null || (input = input.Trim()).Length == 0)
                    continue;

                string[] inputParts = input.Split();

                if (inputParts[0].StartsWith("/"))
                {
                    string cmd = inputParts[0].Substring(1).ToLower();
                    
                    switch (cmd)
                    {
                        case "stop":
                            Server.ServerLogger.Log(LogLevel.Info, "Stopping Server...");
							foreach (Player p in Player.players)
							{
								p.Kick("Server Shutting Down!");
							}
                            Exit();
                            return;
                        default:
                            Command command = Command.all.Find(cmd);
                            if (command == null)
                            {
                                Server.ServerLogger.Log(LogLevel.Info, "Unrecognized command: " + cmd);
                                break;
                            } 

                            if (command.ConsoleUseable)
                            {
                                string[] args = new string[inputParts.Length - 1];
                                
                                Array.Copy(inputParts, 1, args, 0, inputParts.Length - 1);

                                command.Use(Server.consolePlayer, args);
                                break;
                            }
                            else
                            {
                                Server.ServerLogger.Log(LogLevel.Info, cmd + " command not useable in the console.");
                                break;
                            }
                    }
                }
                else if (inputParts[0].StartsWith("@"))
                {
                    string name = inputParts[0].Substring(1);
                    string message = "";
                    Player p = Player.FindPlayer(name);
                    Server.ServerLogger.Log(name + " : " );
                    
                    if (p != null)
                    {
                        if (inputParts.Length <= 1)
                        {
                            Server.ServerLogger.Log(LogLevel.Warning, "Please enter a message to send");
                        }
                        else if (input.Length > 1)
                        {
                            for (int i = 1; i < inputParts.Length; i++)
                            {
                                message += inputParts[i];
                            }
                            p.SendMessage(Color.PrivateMsg + "[Server >> Me] " + Color.ServerDefaultColor + message);
                        }
                    }
                    else
                    {
                        Server.ServerLogger.Log(LogLevel.Warning, "Please enter a valid username");
                    }
                }
                else
                {
                    Player.GlobalMessage(Color.Announce + input);
                }
            }
        }

        private static void UnhandledException_Handler(object sender, UnhandledExceptionEventArgs e)
        {
            Server.ServerLogger.LogError((Exception)e.ExceptionObject);
        }
    }
}
