using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Constants;

/// <summary>
/// General script that manages the scene in relation to server objects
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

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

        //Output Server Details to Log once level is loaded
        System.Console.Clear();
        Server.serverData.VerboseServerInfo();
        System.Console.WriteLine("Loaded map: " + UnityEngine.SceneManagement.SceneManager.GetSceneAt(Server.serverData.mapID+1).name);

        //If there are no specified spawnpoints in manager, attempt to get all points by tag
        if(spawnPoints.Length <= 0) spawnPoints = Helpers.FindTransformByTag("SP");
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