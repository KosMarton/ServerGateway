/** Copyright Márton Kós Hungary 2021. All Rights Reserved
 * CSharp SSH and SFTP Framework for Visual Studio Integration */

using System;
using System.IO;
using Renci.SshNet;
using System.Text;

namespace ServerGateway /** Include namespace if the Framework class and the implemetation definitions are not in the same namespace. */
{
    /** This class contains all the definitions and functions that this Framework comes with.
     * After the contruction of the class instance finishes all the functions all usable if
     * the given class parameters are valid. Construction speed depends on internet speed.*/
    public sealed class ServerGatewayService
    {
        /** Constructor thats sets up the connection with the server an populates the internal variables
         * and if the connection was successfull then sends a message through Console.
         * ISSUE: if the give IP address is invalid, the program crashes. */
        public ServerGatewayService(string username, string ipAddress, string password)
        {
            m_Username = username;
            m_IpAddress = ipAddress;
            m_Password = password;

            /** Sets up the SSH connection client through which information is 
             * sent by the Framework. */
            try
            {
                AuthenticationMethod method = new PasswordAuthenticationMethod(m_Username, m_Password);
                ConnectionInfo connection = new ConnectionInfo(m_IpAddress, m_Username, method);
                m_SshClient = new SshClient(connection);
                if (!m_SshClient.IsConnected)
                {
                    LogMessage($"Connected server on: {m_IpAddress} in user: {m_Username}");
                    m_SshClient.Connect();
                }
            }
            catch (Exception ex)
            {
                LogError("Failed to set up the SSH connenction. Please check your server login information.");
                LogError($"Detailed exception: {ex}");
                Environment.Exit(1);
            }
            
            /** Sets up the SFTP connection client through which files is 
             * sent and received by the Framework. */
            try
            {
                m_SftpClient = new SftpClient(new PasswordConnectionInfo(m_IpAddress, m_Username, m_Password));
                m_SftpClient.Connect();
            }
            catch (Exception ex)
            {
                LogError("Failed to set up the SFTP connenction. Please check your server login information.");
                LogError($"Detailed exception: {ex}");
                Environment.Exit(1);
            }

            /** Finishing sign, that only displayed if everything went well. */
            SshCommand readCommand = m_SshClient.RunCommand("uname -mrs");
            LogMessage("Server's version: " + readCommand.Result);
        }

        /** Deconstructor thats disconnects the Framework's SSH and SFTP clients from server
         * when the class gets deleted from memory.*/
        ~ServerGatewayService()
        {
            m_SshClient.Disconnect();
            m_SftpClient.Disconnect();
        }

        /** Gets some basic informations about the server's operating system. */
        public SshCommand GetServerInformations()
        {
            return m_SshClient.RunCommand("cat /etc/os-release");
        }

        /** Sends a command to the server and immediately executes it.
         * Most of the other functions in the Framework use this as a base function. */
        public SshCommand SendTask(string command)
        {
            return m_SshClient.RunCommand(command);
        }

        /** Creates a file on the server with the given path and
         * writes in the given contents if there are. */
        public SshCommand CreateFile(string filePath, string fileContents = "")
        {
            return SendTask($"echo {fileContents} > {filePath}");
        }

        /** Creates files on the server with the given paths and
         * writes in the given contents if there are. */
        public SshCommand[] CreateFiles(string[] filePaths, string[] fileContents = default)
        {
            SshCommand[] commands = default;
            for (int i = 0; i < filePaths.Length; i++)
            {
                commands[i] = SendTask($"echo {fileContents[i]} > {filePaths[i]}");
            }
            return commands;
        }

        /** Deletes a file on the server with the given path. */
        public SshCommand DeleteFile(string filePath)
        {
            return SendTask($"rm {filePath}");
        }

        /** Deletes files on the server with the given paths. */
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
         * If the given file doesn't exists then the program does it first and
         * write the contents in afterwards. */
        public SshCommand WriteIntoFile(string filePath, string fileContents)
        {
            return SendTask($"echo {fileContents} > {filePath}");
        }

