using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Mode Object - Hill
public class GMO_Hill : MonoBehaviour
{

    [SerializeField] private int scoringTeamID = -1;
    [SerializeField] private List<Player> scoringPlayers = new List<Player>();
    [SerializeField] private bool isNeutral = true;

    private void Start()
    {
        StartCoroutine(Score(1f));
    }

    //Determine scoring team/player based on who is in already in/out of the hill
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            scoringPlayers.Add(player);

            //If no players are here, make current player's team / or self start scoring
            if (scoringPlayers.Count <= 1)
            {
                isNeutral = false;

                //Set scoring team when possible
                if (!GamemodeManager.instance.isFFA) scoringTeamID = player.teamID;
            }

            //For multiple players on hill
            else
            {
 
                //If FFA set point to neutral
                if(GamemodeManager.instance.isFFA) SetHillNeutral();

                //If not a FFA check for team conflict
                else
                {
                    //If two or more players are from different teams set point to neutral
                    if (player.teamID != scoringTeamID) SetHillNeutral();
                }      
            }
        }
    }


    // If the last player leaves the point becomes neutral
    // If a player leaves point goes to last player's team or self score
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            scoringPlayers.Remove(player);

            //Set point back to neutral if no player is left
            if (scoringPlayers.Count <= 0) SetHillNeutral();

            //For 1 player on hill
            else if (scoringPlayers.Count == 1)
            {
                if (!GamemodeManager.instance.isFFA) scoringTeamID = player.teamID;
                isNeutral = false;
            }

            //For 2 or more players
            else
            {
                //If FFA enabled set to neutral
                if (GamemodeManager.instance.isFFA) SetHillNeutral();
                else
                {
                    //If players are from different teams set to neutral
                    if (player.teamID != scoringTeamID) SetHillNeutral();
                }
            }

        }
    }


    //Gives a point to the player in the hill and the team score if nescessary
    private IEnumerator Score(float waitInterval)
    {

        //Keep coroutine from stopping
        while(Application.isPlaying)
        {
            if (!isNeutral)
            {
                //Add score to every player in Hill
                foreach (Player player in scoringPlayers)
                {
                    GamemodeManager.instance.playerScore[player.id]++;
                }

                //Add team score
                if (!GamemodeManager.instance.isFFA)
                {
                    GamemodeManager.instance.teamScore[scoringTeamID]++;
                }

            }

            yield return new WaitForSeconds(waitInterval);
        }
    }


    private void SetHillNeutral()
    {
        scoringTeamID = -1;
        isNeutral = true;
    }
    
}
