using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Player : MonoBehaviour
{
    //[SerializeField] private int Difficulty = 0; // 0 Lowest <---> 5 Highest
    [SerializeField] private State currentState;


    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Search:
                break;
            case State.Score:
                break;
            case State.Chase:
                break;
            case State.Attack:
                break;

            default:
                break;
        }

    }


    // Get gamemode type and determine what to do
    // For Deathmatch, find nearest player to attack
    // For Single Capture the Flag, find the flag
    // For Dual Capture the Flag, find enemy flag
    // For King of the Hill, find the point
    private void Search(int gamemodeID)
    {

    }







    //States AI Player can be in.
    // Idle - Just stands still doing nothing
    // Search - Depending on gamemode either searches for enemy players or objectives
    // Score - If objective based mode, attempt to score (i.e Capture Flag, stand on capture point, etc)
    // Chase - Attempts to get closer to enemy players to attack
    // Attack - As name implies attack enemy players
    private enum State
    {
        Idle,
        Search,
        Score,
        Chase,
        Attack
    }

}
