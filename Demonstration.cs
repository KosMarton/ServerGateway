/** Demonstration file, only to showcase and give an example to how you can implement and
 * use the Framework. */

using System;
using Renci.SshNet;
using ServerGateway; /** Include the API's namespace. */

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting operation...");

        /** Server informations what the Framework will use to connect the SSH and SFTP clients. */
        string username = "serverusername";
        string ipAddress = "serveripadress";
        string password = "serverpassword";

        /** A simple Linux bash command for just showcase. */
        string command = "echo 'Welcome to Linux (:'";

        /** Creates an instance of the ServerGateway class that contains all the Framework's 
         * definitions and functions. */
        ServerGatewayService serverGatewayService = new ServerGatewayService(username, ipAddress, password);
        /** Using a function on the instance to get some basic information about the server. */
        SshCommand infos = serverGatewayService.GetServerInformations();
        Console.WriteLine(infos.Result);

        /** Sends a task through the Framework's SSH client. */
        SshCommand sshCommand = serverGatewayService.SendTask(command);
        Console.WriteLine(sshCommand.Result);

        /** Download a file from the server into this machine. */
        serverGatewayService.DownloadFile("/var/local/image-2.png", @"C:\dev\");

        Console.WriteLine("Operation exited successfully!");
    }
}