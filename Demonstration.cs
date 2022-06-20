/** Demonstration file, only to showcase and give an example to how you can implement and
 * use the Framework. */

using System;
using Renci.SshNet; /** Only necessary in the backend file, in this file it is only used for demonstration. */
using ServerGateway; /** Include the API's namespace. */

class Program
{
    static void Main(/* string[] args */) /** Uncomment this arguement if the program has the entry point error (This can happen if you're using Visual Studio 2019 or older versions). */
    {
        Console.WriteLine("Starting operation...");

        /** Server informations what the Framework will use to connect the SSH and SFTP clients. */
        // Fill in with your datas, must be valid to work properly
        string username = "username";
        string ipAddress = "ipaddress";
        string password = "password";

        /** A simple Linux bash command for just showcase. */
        string command = "echo 'Welcome to Linux (:'";

        /** Creates an instance of the ServerGateway class that contains all the Framework's 
         * definitions and functions. */
        ServerGatewayService serverGatewayService = new ServerGatewayService(username, ipAddress, password);
        /** Setting all type of Logging to enabled, because this is a console application, so we can make use of logging in this example. */
        serverGatewayService.SetAllLoggingEnablesToTrue(true);

        /** Using a function on the instance to get some basic information about the server. */
        SshCommand infos = serverGatewayService.GetServerInformations();
        Console.WriteLine(infos.Result);

        /** Sends a task through the Framework's SSH client. */
        SshCommand sshCommand = serverGatewayService.SendTask(command);
        Console.WriteLine(sshCommand.Result);

        /** Download a file from the server into this machine. */
        serverGatewayService.DownloadFile("/var/www/html/marcisuli/Program.cs", @"C:\dev\");

        Console.WriteLine("Operation exited successfully!");
    }
}
