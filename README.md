# Server Gateway
## A simple and fast C# Framework that allows you to communicate with a server over SSH and SFTP connections.

This Framework will be continuously developed and expanded with additional features.
For now this is only a Beta version of the Framework, but the main functionalities are already implemented in the code.
Feel free to send issues to help the development of the Framework.
The program tested and developed in Visual Studio 2022 and I recommend to use with that version. The source file's code is version independent
so you can use it in later versions as well.

## How to use it?

To clone the repository just go to the desired directory on your machine, open a cmd and type: `git clone https://github.com/KosMarton/ServerGateway`.
Then if the repository have cloned to your directory, open the Visual Studio Solution file that the repository comes with (ServerGateway.sln).
Then just build and run the program. Don't forget to fill in the server parameter variables in the demonstration file (Demonstration.cs).

## If you want to use the Framework in your projects just import the `ServerGateway.cs` file into your project and include the `ServerGateway` namespace.
## Don't forget to download the Renci.Sshnet NuGet package to your project solution, otherwise it's not going to work. Thats becase the backend of the code needs this additional library to function. The repository's solution already contains this additional library!

## License
The code is open source and free to copy but don't forget to mention my Github if you using this in your project.
You can read more in the license file.

Have fun and good coding :)
