using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayStatus : MonoBehaviour, IBossEventListener
{
    // Events to control camera movement and why not something else!
    public UnityEvent Intro, Choice, AttackLeft, AttackRight, EndEncounter;

    public Boss boss;
    public bool alwaysLeftDebugBool;
    public PopUpMessage popUpMsg;
    private bool bossDead;
    private bool startRound;
    public int prizePool;
    public int playerCount;

    float spawnPoint_x;
    float spawnPoint_z;
    private Vector3 moveDestination;

    public PlayerPool playerPool; // storing and controlling PlayerObject's 3D models here.
    public static PlayStatus instance;

    public PlayerObject playerObject;
    public PlayerController playerControl;
    public BossDialog bossDialog;
    public BossDialog bossDialogWhenPlayerDead;
    public GameObject modelMass;
    public Transform wp_mid;
    public Transform wp_start;
    public Transform previewPoint;
    public Transform previewLookAtPoint;

    public List<PlayerObject> players = new List<PlayerObject>();
    public List<PlayerObject> deadPlayers = new List<PlayerObject>();

    // HAAMUILU
    public bool playerDead = false;
    public HauntingManager hauntingManager;
    private List<PlayerObject> frontlineList = new List<PlayerObject>();

    // for storing player choices
    List<PlayerObject> left = new List<PlayerObject>();
    List<PlayerObject> right = new List<PlayerObject>();

    //Bonus Stage Igorin lis?ys
    [SerializeField]
    bool BonusStage = false;
    public GameObject BonusStageAskerobj;

    // Makes copy of playerobject (scriptable object) with given parameters
    PlayerObject AddPlayer(int ID, int honor, int gold, string name, GameObject model, WieldedWeapon weapon)
    {
        var po = Instantiate(playerObject);

        po.playerObj.weapon = weapon;
        po.playerObj.ID = ID; // specific ID in game
        po.playerObj.honor = honor;
        po.playerObj.goldCoins = gold;
        po.playerObj.name = name;
        po.playerObj.model = model;
        po.playerObj.isDead = false;

        return po;
    }

    void Awake()
    {
        instance = this;
        boss.listener = this;
        boss.playStatus = this;

        // Conjure players (place holder)
        for (int i = 0; i < playerCount; i++)
            players.Add(AddPlayer(i, 100 - i, 1000, "player " + i.ToString(), modelMass, playerControl.ApplyWeapon(1, 1, 0, WieldedWeapon.ElementEnum.Normal, false)));

        players.Add(AddPlayer(playerCount, 1, 1000, "myPlayer", modelMass, playerControl.ApplyWeapon(2, 5, 10, WieldedWeapon.ElementEnum.Fire, true)));
        playerControl.instanceID = playerCount;

        // Sort players by honor amount
        players = players.OrderByDescending(o => o.playerObj.honor).ToList();

        // Gather the player models in pool for activation/deactivation
        playerPool.PoolPlayerModels(players);


    }
    void Start()
    {
        // Event listeners
        GameManager.global.StartAttack.AddListener(Fight);

        // start the game loop
        Intro.Invoke();
        startRound = true;
        Invoke(nameof(ResetRound), 7f);
    }

    void AskChoices()
    {
        int myPlayerIndex = FindMyPlayerIndex();

        Choice.Invoke();

        for (int i = 0; i < players.Count; i++)
        {
            // TODO: Place power up resets to somewhere where they make sense
            // Reset previous power ups.
            players[i].playerObj.godMode = false;
            players[i].playerObj.sendPowerUpToFight = false;
            // Reset the player model if for example henchman is used
            if (players[i].playerObj.powerUpGameObject != null)
            {
                Destroy(players[i].playerObj.powerUpGameObject);
            }



            if (!players[i].playerObj.isDead && i != myPlayerIndex)
                playerControl.BotChoice(i);
            if (i == myPlayerIndex)
            {
                StartCoroutine(playerControl.ChoiceScreen(myPlayerIndex, players[myPlayerIndex].playerObj.isDead));
            }

        }
    }

    public void ReceiveChoices(int index, int _choice)
    {
        if (_choice == 1)
            left.Add(players[index]);

        if (_choice == 2)
            right.Add(players[index]);

        players[index].playerObj.choice = _choice;
    }

    // This integer is only used to store the last random number that decides whitch side gets killed
    int bossSideSelection;

    void Fight()
    {
        Debug.Log(left.Count + " players in left, " + right.Count + " players in right.");
        PlayerAttackMovement();

        int myPlayerIndex = FindMyPlayerIndex();
        if (!players[myPlayerIndex].playerObj.isDead)
            if (players[myPlayerIndex].playerObj.choice == 1)
            {
                AttackLeft.Invoke();
            }

            else
            {
                AttackRight.Invoke();
            }



        // MoveBoss can take a optional callback parameter to activate the attack function.
        // Attack won't happen if there is only two parameters passed to the function.
        StartCoroutine(boss.MoveBoss(wp_mid.position, 2.5f, callback =>
        {
            if (callback != null)
            {
                bossSideSelection = callback;
            }
        }));
        //Invoke(nameof(BossAttack), 4f);

    }
    void PlayerAttackMovement()
    {
        float z = 0;
        float x = 0;

        // movement
        for (int i = 0; i < players.Count; i++)
        {
            if (playerPool.playerObjects[i].activeInHierarchy)
            {
                if (players[i].playerObj.choice == 1)
                {
                    x = Random.Range(5f, 7f);
                    z = Random.Range(-2f, 2f);

                    moveDestination = new Vector3(x, 1.17f, z);

                    if (players[i].playerObj.sendPowerUpToFight)
                    {
                        playerControl.StartMoving(players[i].playerObj.powerUpGameObject, moveDestination, (float)Random.Range(3.0f, 4f), boss.bossObject, true);
                    }
                    else
                    {
                        playerControl.StartMoving(playerPool.playerObjects[i], moveDestination, (float)Random.Range(3.0f, 4f), boss.bossObject, true);
                    }
                }
                else
                {
                    x = Random.Range(-5f, -7f);
                    z = Random.Range(-2f, 2f);

                    moveDestination = new Vector3(x, 1.17f, z);

                    if (players[i].playerObj.sendPowerUpToFight)
                    {
                        playerControl.StartMoving(players[i].playerObj.powerUpGameObject, moveDestination, (float)Random.Range(3.0f, 4f), boss.bossObject, true);
                    }
                    else
                    {
                        playerControl.StartMoving(playerPool.playerObjects[i], moveDestination, (float)Random.Range(3.0f, 4f), boss.bossObject, true);
                    }
                }
            }
        }
    }

    void ResetRound()
    {
        left.Clear();
        right.Clear();

        // new round or declare winners?
        if (!bossDead && players.Count - deadPlayers.Count > 0)
        {
            StartCoroutine(boss.MoveBoss(wp_start.position, 2f));
            SetupPlayerModels();
            AskChoices();
        }
        else
        {
            EndEncounter.Invoke();
            Debug.Log("THE RAIDING PARTY HAS BEEN DEFEATED!");
            SceneManager.LoadScene("Map");
        }
        // else LOSE
    }
    void SetupPlayerModels()
    {
        // Defining first spawn point
        if (players.Count - deadPlayers.Count < 10)
            spawnPoint_x = (players.Count - deadPlayers.Count) - 2f;
        else
            spawnPoint_x = 4.5f;
        spawnPoint_z = 11f;

        // Activate front line player models
        int myPlayerIndex = FindMyPlayerIndex();
        playerPool.ActivatePlayerModels(players, myPlayerIndex);

        // Move active player models to start positions
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].playerObj.isDead)
            {
                if (playerPool.frontLineObjectIndexes.Contains(i))
                {
                    // front line position
                    if (startRound)
                    {
                        playerPool.playerObjects[i].transform.position = new Vector3(spawnPoint_x, 1.17f, spawnPoint_z);
                        spawnPoint_x -= 1;
                    }
                    else
                    {
                        playerControl.StartMoving(playerPool.playerObjects[i], new Vector3(spawnPoint_x, 1.17f, spawnPoint_z), 2f, boss.bossObject, true);
                        spawnPoint_x -= 1;
                    }

                }
                else
                {
                    // back line position
                    if (startRound)
                        playerPool.playerObjects[i].transform.position = new Vector3((Random.Range(9.0f, -9.0f)) * (players.Count - deadPlayers.Count) / 100, 1.17f, Random.Range(13.0f, 14.0f));
                    else
                        playerControl.StartMoving(playerPool.playerObjects[i], new Vector3((Random.Range(9.0f, -9.0f)) * (players.Count - deadPlayers.Count) / 100, 1.17f, Random.Range(13.0f, 14.0f)), 2f, boss.bossObject, true);

                }

                // my player position if not in front line
                if (!playerPool.frontLineObjectIndexes.Contains(myPlayerIndex))
                {
                    if (startRound)
                        playerPool.playerObjects[myPlayerIndex].transform.position = new Vector3(0, 1.17f, 12f);
                    else
                        playerControl.StartMoving(playerPool.playerObjects[myPlayerIndex], new Vector3(0, 1.17f, 12f), 2f, boss.bossObject, true);
                }

                playerPool.playerObjects[i].transform.LookAt(boss.bossObject.transform);
            }
        }

        // Reset haunting damage and give Haunting script the first player (for now, only testing to see if ghosting damage multiplier applies)
        hauntingManager.ChangeHauntingTarget(players[playerPool.frontLineObjectIndexes[0]]);
        hauntingManager.hauntingTarget.playerObj.hauntingDamageBonus = 0;

        // Refresh the new frontline player list in haunting
        // List for ghosting
        frontlineList.Clear();
        for (int i = 0; i < playerPool.frontLineObjectIndexes.Count; i++)
            frontlineList.Add(players[playerPool.frontLineObjectIndexes[i]]);

        hauntingManager.UpdateFrontlineList(frontlineList);

        startRound = false;
    }
    public void BossAttack()
    {
        // boss chooses left/right
        if (bossSideSelection <= 50 || alwaysLeftDebugBool) // <= 50
        {
            KillPlayers(left);
            DamageByPlayers(right);
        }

        else
        {
            KillPlayers(right);
            DamageByPlayers(left);
        }



        // K?yd??n l?pi kuolleet pelaajat
        if (playerDead == false)
        {

            int listLength = deadPlayers.Count();

            for (int i = 0; i < deadPlayers.Count(); i++)
            {
                // Tsekataan onko oma pelaaja kuollut
                if (deadPlayers[i].playerObj.ID == playerControl.instanceID)
                {
                    playerDead = true;
                }
            }
            if (playerDead == true)
            {
                // Aloitetaan haamuilu x ajan kuluttua!
                // HUOM!! Haamuilu aloitetaan vain kerran peliss?, kun pelaaja kuolee ensimm?isen kerran! WAND OF THE NECROMANCER EI VIEL? OLEMASSA
                hauntingManager.StartHaunting(1f);
            }
        }

        // Reset run
        Invoke(nameof(ResetRound), 2f);
    }

    public int FindMyPlayerIndex()
    {
        // Find my player position in players List
        for (int i = 0; i < players.Count; i++)
            if (players[i].playerObj.ID == playerControl.instanceID)
                return i;

        return -1;
    }

    public void KillPlayers(List<PlayerObject> leftRight)
    {
        if (leftRight != null)
        {

            for (int i = 0; i < leftRight.Count; i++)
            {
                for (int j = players.Count - 1; j >= 0; j--)
                {
                    if (leftRight[i].playerObj.ID == players[j].playerObj.ID)
                    {
                        if (!players[j].playerObj.godMode && !players[j].playerObj.isDead)
                        {
                            players[j].playerObj.isDead = true;
                            deadPlayers.Add(players[j]);
                            playerPool.DeactivateDeadPlayers(j);
                        }
                        else // Protective power up?
                        {

                        }
                    }
                }
            }
        }


        playerControl.UpdateTeamHp();
    }

    public void DamageByPlayers(List<PlayerObject> leftRight)
    {
        // upcoming damage to boss
        int damageToBoss = 0;
        int playerDamage = 0;
        // place holder. execute boss.animation function here
        //boss.transform.forward = new Vector3(5, 0, 0);

        // award honor for right side players
        if (leftRight != null)
        {
            for (int i = 0; i < leftRight.Count; i++)
            {
                playerDamage = playerControl.DamageEnemy(leftRight[i]);
                damageToBoss += playerDamage;
                leftRight[i].playerObj.damageDone += playerDamage;
            }
        }

        Debug.Log("Boss takes " + damageToBoss + " damage!");
        boss.ManageHealth(-damageToBoss);
    }

    public void BossDied(Boss boss)
    {
        Destroy(boss);
        bossDead = true;

        //map thing (igors code)
        if (BonusStage == true)
        {
            GameManager.global.MaxPlayersInBonusStage = players.Count;
            BonusStageAskerobj.SetActive(true);
            BonusStageAskerobj.GetComponent<BonuStageAsker>().DeactivateOtherObjects();
        }
        else
        {
            GameManager.global.levelsUnlocked[GameManager.global.currentLevelIndex] = 1;

            SceneManager.LoadScene("Map");
        }

    }
    void DisplayFrontLine()
    {
        string message = "FRONTLINE PLAYERS: ";

        for (int i = 0; i < playerPool.frontLineObjectIndexes.Count; i++)
        {
            message += players[playerPool.frontLineObjectIndexes[i]].playerObj.name + ", ";
        }

        Debug.Log(message);
    }
}
