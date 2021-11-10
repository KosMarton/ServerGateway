// Copyright Márton Kós Hungary 2021. All Rights Reserved

using System;
using System.IO;
using Renci.SshNet;
using System.Text;

namespace ServerGateway
{
    /** The class for the entire API
     * This class contains all the functions what the API comes with */
    class ServerGatewayService
    {
        /** Constructor thats sets up the connection with the server an populates the internal variables
         * And if the connection was successfull then sends a message through Console
         * ISSUE: if the give IP address is invalid, the program crashes */
        public ServerGatewayService(string username, string ipAddress, string password)
        {
            m_Username = username;
            m_IpAddress = ipAddress;
            m_Password = password;

            AuthenticationMethod method = new PasswordAuthenticationMethod(m_Username, m_Password);
            ConnectionInfo connection = new ConnectionInfo(m_IpAddress, m_Username, method);
            client = new SshClient(connection);
            if (!client.IsConnected)
            {
                Console.WriteLine($"Connected server on: {m_IpAddress} in user: {m_Username}");
                client.Connect();
            }
            SshCommand readCommand = client.RunCommand("uname -mrs");
            Console.WriteLine("Server's version: " + readCommand.Result);
        }

        /** Deconstructor thats disconnects the API from server when the class gets deleted */
        ~ServerGatewayService()
        {
            client.Disconnect();
        }

        /** Gets some basic informations about the servers operating system */
        public SshCommand GetServerInformations()
        {
            return client.RunCommand("cat /etc/os-release");
        }

        /** Sends a command to the server and immediately executes it */
        public SshCommand SendTask(string command)
        {
            return client.RunCommand(command);
        }

        /** Creates a file on the server with the given arguements */
        public SshCommand CreateFile(string filePath, string fileContents = "")
        {
            return SendTask($"echo {fileContents} > {filePath}");
        }

        /** Creates files on the server with the given arguements */
        public SshCommand[] CreateFiles(string[] filePaths, string[] fileContents = default)
        {
            SshCommand[] commands = default;
            for (int i = 0; i < filePaths.Length; i++)
            {
                commands[i] = SendTask($"echo {fileContents[i]} > {filePaths[i]}");
            }
            return commands;
        }

        /** Deletes a file on the server with the given arguement */
        public SshCommand DeleteFile(string filePath)
        {
            return SendTask($"rm {filePath}");
        }

        /** Deletes files on the server with the given arguements */
        public SshCommand DeleteFiles(string[] filePaths)
        {
            StringBuilder pathBuilder = new StringBuilder();
            for (int i = 0; i < filePaths.Length; i++)
            {
                if (i != 0)
                    pathBuilder.Append(" ");
                pathBuilder.Append(filePaths[i]);
            }
            return SendTask($"rm {pathBuilder}");
        }

        /** Writes into the given file regardless if its exists.
         * If the given file doesnt exists then the program does it first and
         * write the arguements in afterwards */
        public SshCommand WriteIntoFile(string filePath, string fileContents)
        {
            return SendTask($"echo {fileContents} > {filePath}");
        }

        /** Writes into the given filse regardless if they exist.
         * If the given files arent exist then the program does it first and
         * write the arguements in afterwards */
        public SshCommand[] WriteIntoFiles(string[] filePaths, string[] fileContents)
        {
            SshCommand[] commands = default;
            for (int i = 0; i < filePaths.Length; i++)
            {
                commands[i] = SendTask($"echo {fileContents[i]} > {filePaths[i]}");
            }
            return commands;
        }

        /** Appends the given contents to the given file */
        public SshCommand AppendTextToFile(string filePath, string fileContents)
        {
            return SendTask($"echo {fileContents} >> {filePath}");
        }

        /** Appends the given contents to the given files */
        public SshCommand[] AppendTextToFiles(string[] filePaths, string[] fileContents)
        {
            SshCommand[] commands = default;
            for (int i = 0; i < filePaths.Length; i++)
            {
                commands[i] = SendTask($"echo {fileContents[i]} >> {filePaths[i]}");
            }
            return commands;
        }

        /** Uploads a file to the server from the given path.
         * The given path must be in your local machine (what from you running this API)
         * The 'destinationPath' must have be located on the server */
        public void UploadFile(string localFilePath, string destinationPath)
        {
            SftpClient client = new SftpClient(new PasswordConnectionInfo(m_IpAddress, m_Username, m_Password));
            client.Connect();

            using (Stream stream = File.OpenRead(localFilePath))
            {
                client.UploadFile(stream, destinationPath + Path.GetFileName(localFilePath), x =>
                { Console.WriteLine($"Uploaded file name: [{Path.GetFileName(localFilePath)}] Uploaded file size (in bytes): [{x}]"); });
            }

            client.Disconnect();
        }

        /** Downloads a file from the server to a destination path.
         * The 'destinationPath' must be in your local machine (what from you running this API)
         * The 'downloadableFilePath' must have be located on the server */
        public void DownloadFile(string downloadableFilePath, string destinationPath)
        {
            SftpClient client = new SftpClient(new PasswordConnectionInfo(m_IpAddress, m_Username, m_Password));
            client.Connect();

            using (Stream stream = File.OpenWrite(destinationPath + Path.GetFileName(downloadableFilePath)))
            {
                client.DownloadFile(downloadableFilePath, stream, x =>
                { Console.WriteLine($"Downloaded file name: [{Path.GetFileName(downloadableFilePath)}] Downloaded file size (in bytes): [{x}]"); });
            }

            client.Disconnect();
        }

        /** Private internal variables */
        private readonly string m_Username;
        private readonly string m_IpAddress;
        private readonly string m_Password;
        private readonly SshClient client;
    }
}
