using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayStatus : MonoBehaviour
{

    private bool counting;
    public int prizePool;
    public int bossRounds;
    public int playerCount;
    int spawnPoint_x;
    int spawnPoint_z;
    public PlayerObject player;
    PlayerController playerControl;
    Camera cam;
    public GameObject modelP;
    public GameObject modelNPC;
    public GameObject modeljim;

    public Transform wp_left;
    public Transform wp_right;
    public Transform wp_mid;
    public Transform wp_start;

    // for displaying player models only
    List<GameObject> playerObjects = new List<GameObject>();
    // for storing data of players, player.playerObj.model ---> List<GameObject> playerObjects
    List<PlayerObject> players = new List<PlayerObject>();

    List<PlayerObject> deadPlayers = new List<PlayerObject>();

    // for storing player choices
    List<PlayerObject> left = new List<PlayerObject>();
    List<PlayerObject> right = new List<PlayerObject>();
    


    // Makes copy of playerobject (scriptable object) with given parameters
    PlayerObject AddPlayer(int ID, int honor, int gold, string name, GameObject model)
    {
        var po = Instantiate(player);

        po.playerObj.ID = ID; // specific ID in game
        po.playerObj.honor = honor;
        po.playerObj.goldCoins = gold;
        po.playerObj.name = name;
        po.playerObj.model = model;

        return po;
    }

    void Start()
    {
        playerControl = GameObject.Find("PlayerControl").GetComponent<PlayerController>();
        cam = Camera.main;
        
        // test test test, get this ID from server list index
        playerControl.instanceID = (int)Random.Range(0, playerCount);

        // add npcs (place holder)
        for (int i = 0; i < playerCount; i++)
        {
            if (i == playerControl.instanceID) 
            {
                players.Add(AddPlayer(i, 1, 1000, "jimi" + i.ToString(), modeljim));

            } 
            else players.Add(AddPlayer(i, Random.Range(1, 200), 1000, "player " + i.ToString(), modelP));
        }




        // start the game loop
        FrontLineSetup();
    }

    void Update()
    {
        // start fight when ready counting
        if (left.Count + right.Count == players.Count && counting)
        {
            Debug.Log(left.Count + " players in left");
            Debug.Log(right.Count + " players in right");
            counting = false;
            Fight();
        }
    }

    // order players by honor in list and display in frontline
    void FrontLineSetup()
    {
        Debug.Log("Boss rounds remaining: " + bossRounds);
        StartCoroutine(MoveBoss(wp_start.position, 2f));

        if (playerObjects.Count != 0)
            foreach (GameObject playerObj in playerObjects)
                Destroy(playerObj);
        playerObjects.Clear();

        players = players.OrderByDescending(o => o.playerObj.honor).ToList();

        StartCoroutine(RotateCamera(new Vector3(30, 110, 0)));

        // Defining first spawn point
        if (players.Count < 10) spawnPoint_x = players.Count - 1;
        else spawnPoint_x = 9;
        spawnPoint_z = 10;



        // display players alive and frontline
        Debug.Log(players.Count + " players alive!");
        int playersListSize = players.Count;
        if (playersListSize > 10) playersListSize = 10;
        
        ListDisplayer(players, "FRONTLINE PLAYERS: ", playersListSize);

        for (int i = 0; i < playersListSize; i++)
        {
            playerObjects.Add(Instantiate(players[i].playerObj.model, new Vector3(spawnPoint_x, 1.75f, spawnPoint_z), Quaternion.Euler(0, 180, 0)));
            spawnPoint_x -= 2;
        }

        // Spawn player in back line if not in front line
        for (int i = 10; i < players.Count; i++)
        {
            if (players[i].playerObj.ID == playerControl.instanceID)
            {
                playerObjects.Add(Instantiate(players[i].playerObj.model, new Vector3(0, 1.75f, 12), Quaternion.Euler(0, 180, 0)));
            }
        }

        // new round or declare winners?
        if (bossRounds > 0 && players.Count > 0) 
            AskChoices();
        else
        {
            if (players.Count > 0)
            {
                ListDisplayer(players, "GAME OVER! SURVIVORS: ", players.Count);
                Debug.Log("Survivors receive " + prizePool / players.Count + " each!");
                foreach (PlayerObject player in players)
                {
                    player.playerObj.goldCoins += prizePool / players.Count;
                    Debug.Log("Player name: " + player.playerObj.name + ", Total Coins: " + player.playerObj.goldCoins + " Honor: " + player.playerObj.honor);
                }
            } else Debug.Log("GAME OVER! NO SURVIVORS!");
        } 
    }

    void AskChoices()
    {
        counting = true;

        for (int i = 0; i < players.Count; i++)
        { 
            if (players[i].playerObj.ID == playerControl.instanceID)
            {
                // show choose buttons
                playerControl.StartChoise(i);
            } else
            {
                // for bots
                playerControl.StartChoosing(i);
            }

        }
    }

    public void ReceiveChoices(int index, int _choice)
    {
        if (_choice == 0)
        {
            int rnd = Random.Range(0, 2);
            if (rnd == 0)
            {
                left.Add(players[index]);
                players[index].playerObj.choice = 1;
            }
            else
            {
                right.Add(players[index]);
                players[index].playerObj.choice = 2;
            }
        }
        if (_choice == 1)
        {
            left.Add(players[index]);
            players[index].playerObj.choice = _choice;
        }
        if (_choice == 2)
        {
            right.Add(players[index]);
            players[index].playerObj.choice = _choice;
        }

    }

    void Fight()
    {
        int z = 0;
        int z2 = 0;
        int x = 0;
        int x2 = 0;


        //front line movement
        for (int i = 0; i < playerObjects.Count; i++)
        {
            // player not in front line?
            if (i == 10)
            {
                for (int j = 10; j < players.Count; j++)
                    if (players[j].playerObj.ID == playerControl.instanceID)
                        if (players[j].playerObj.choice == 1) 
                        {
                            Vector3 original = wp_left.position;
                            original += new Vector3(x, 0, z);
                            playerControl.StartMoving(playerObjects[10], original, (float)Random.Range(2.0f, 3.0f), gameObject); 
                        }

                        else
                        {
                            Vector3 original = wp_right.position;
                            original += new Vector3(x2, 0, z2);
                            playerControl.StartMoving(playerObjects[10], original, (float)Random.Range(2.0f, 3.0f), gameObject);
                        }

            } else
            {
                if (players[i].playerObj.choice == 1)
                {
                    Vector3 original = wp_left.position;
                    original += new Vector3(x, 0, z);
                    playerControl.StartMoving(playerObjects[i], original, (float)Random.Range(2.0f, 3.0f), gameObject);
                    Debug.Log(original);


                    if (z < -5)
                    {
                        z = 0;
                        x = 1;
                    }
                    else
                        z -= 1;
                }

                else
                {
                    Vector3 original = wp_right.position;
                    original += new Vector3(x2, 0, z2);
                    playerControl.StartMoving(playerObjects[i], original, (float)Random.Range(2.0f, 3.0f), gameObject);
                    Debug.Log(original);
                    if (z2 < -5)
                    {
                        z2 = 0;
                        x2 = -1;
                    }
                    else
                        z2 -= 1;
                }
            }
        }



        Debug.Log(Camera.main.transform.rotation.eulerAngles);
        StartCoroutine(RotateCamera(new Vector3(30, 130, 0)));
        StartCoroutine(MoveBoss(wp_mid.position, 4f));
        StartCoroutine(BossAttack());


    }
    IEnumerator BossAttack()
    {
        yield return new WaitForSeconds(4);
        

        // boss chooses left/right
        switch ((int)Random.Range(1, 3))
        {
            case 1:
                Debug.Log("Boss attacks left!");
                transform.forward = new Vector3(5, 0, 0);

                // award honor
                foreach (PlayerObject player in right)
                    player.playerObj.honor += 100 - (bossRounds * 10);
                Debug.Log("Alive frontline players awarded " + (100 - (bossRounds * 10)) + " honor!");

                // add dying players to deadPlayers list
                for (int i = 0; i < left.Count; i++)
                {
                    for (int j = 0; j < players.Count; j++)
                    {
                        if (left[i].playerObj.name == players[j].playerObj.name)
                        {
                            deadPlayers.Add(players[j]);
                        }
                    }
                }
                for (int i = 0; i < deadPlayers.Count; i++)
                {
                    for (int j = 0; j < players.Count; j++)
                    {
                        if (players[j].playerObj.ID == deadPlayers[i].playerObj.ID)
                        {
                            players.RemoveAt(j);
                            j--;
                        }

                    }
                }
                break;

            case 2:
                Debug.Log("Boss attacks right!");
                transform.forward = new Vector3(-5, 0, 0);
                // award honor
                foreach (PlayerObject player in left)
                    player.playerObj.honor += 100 - (bossRounds * 10);
                Debug.Log("Alive frontline players awarded " + (100 - (bossRounds * 10)) + " honor!");

                // add dying players to deadPlayers list
                for (int i = 0; i < right.Count; i++)
                {
                    for (int j = 0; j < players.Count; j++)
                    {
                        if (right[i].playerObj.name == players[j].playerObj.name)
                        {
                            deadPlayers.Add(players[j]);
                        }
                    }
                }
                for (int i = 0; i < deadPlayers.Count; i++)
                {
                    for (int j = 0; j < players.Count; j++)
                    {
                        if (players[j].playerObj.ID == deadPlayers[i].playerObj.ID)
                        {
                            players.RemoveAt(j);
                            j--;
                        }

                    }
                }
                break;
        }
        Invoke(nameof(ResetRun), 3f);
    }

    IEnumerator RotateCamera(Vector3 rotation)
    {
        Vector3 basicRot = cam.transform.eulerAngles;
        float duration = 2f;
        float time = 0;
        while (time < duration)
        {
            cam.transform.eulerAngles = Vector3.Lerp(basicRot, rotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator MoveBoss(Vector3 pos, float duration)
    {
        transform.forward = new Vector3(0, 0, 0);
        Vector3 basicPos = transform.position;
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(basicPos, pos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
    void ResetRun()
    {
        left.Clear();
        right.Clear();
        bossRounds--;
        Invoke(nameof(FrontLineSetup), 2f);
    }

    void ListDisplayer(List<PlayerObject> list, string message, int rangeMax)
    {
        for (int i = 0; i < rangeMax; i++)
            message += " " + list[i].playerObj.name;

        Debug.Log(message);
    }
}