        /** Writes into the given filse regardless if they exist.
         * If the given files aren't exist then the program does it first and
         * write the contents in afterwards. */
        public SshCommand[] WriteIntoFiles(string[] filePaths, string[] fileContents)
        {
            SshCommand[] commands = default;
            for (int i = 0; i < filePaths.Length; i++)
            {
                commands[i] = SendTask($"echo {fileContents[i]} > {filePaths[i]}");
            }
            return commands;
        }

        /** Appends the given contents in the given file. */
        public SshCommand AppendTextToFile(string filePath, string fileContents)
        {
            return SendTask($"echo {fileContents} >> {filePath}");
        }

        /** Appends the given contents in the given files. */
        public SshCommand[] AppendTextToFiles(string[] filePaths, string[] fileContents)
        {
            SshCommand[] commands = default;
            for (int i = 0; i < filePaths.Length; i++)
            {
                commands[i] = SendTask($"echo {fileContents[i]} >> {filePaths[i]}");
            }
            return commands;
        }

        /** Uploads a file to the server from the given local path.
         * Then the uploaded file placed in the given destination path.
         * The given path must be on your local machine (what from you running this Framework). */
        public void UploadFile(string localFilePath, string destinationPath)
        {
            using (Stream stream = File.OpenRead(localFilePath))
            {
                m_SftpClient.UploadFile(stream, destinationPath + Path.GetFileName(localFilePath), x =>
                { LogMessage($"Uploaded file name: [{Path.GetFileName(localFilePath)}] Uploaded file size (in bytes): [{x}]"); });
            }
        }

        /** Uploads files to the server from the given local paths.
         * Then the uploaded files placed in the given destination paths.
         * The given paths must be on your local machine (what from you running this Framework). */
        public void UploadFiles(string[] localFilePaths, string[] destinationPaths)
        {
            for (int i = 0; i < localFilePaths.Length; i++)
            {
                using (Stream stream = File.OpenRead(localFilePaths[i]))
                {
                    m_SftpClient.UploadFile(stream, destinationPaths[i] + Path.GetFileName(localFilePaths[i]), x =>
                    { LogMessage($"Uploaded file name: [{Path.GetFileName(localFilePaths[i])}] Uploaded file size (in bytes): [{x}]"); });
                }
            }
        }

        /** Downloads a file from the server to the given local path.
         * Then the downloaded file placed in the given local destination path.
         * The given downloadable path must be on your server. */
        public void DownloadFile(string downloadableFilePath, string destinationPath)
        {
            using (Stream stream = File.OpenWrite(destinationPath + Path.GetFileName(downloadableFilePath)))
            {
                m_SftpClient.DownloadFile(downloadableFilePath, stream, x =>
                { LogMessage($"Downloaded file name: [{Path.GetFileName(downloadableFilePath)}] Downloaded file size (in bytes): [{x}]"); });
            }
        }

        /** Downloads files from the server to the given local paths.
         * Then the downloaded files placed in the given local destination paths.
         * The given downloadable paths must be on your server. */
        public void DownloadFiles(string[] downloadableFilePaths, string[] destinationPaths)
        {
            for (int i = 0; i < downloadableFilePaths.Length; i++)
            {
                using (Stream stream = File.OpenWrite(destinationPaths[i] + Path.GetFileName(downloadableFilePaths[i])))
                {
                    m_SftpClient.DownloadFile(downloadableFilePaths[i], stream, x =>
                    { LogMessage($"Downloaded file name: [{Path.GetFileName(downloadableFilePaths[i])}] Downloaded file size (in bytes): [{x}]"); });
                }
            }
        }

        /** Private internal functions that the class uses to print to the Console, if logging is enabled. */
        internal void LogMessage(string message)
        {
            if (loggingEnabled_Message)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
            }
        }
        internal void LogError(string message)
        {
            if (loggingEnabled_Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
            }
        }

        /** Public variables that determines, whether the Framework can log or not.
         * They can be modified outside of the class. */
        public bool loggingEnabled_Message = true;
        public bool loggingEnabled_Error = true;

        /** Private internal variables to get hold of the reusable information */
        internal readonly string m_Username;
        internal readonly string m_IpAddress;
        internal readonly string m_Password;
        internal readonly SshClient m_SshClient;
        internal readonly SftpClient m_SftpClient;
    }
}
