// Demonstration file, only to showcase and give an example to how you can implement the API

using System;
using Renci.SshNet;
using ServerGateway; // Include the API's namespace

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting operation...");

        // Server informations that the API needs to connect to the server through SSH
        string username = "yourserverusername";
        string ipAddress = "yourserveripaddress";
        string password = "yourserverpassword";
        string command = "echo 'Welcome to Linux (:'";
        
        // Creates an instance of the Service
        ServerGatewayService serverGatewayService = new ServerGatewayService(username, ipAddress, password);
        // Using a function on the instance to get some basic information about the server
        SshCommand infos = serverGatewayService.GetServerInformations();
        Console.WriteLine(infos.Result);

        // Sends a task through the API
        SshCommand sshCommand = serverGatewayService.SendTask(command);
        Console.WriteLine(sshCommand.Result);

        Console.WriteLine("Operation exited successfully!");
    }
}