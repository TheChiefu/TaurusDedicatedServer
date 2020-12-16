using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Constants;

/// <summary>
/// Setup class that initalizes server on startup and sends data to Server.cs class
/// As well as manages spawning items in server
/// </summary>
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    [Tooltip("For settings up Server.cs settings for or scripts to read")]
    [Header("Server Settings")]
    public int maxPlayers = 8;
    public int port = 2500;
    public string serverName = string.Empty;
    public string serverDescription = string.Empty;
    public string serverBannerURL = string.Empty;

    [Space(5)]
    public Gamemode gamemode = Gamemode.Deathmatch;

    [Header("Game Objects")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;
    public Transform[] spawnPoints;

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

        //If there are no specified spawnpoints in manager, attempt to get all points by tag
        if(spawnPoints.Length <= 0) spawnPoints = Helpers.FindTransformByTag("SP");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        ServerData sd = new ServerData(serverName, serverDescription, port, maxPlayers);

        Server.Start(sd);
    }


    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public void InstantiateEnemy(Vector3 _position)
    {
        Instantiate(enemyPrefab, _position, Quaternion.identity);
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }
}