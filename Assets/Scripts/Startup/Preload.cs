using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Preload : MonoBehaviour
{
    
    /// <summary>
    /// Property fields accessable in server-properties file that modify server behavior
    /// </summary>
    private string[] Properties = new string[] { "Server-Name=", "Server-Description=", "Server-Port=", "Max-Players=", "Map-ID=", "Gamemode-ID=", "Server-Version=", "Server-MOTD=", "Server-Admins="};
    public ServerData GlobalData = null;

    //Load settings on awake
    private void Awake()
    {
        //HAVE TO DO THIS IN AWAKE
        string PropertyPath = (Application.dataPath + "/server-properties.txt");
        string DocumentPath = (Application.dataPath + "/server-documentation.txt");

        //Check if server documentation exists, if not create it
        CheckDocsExists(DocumentPath);

        //Check if server property file exists, if not create it and then start server
        if (CheckPropertyFileExists(PropertyPath, ref GlobalData))
        {
            VerboseServerInfo(GlobalData);
            //Start Server
        }

        //If one can't be created exit program
        else
        {
            Debug.LogError("Could not read/write property file to '" + PropertyPath + "' maybe the path is inaccessible?");
            System.Console.ReadKey();
            Application.Quit();
        }
    }





    // Checker Functions //

    /// <summary>
    /// Check if server-property file exists
    /// </summary>
    /// <param name="path"></param>
    private bool CheckPropertyFileExists(string path, ref ServerData data)
    {
        //Check if server file exists
        if (File.Exists(path))
        {
            Debug.Log("File exists at: " + path);

            try
            {
                data = InterpretProperties(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //If file doesn't exist try to write defaults
        else
        {
            try
            {
                WriteDefaults(path);
                data = InterpretProperties(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Checks if server documentation exists
    /// </summary>
    /// <param name="path"></param>
    private void CheckDocsExists(string path)
    {
        //Check if server file exists
        if (!File.Exists(path))
        {
            try
            {
                Debug.Log("Document doesn't exists at: " + path + "\nCreating one...");
                WriteServerDocumentation(path);
            }

            catch
            {
                Debug.Log("Could not write server document to: " + path + "\nMaybe it is unaccessable?");
            } 
        }
    }

    /// <summary>
    /// Interprets properties from path and return newly created ServerData class object
    /// </summary>
    private ServerData InterpretProperties(string path)
    {

        ServerData data = new ServerData();

        //Get all lines in file
        List<string> lines = new List<string>();
        StreamReader file = new StreamReader(path);
        string line;
        while ((line = file.ReadLine()) != null) lines.Add(line);

        //Interate through each line, find keywords and parse data from lines
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < Properties.Length; j++)
            {
                //Check if current line contains any keywords
                if (lines[i].Contains(Properties[j]))
                {
                    //Once keyword is found split left and right side of = to get data on right-hand side
                    string[] temp = lines[i].Split('=');

                    //If any value is empty report it
                    if(temp[1] == string.Empty)
                    {
                        Debug.LogError(temp[0] + " has no value assigned for it!");
                    }

                    //Dependent on value, assign to ServerData value
                    switch (Properties[j])
                    {
                        case "Server-Name=":
                            data.name = temp[1];
                            break;
                        case "Server-Description=":
                            data.description = temp[1];
                            break;
                        case "Server-Port=":
                            data.port = int.Parse(temp[1]);
                            break;
                        case "Max-Players=":
                            data.maxPlayers = int.Parse(temp[1]);
                            break;
                        case "Map-ID=":
                            data.mapID = int.Parse(temp[1]);
                            break;
                        case "Gamemode-ID=":
                            data.gamemodeID = int.Parse(temp[1]);
                            break;
                        case "Server-Version=":
                            CheckVersion(temp[1]);
                            break;
                        case "Server-MOTD=":
                            data.MOTD = temp[1];
                            break;
                        case "Server-Admins=":
                            data.Admins = temp[1].Split(',');   //Get list of names seperated by comma values
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        Debug.Log("Loaded server properties sucessfully.");
        Debug.Log("");

        return data;
    }

    /// <summary>
    /// Check what version the server is running on versus the properties file
    /// </summary>
    private void CheckVersion(string fileVersion)
    {
        //Attempt to find version numbers and compare between them
        try
        {
            if (float.Parse(fileVersion) > float.Parse(Application.version))
            {
                Debug.LogWarning("This property file is newer than the current server version.\n" +
                    "This may cause issues with the server, so be wary of any issues or bugs that arise.\n" +
                    "Try running an older server version that matches the property file. Or update the property file.");
            }
            else if (float.Parse(fileVersion) < float.Parse(Application.version))
            {
                Debug.LogWarning(
                "This server property file is oudated, it should be running on version: " + Application.version + " but is running on version: " + fileVersion
                + "\nPlease update this file or delete it to refresh it with a new version's default values."
                + "\nYou can keep the server running with these current settings, but issues or bugs may arrise so please be wary."
                );
            }
        }
        catch
        {
            Debug.LogWarning("Error: Could not compare between versions.");
        }
    }




    // Writing Functions //



    /// <summary>
    /// From given path of Application, write server documentation to allow user to understand the Server-Properties file
    /// </summary>
    /// <param name="path"></param>
    private void WriteServerDocumentation(string path)
    {
        string[] output = {
            "# Taurus Dedicated Server Documentation #\n",
            "List of Server Properties available:",
            "- Server-Name: This is self explanatory and is the display name of the server",
            "- Server-Description: Again self explanatory and describes what the server is about",
            "- Server-Port: The port the host machine uses to send/receive data from",
            "- Max-Players: The maximum number of players allowed on the server",
            "- Map-ID: The ID of the map being played",
            "- Gamemode-ID: The ID of the gamemode being used",
            "- Server-Version: The version of the server being ran",
            "- Server-MOTD: Message of the day that displays on map refresh",
            "- Server-Admins: Approved usernames that can operate server commands in the game. Follow usernames by commas to add to approved list.",
            "\tFor example: User1,User2",
            "\n",
            "Map IDs:",
            "- 0. Testing Grounds",
            "- 1. Space Port",
            "\nGamemode IDs:",
            "- 0. Team Deathmatch",
            "- 1. Capture the Flag",
            "- 2. King of the Hill",
            "- 3. Free for All",
            "\n# Server based on Tom Weiland's Unity Server: https://github.com/tom-weiland/tcp-udp-networking"
            };

        //Create default values
        using (StreamWriter file = new StreamWriter(path))
        {
            foreach (string line in output)
            {
                file.WriteLine(line);
            }
        }
    }

    /// <summary>
    /// Given path write a default server property file
    /// </summary>
    /// <param name="path"></param>
    private void WriteDefaults(string path)
    {
        Debug.Log("Server property file doesn't exist at: '" + path + "'");
        Debug.Log("Creating server property file with default settings...");

        //Attempt to write server property file to application path
        string[] lines = {
                "# Taurus Dedicated Unity Server",
                "# Refer to the docmentation for more instruction on how to customize the server",
                "",
                "# Server Properties ",
                "Server-Version=" + Application.version,
                "Server-Name=Tile goes here",
                "Description=Description goes here",
                "Server-Port=2500",
                "Max-Players=8",
                "Map-ID=0",
                "Gamemode-ID=0",
                "MOTD=Welcome to the Server! Enjoy your stay.",
                "Server-Admins=Admin"
                };

        //Create default values
        using (StreamWriter file = new StreamWriter(path))
        {
            foreach (string line in lines)
            {
                file.WriteLine(line);
            }
        }

        Debug.Log("Created default server properties at '" + path + "'");
    }

    /// <summary>
    /// Outputs a debug log that describes all the server information for sanity sake
    /// </summary>
    private void VerboseServerInfo(ServerData data)
    {
        string output = ("Server Info: \n"
            + "Server Name: " + data.name + "\n"
            + "Description: " + data.description + "\n"
            + "Server-Port: " + data.port + "\n"
            + "Max-Players: " + data.maxPlayers + "\n"
            + "Map-ID: " + data.mapID + "\n"
            + "Gamemode-ID: " + data.gamemodeID + "\n"
            + "Version: " + Application.version + "\n"
            + "MOTD: " + data.MOTD
            );

        Debug.Log(output);
    }
}
