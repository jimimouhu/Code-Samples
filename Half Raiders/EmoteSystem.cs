using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteSystem : MonoBehaviour
{
    public PlayStatus playStatus;
    public PlayerPool playerObjectPool;
    [HideInInspector]
    List<GameObject> playerObjects = new List<GameObject>();
    List<GameObject> massObjects = new List<GameObject>();

    public List<GameObject> emoteSprites;

    float spamTimer = 5;
    bool menuIsOpen = false;

    public enum Emotes
    {
        Laugh,
        OMG,
        Left,
        Right,
        Shit
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(spamTimer < 5)
        {
            spamTimer += Time.deltaTime;
        }
    }

    public void ToggleEmoteMenu()
    {

        if (transform.GetChild(transform.childCount - 2).GetComponent<Button>().interactable == menuIsOpen)
        {
            menuIsOpen = !menuIsOpen;

            for (int i = 0; i < transform.childCount - 1; i++)
            {
                transform.GetChild(i).GetComponent<EmoteButtonScript>().Toggle(menuIsOpen);
            }
        }
    }

    public void TriggerEmote(Emotes emote)
    {
        if (spamTimer > 4)
        {
            spamTimer = 0;
            // Some server stuff beep boop

            //vvvvvv Placeholder code vvvvvvv
            int playerIndex = playStatus.FindMyPlayerIndex();
            ReceiveEmoteFromServer((int)emote, playerIndex);
            //^^^^^^ Placeholder code ^^^^^^^
        }

    }

    public void ReceiveEmoteFromServer(int emote, int playerIndex)
    {

        // MOAR server stuff beep boop
        for (int i = 0; i < playStatus.players.Count; i++)
        {
            playerObjects.Add(playStatus.playerPool.playerObjects[i]);
        }

        massObjects = playStatus.playerPool.playerObjects;

        if (playerIndex < 10)
        {
            GameObject emoteSprite = Instantiate(emoteSprites[emote], playerObjects[playerIndex].transform.position + Vector3.up * 3, Quaternion.identity, playerObjects[playerIndex].transform);
            Destroy(emoteSprite, 3);
        }else if(playerIndex == playStatus.FindMyPlayerIndex())
        {
            GameObject emoteSprite = Instantiate(emoteSprites[emote], playerObjects[playerIndex].transform.position + Vector3.up * 3, Quaternion.identity, playerObjects[playerIndex].transform);
            Destroy(emoteSprite, 3);
        }
        else
        {
            //This is randomized in case there are more players than mass objects:
            int randomInt = Random.Range(0, massObjects.Count);
            GameObject emoteSprite = Instantiate(emoteSprites[emote], playerObjects[randomInt].transform.position + Vector3.up * 3, Quaternion.identity, massObjects[randomInt].transform);
            Destroy(emoteSprite, 3);
        }
    }
}
