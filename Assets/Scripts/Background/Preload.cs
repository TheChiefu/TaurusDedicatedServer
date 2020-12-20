using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Class that is first called on Server Application startup, to ensure everything loads properly.
/// </summary>
public class Preload : MonoBehaviour
{
    

    //Load settings on awake
    private void Awake()
    {

        //Set up core features before doing anything else
        ConsoleSetup();
        UnitySetup();

        //Clear Console
        Console.Clear();

        //HAVE TO DO THIS IN AWAKE
        string PropertyPath = (Application.dataPath + "/server-properties.txt");
        string DocumentPath = (Application.dataPath + "/server-documentation.txt");

        //Create server data to be passed to Server.cs script
        ServerData data = null;
        
        //Check if server documentation exists, if not create it
        CheckDocsExists(DocumentPath);

        //Check if server property file exists, if not create it and then start server
        if (CheckPropertyFileExists(PropertyPath, ref data))
        {
            //Start Server from Backend script
            Server.Start(data);
            Console.Title = data.name;

            //Load proper map/scene
            if(CheckForValidData(data))
            {
                
                SceneManager.LoadScene(data.mapID + 1, LoadSceneMode.Additive);
            }
            else
            {
                Console.ReadKey();
                Application.Quit();
            }   
        }

        //If one can't be created exit program
        else
        {
            Console.WriteLine("Could not read/write property file to '" + PropertyPath + "' maybe the path is inaccessible?");
            Console.ReadKey();
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
            Console.WriteLine("File exists at: " + path);

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
                Console.WriteLine("Document doesn't exists at: " + path + "\nCreating one...");
                WriteServerDocumentation(path);
            }

            catch
            {
                Console.WriteLine("Could not write server document to: " + path + "\nMaybe it is unaccessable?");
            } 
        }
    }

    /// <summary>
    /// Interprets properties from path and return newly created ServerData class object
    /// </summary>
    private ServerData InterpretProperties(string path)
    {

        ServerData data = new ServerData();
        string[] Properties = new string[] { "Server-Name=", "Server-Description=", "Server-Port=", "Max-Players=", "Map-ID=", "Gamemode-ID=", "Server-Version=", "Server-MOTD=", "Server-Admins="};


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
                        Console.WriteLine(temp[0] + " has no value assigned for it!");
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
                Console.WriteLine("This property file is newer than the current server version.");
                Console.WriteLine("This may cause issues with the server, so be wary of any issues or bugs that arise.");
                Console.WriteLine("Try running an older server version that matches the property file. Or update the property file.");
            }
            else if (float.Parse(fileVersion) < float.Parse(Application.version))
            {
                Console.WriteLine("This server property file is oudated, it should be running on version: " + Application.version + " but is running on version: " + fileVersion);
                Console.WriteLine("Please update this file or delete it to refresh it with a new version's default values.");
                Console.WriteLine("You can keep the server running with these current settings, but issues or bugs may arrise so please be wary.");
            }
        }
        catch
        {
            Console.WriteLine("Error: Could not compare between versions.");
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
        using StreamWriter file = new StreamWriter(path);
        foreach (string line in output)
        {
            file.WriteLine(line);
        }
    }

    /// <summary>
    /// Given path write a default server property file
    /// </summary>
    /// <param name="path"></param>
    private void WriteDefaults(string path)
    {
        Console.WriteLine("Server property file doesn't exist at: '" + path + "'");
        Console.WriteLine("Creating server property file with default settings...");

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

        Console.WriteLine("Created default server properties at '" + path + "'");
    }


    ////// QUESTIONABLE CODE FOR CHECKING VALID DATA //////

    /// <summary>
    /// Check all server data for valid values
    /// </summary>
    /// <param name="data"></param>
    private bool CheckForValidData(ServerData data)
    {
        //Checker for if no data is present, but is still okay for server to run without
        bool valid = true;

        //No name given
        if(data.name == string.Empty)
        {
            Console.WriteLine("Notice: No name given using default server name.");
            valid = true;
        }

        //No description given
        if(data.description == string.Empty)
        {
            Console.WriteLine("Notice: No description given using default server description.");
            valid = true;
        }

        //Outside of port range
        if(data.port < 0 || data.port > 65535)
        {
            Debug.LogError("Outside of valid port range!");
            valid = false;
        }

        //Less than 1 player or server cap
        if(data.maxPlayers < 1 || data.maxPlayers > 32)
        {
            Debug.LogError("Outside of valid max player range!");
            valid = false;
        }


        //Below 0 or above available map IDs
        if(data.mapID < 0 || data.mapID > 3)
        {
            Debug.LogError("Outside of valid the mapID range!");
            valid = false;
        }

        //Below 0 or above available gamemode IDs
        if(data.gamemodeID < 0 || data.gamemodeID > 3)
        {
            Debug.LogError("Outside of valid the gamemodeID range!");
            valid = false;
        }

        //No motto of the day given
        if(data.MOTD == string.Empty)
        {
            Console.WriteLine("No MOTD given, using default one.");
            valid = true;
        }

        //No admins
        if(data.Admins == null)
        {
            Console.WriteLine("Notice: No admins given, no one on the server can access server commands.");
            valid = true;
        }

        //Returner
        if (valid)
        {

            Console.WriteLine("No outstanding errors, using default values on certain values.");
            return true;
        }
        else
        {
            Console.WriteLine("Quitting the server.");
            return false;
        }

    }

    /// <summary>
    /// Set up the console with custom parameters
    /// </summary>
    private void ConsoleSetup()
    {
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.ForegroundColor = ConsoleColor.Black;
    }

    /// <summary>
    /// Setup Unity based values
    /// </summary>
    private void UnitySetup()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    //When application quits stop server
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
}
