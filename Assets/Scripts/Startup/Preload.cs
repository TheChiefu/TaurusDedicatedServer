using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Preload : MonoBehaviour
{
    private string GlobalFilePath = string.Empty;
    private string[] Properties = new string[] { "Server-Name", "Description", "Server-Port", "Max-Players", "Map-ID", "Gamemode-ID", "Version"};
    public ServerData GlobalData = new ServerData();

    //Load settings on awake
    private void Awake()
    {
        //HAVE TO DO THIS IN AWAKE
        GlobalFilePath = (Application.dataPath + "/server-properties.txt");

        //Check if server file exists
        if (File.Exists(GlobalFilePath))
        {
            Debug.Log("File exists at: " + GlobalFilePath);

            try
            {
                InterpretProperties(GlobalFilePath, ref GlobalData);
            }
            catch
            {
                Debug.LogError("Cannot read or interpret server properties. Maybe it's inaccessible or corrupt?");
                System.Console.ReadKey();
                Application.Quit();
            }
        }

        //If file doesn't exist try to write defaults
        else
        {
            try
            {
                WriteDefaults(GlobalFilePath);
                InterpretProperties(GlobalFilePath, ref GlobalData);
            }
            catch
            {
                Debug.LogError("Could not write property file to '" + GlobalFilePath + "' maybe the path is inaccessible?");
                System.Console.ReadKey();
                Application.Quit();
            }
        }
    }


    /// <summary>
    /// Given path write a default server property file
    /// </summary>
    /// <param name="path"></param>
    private void WriteDefaults(string path)
    {
        Debug.Log("File doesn't exist at: '" + path + "'");
        Debug.Log("Creating server property file with default settings...");

        //Attempt to write server property file to application path
        string[] lines = {
                "# Taurus Dedicated Unity Server",
                "# ",
                "# Map IDs: ",
                "# 0. Testing Grounds ",
                "# 1. Fighting Arena",
                "# ",
                "# Gamemode IDs: ",
                "# 0. Team Deathmatch ",
                "# 1. Capture the Flag", 
                "# 2. King of the Hill",
                "# 3. Free for All\n",
                "# Server Properties ",
                "Version=" + Application.version,
                "Server-Name=Tile goes here",
                "Description=Description goes here",
                "Server-Port=2500",
                "Max-Players=8",
                "Map-ID=0",
                "Gamemode-ID=0"
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
    /// Interprets server property file text to 'ServerData' class type
    /// </summary>
    private void InterpretProperties(string path, ref ServerData data)
    {
        string line = string.Empty;
        List<string> lines = new List<string>();

        //Get all lines in file
        StreamReader file = new StreamReader(path);
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
                        case "Server-Name":
                            data.name = temp[1];
                            break;
                        case "Description":
                            data.description = temp[1];
                            break;
                        case "Server-Port":
                            data.port = int.Parse(temp[1]);
                            break;
                        case "Max-Players":
                            data.maxPlayers = int.Parse(temp[1]);
                            break;
                        case "Map-ID":
                            data.mapID = int.Parse(temp[1]);
                            break;
                        case "Gamemode-ID":
                            data.gamemodeID = int.Parse(temp[1]);
                            break;
                        case "Version":
                            VersionCheck(temp[1]);
                            break;
                    }
                }
            }
        }

        Debug.Log("Loaded server properties sucessfully.");
        VerboseServerInfo();
        Debug.Log("");
    }

    /// <summary>
    /// Outputs a debug log that describes all the server information for sanity sake
    /// </summary>
    private void VerboseServerInfo()
    {
        string output = ("Server Info: \n"
            + "Server Name: " + GlobalData.name + "\n"
            + "Description: " + GlobalData.description + "\n"
            + "Server-Port: " + GlobalData.port + "\n"
            + "Max-Players: " + GlobalData.maxPlayers + "\n"
            + "Map-ID: " + GlobalData.mapID + "\n"
            + "Gamemode-ID: " + GlobalData.gamemodeID + "\n"
            + "Version: " + Application.version);

        Debug.Log(output);
    }

    /// <summary>
    /// Check what version the server is running on versus the properties file
    /// </summary>
    private void VersionCheck(string fileVersion)
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
}
