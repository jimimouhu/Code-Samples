using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPool : MonoBehaviour
{
    public Transform alivePlayerObjectsParent;
    public Transform deadPlayerObjectsParent;
    public List<GameObject> playerObjects;
    public List<int> frontLineObjectIndexes; // this list keeps up with information of current front line players. The int variable = index in players -list.
    public int activeModelsAmount = 0;
    public int activeMassAmount = 0;
    private int maxActivatedModels = 10;


    public void PoolPlayerModels(List<PlayerObject> players)
    {
        // Fill the player model pool
        for (int i = 0; i < players.Count; i++)
        {
            GameObject temp;
            temp = Instantiate(players[i].playerObj.model, alivePlayerObjectsParent);
            temp.SetActive(false);
            playerObjects.Add(temp);

        }
    }

    public void ActivatePlayerModels(List<PlayerObject> players, int myPlayerIndex)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].playerObj.isDead)
            {               
                // front line
                if (activeModelsAmount < maxActivatedModels && !frontLineObjectIndexes.Contains(i))
                {

                    frontLineObjectIndexes.Add(i);
                    activeModelsAmount++;
                    playerObjects[i].SetActive(true);
                    // ++ change front line player's mesh to their personal character

                }
                // back line
                else
                {
                    // ++ mesh = mass model
                    playerObjects[i].SetActive(true);
                }
            }         
        }
    }

    public void DeactivateDeadPlayers(int playerIndex)
    {
        playerObjects[playerIndex].SetActive(false);
        playerObjects[playerIndex].transform.parent = deadPlayerObjectsParent;
        if (frontLineObjectIndexes.Contains(playerIndex))
        {
            frontLineObjectIndexes.Remove(playerIndex);
            activeModelsAmount--;
        }
    }
}
