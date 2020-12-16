﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class Constants
{
    // IMPORTANT Note
    // Unity FixedTimeSet is set by 1 / ServerTickRate
    // So if server tickrate is 30, it the FixedTimeSet will be 0.03

    public const int TICKS_PER_SEC = 120; // How many ticks per second
    public const float MS_PER_TICK = 1000f / TICKS_PER_SEC; // How many milliseconds per tick
    public enum Gamemode
    {
        Deathmatch,
        CaptureTheFlag,
        KingOfTheHill
    }


    
}


/// <summary>
/// Class with helper functions to find data, convert, and etc.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// From a given tag return a Transform array of found objects
    /// </summary>
    /// <param name="tagName"></param>
    /// <returns></returns>
    public static Transform[] FindTransformByTag(string tagName)
    {
        GameObject[] foundObj = GameObject.FindGameObjectsWithTag(tagName);
        Transform[] transformObj = new Transform[foundObj.Length];

        for (int i = 0; i < foundObj.Length; i++) transformObj[i] = foundObj[i].GetComponent<Transform>();
        return transformObj;
    }
}

//Used for getting and setting player data from client
public class PlayerData
{
    public int id { get; private set; }
    public string username { get; private set; }
    public int modelIndex { get; private set; }
    public int materialIndex { get; private set; } 

    public PlayerData (int id, string userName, int modelIndex, int materialIndex)
    {
        this.id = id;
        this.username = userName;
        this.modelIndex = modelIndex;
        this.materialIndex = materialIndex;
    }
}

//Used for server setup information
public class ServerData
{
    public string name { get; set; }
    public string description { get; set; }
    public int port { get; set; }
    public int maxPlayers { get; set; }
    public int mapID { get; set; }
    public int gamemodeID { get; set; }

    /// <summary>
    /// ServerData with all default values
    /// </summary>
    public ServerData()
    {
        this.name = "Default Server";
        this.description = "Give me a description!";
        this.port = 2500;
        this.maxPlayers = 8;
        this.mapID = 0;
        this.gamemodeID = 0;
    }

    /// <summary>
    /// ServerData object with default values but custom name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public ServerData(string name)
    {
        this.name = name;
        this.description = "Default Taurus Server Description";
        this.port = 2500;
        this.maxPlayers = 8;
        this.mapID = 0;
        this.gamemodeID = 0;
    }

    /// <summary>
    /// ServerData object with default values but custom name and description
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public ServerData(string name, string description)
    {
        this.name = name;
        this.description = description;
        this.port = 2500;
        this.maxPlayers = 8;
        this.mapID = 0;
        this.gamemodeID = 0;
    }

    /// <summary>
    /// ServerData object with default values but custom name, description, and port
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="port"></param>
    /// <param name="maxPlayers"></param>
    public ServerData(string name, string description, int port)
    {
        this.name = name;
        this.description = description;
        this.port = port;
        this.maxPlayers = 8;
        this.mapID = 0;
        this.gamemodeID = 0;
    }

    /// <summary>
    /// ServerData object with default values but custom name, description, port, and max players
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="port"></param>
    /// <param name="maxPlayers"></param>
    public ServerData (string name, string description, int port, int maxPlayers)
    {
        this.name = name;
        this.description = description;
        this.port = port;
        this.maxPlayers = maxPlayers;
        this.mapID = 0;
        this.gamemodeID = 0;
    }

    /// <summary>
    /// Fully customizated server data
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="port"></param>
    /// <param name="maxPlayers"></param>
    /// <param name="currentMapID"></param>
    /// <param name="currentGamemode"></param>
    /// <param name="inRandomOrder"></param>
    public ServerData(string name, string description, int port, int maxPlayers, int mapID, int currentGamemode)
    {
        this.name = name;
        this.description = description;
        this.port = port;
        this.maxPlayers = maxPlayers;
        this.mapID = mapID;
        this.gamemodeID = currentGamemode;
    }
}