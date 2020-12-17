using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Constants;

public class GamemodeManager : MonoBehaviour
{

    public static GamemodeManager instance;

    public Gamemode gamemode = Gamemode.Deathmatch;
    public bool isFFA = false;  // Free For All
    public Transform[] flags = null;
    public Transform[] hills = null;

    [Header("Stats")]
    public int[] teamScore = null;
    public int[] playerKills = null;
    public int[] playerScore = null;
    public int[] playerDeaths = null;


    //Pre-Setup, Server also starts in awake so any data requiring server info is after Awake()
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        //Output Gamemode Details to Log
        System.Console.WriteLine("Current Gamemode: " + gamemode.ToString());
        
    }

    private void Start()
    {
        //gamemode = NetworkManager.instance.gamemode;
        gamemode = (Gamemode)Server.serverData.gamemodeID;

        //Setup Gamemode based on given ID
        switch (gamemode)
        {
            case Gamemode.Deathmatch:
                break;
            case Gamemode.CaptureTheFlag:
                Setup_CTF();
                break;
            case Gamemode.KingOfTheHill:
                Setup_KoTH();
                break;
        }
    }

    public void StartGamemode()
    {
        //Setup statistic tracking with int being player IDs
        int totalPlayers = Server.serverData.maxPlayers;
        teamScore = new int[totalPlayers];
        playerKills = new int[totalPlayers];
        playerScore = new int[totalPlayers];
        playerDeaths = new int[totalPlayers];
    }

    private void Setup_CTF()
    {
        
        //If flags are not given search level for them
        if (flags.Length <= 0) flags = Helpers.FindTransformByTag("Flag");
    }

    private void Setup_KoTH()
    {
        //If hills are not given search level for them
        if (hills.Length <= 0) flags = Helpers.FindTransformByTag("Hill");
    }
}
