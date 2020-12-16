using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Preload : MonoBehaviour
{
    public string GlobalFilePath = string.Empty;

    //Load settings on awake
    private void Awake()
    {
        //HAVE TO DO THIS IN AWAKE
        GlobalFilePath = (Application.dataPath + "/server-properties.txt");

        try
        {
            LoadSettings(GlobalFilePath);
            InterpretProperties(GlobalFilePath);
        }
        catch
        {
            Debug.LogError("Could not load server settings, maybe the server properties file is corrupted? Or path is inaccessible?");
        }

        
    }

    /// <summary>
    /// Attempt to load server settings from application path, return if result was sucess or not
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private bool LoadSettings(string path)
    {
        //Check if server file exists
        if (File.Exists(path))
        {
            Debug.Log("File exists at: " + path);
            return true;
        }
        else
        {
            Debug.Log("File doesn't exist at: '" + path + "'");
            Debug.Log("Creating server property file with default settings...");

            //Attempt to write server property file to application path
            try
            {
                string[] lines = {
                "#Taurus Dedicated Unity Server",
                "Server-Name=Tile goes here",
                "Description=Description goes here",
                "Server-Port=2500",
                "Max-Players=8" };

                //Create default values
                using (StreamWriter file = new StreamWriter(path))
                {
                    foreach (string line in lines)
                    {
                        file.WriteLine(line);
                    }
                }

                Debug.Log("Created default server properties at '" + path + "'");
                return true;
            }
            catch
            {
                Debug.LogError("Could not write property file to '" + path + "' maybe the path is inaccessible?");
                return false;
            }
        }
    }

    /// <summary>
    /// Interprets server property file text to 'ServerData' class type
    /// </summary>
    private void InterpretProperties(string path)
    {
        ServerData data = new ServerData();
        string[] keywords = new string[] { "Server-Name", "Description", "Server-Port", "Max-Players" };
        

        try
        {
            int counter = 0;
            string line = string.Empty;
            string[] lines = new string[6]; //Number of lines in file

            //Get all lines in file
            StreamReader file = new StreamReader(path);
            while((line = file.ReadLine()) != null)
            {
                lines[counter] = line;
                counter++;
            }

            //Interate through each line, find keywords and parse data from lines
            for(int i = 0; i < counter; i++)
            {
                for(int j = 0; j < keywords.Length; j++)
                {
                    //Check if current line contains any keywords
                    if (lines[i].Contains(keywords[j]))
                    {
                        //Once keyword is found split left and right side of = to get data on right-hand side
                        string[] temp = lines[i].Split('=');

                        Debug.Log(temp[1]);

                        //Dependent on value, assign to ServerData value
                        switch (keywords[j])
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
                        }

                    }
                }
            }

            Debug.Log("Loaded server properties");


        }
        catch
        {
            Debug.LogError("Cannot read or interpret server properties. Maybe it's inaccessible or corrupt?");
        }

    }

}
