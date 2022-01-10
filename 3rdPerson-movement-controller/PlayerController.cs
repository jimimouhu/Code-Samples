using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPunCallbacks
{
    //Debug:
    public bool GOD_MODE = true;
    public GameObject dashParticle;
    public GameObject leftHandItemSlot;
    public GameObject rightHandItemSlot;
    public GameObject landingPoint;
    public GameObject visible;
    public GameObject invisible;
    public GameObject[] spells = new GameObject[8];
    public ParticleSystem chargeEffect;
    GameObject map;
    GameObject uiSkills;
    public Item.ItemType leftHandItemType;
    public string objectInLeftHand;
    public float itemDamage;

    public static PlayerController instance;
    public CharacterController controller;
    ActionBarController[] bars = new ActionBarController[8];
    CinemachineSens camSens;
    Crosshair[] xhair = new Crosshair[2];
    CinemachineFreeLook cam;

    Transform camTransform;
    public Transform groundCheck;
    public Transform attackArea;
    public Transform lookAt;

    public LayerMask groundMask;
    public LayerMask enemy;
    public LayerMask tree;

    float spellCastLevel;
    float mass = 3.0F; // defines the character mass
    public float fallPosition;
    public float fallVelocity = 0;
    public float groundDistance = 0.4f;
    public float speed = 6.0f;
    public float sprintspeed = 12.0f;
    public float jumpHeight = 3.0f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public float attackRange;
    public float targetFov; // camera fov
    public float restTimer;
    public float invisibleTimer = 5;
    public float damage;
    public float doDamage = 10; // place holder
    private readonly float gravity = -9.81f;
    private float angle;
    private float targetAngle;
    float rollTimer = 10;
    float rollSpeed = 20;
    float energyRegenWait;

    public Animator animation; // Animaattori

    Vector3 impact = Vector3.zero;
    Vector3 velocity;
    Vector3 moveDir;
    Vector3 aim = new Vector3(1.0f, 0.4f, 0);
    Vector3 normal = new Vector3(0.0f, 0.4f, 0);
    Vector3 areaOfAttack = new Vector3(1, 1.7f, 1.5f);

    bool[] spellsActivated = new bool[8];
    bool mapEnabled;
    bool dead;
    bool invisible_running;
    bool isGrounded;
    bool sprinting;
    bool moving;
    public bool charging; //spell
    public bool aiming;
    public bool castReady;
    bool hitting = false;
    bool resting = false;
    bool parry = false;
    public bool uiOn;
    bool spawnDrop = true;

    // GUI Images + HP/Energy variables
    Image GuiHp;
    Image GuiEnergy;



    [SerializeField] public float hp_fill;
    [SerializeField] [Range(0, 10)] float energy_fill;

    // Action Baarin keyt
    private readonly KeyCode[] actionBarKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8 };

    //INVENTORY TARVITSEE 1/3:
    public Inventory inventory;
    GameObject uiInventory;
    //_______^_INVENTORY_^__________

    //BUILD MENU TARVITSEE 1/3:
    GameObject uiBuildMenu;
    //____^_BUILDMENU_^_____

    //PICKUP TARVITSEE 1/:
    public float pickupDistance = 1;
    public LayerMask pickupLayer;
    bool nearPickupable;
    string objectName;


    private void OnGUI()
    {
        if (nearPickupable)
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200, 200), "Press \"e\" to pick up " + objectName);
    }
    //________^_PICKUP_^_____________

    private void Awake()
    {
        GuiHp = GameObject.Find("fillbar_health").GetComponent<Image>();
        GuiEnergy = GameObject.Find("fillbar_energy").GetComponent<Image>();

        cam = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();
        camTransform = GameObject.Find("Main Camera").GetComponent<Transform>();
        camSens = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineSens>();

        if (photonView.IsMine)
        {
            for (int a = 0; a < bars.Length; a++)
            {
                bars[a] = GameObject.Find("bar" + a.ToString()).GetComponent<ActionBarController>();
                bars[a].GetComponent<Image>().fillAmount = 0;
                bars[a].enabled = false;
                spellsActivated[a] = false;
            }
            rightHandItemSlot.SetActive(false);
        }


    }

    void Start()
    {
        if (photonView.IsMine)
            if (photonView.IsMine)
            {
                leftHandItemType = Item.ItemType.Empty;
                //BUILD MENU TARVITSEE 2/3 (Oltava Startissa):
                uiBuildMenu = GameObject.Find("Canvas").transform.Find("UI_BuildMenu").gameObject;
                //_____________^_BUILD MENU_^____________________

                //INVENTORY TARVITSEE 2/3 (Oltava Awakessa):
                uiInventory = GameObject.Find("Canvas").transform.Find("UI_Inventory").gameObject;
                inventory = new Inventory();
                uiInventory.GetComponent<UI_Inventory>().SetInventory(inventory);

                //_________________________^_INVENTORY_^___________________________
                //SAVEN LOADAAMINEN TARVITSEE:
                inventory.SetItemList(GameStatus.instance.saveData.itemList);
                //------------------------------------------------------------

                photonView.Owner.TagObject = gameObject;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                targetFov = 40;
                cam.m_CommonLens = true;
                cam.Follow = this.lookAt;
                cam.LookAt = this.lookAt;
                restTimer = 1;

                for (int i = 0; i < xhair.Length; i++)
                {
                    xhair[i] = GameObject.Find("Xhair" + i.ToString()).GetComponent<Crosshair>();
                }

                photonView.Owner.TagObject = this.gameObject;
                GameStatus.instance.player = gameObject;
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("ID", photonView.ViewID);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                map = GameObject.Find("Main Camera").transform.Find("Map").gameObject;
                uiSkills = GameObject.Find("Canvas").transform.Find("UI_Skills").gameObject;
                GameStatus.instance.RefreshQuests();

            }
            else
            {
                EquipCharacter();
            }
        //fallVelocity = 0;
        //fallPosition = transform.position.y;
        //velocity.y = 0;

        //if (groundCheck)
        //{
        //    if (fallVelocity > 27 && !spawnDrop)
        //    {
        //        //Debug.Log("Rivi 395");
        //        HurtPlayer(fallVelocity / 15);
        //    }
        //    spawnDrop = false;
        //}
    }

    private void FixedUpdate()
    {
        fallVelocity = (fallPosition - transform.position.y) / Time.fixedDeltaTime;
        fallPosition = transform.position.y;
    }

    void Update()
    {

        if (photonView.IsMine)
        {
            SpellCheck();

            if (rightHandItemSlot.activeSelf == true)
            {
                animation.SetBool("NoStoneIdle", false);
            }
            else
            {
                animation.SetBool("NoStoneIdle", true);
            }



            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            // HP/Energy amount refresh
            GuiHp.fillAmount = hp_fill / GameStatus.instance.skillsAndStats.healthLevel;
            GuiEnergy.fillAmount = energy_fill / 10;

            // Minimum HP level = 10
            if (GameStatus.instance.skillsAndStats.healthLevel < 10)
            {
                GameStatus.instance.skillsAndStats.healthLevel = 10;
            }

            // Minimum HP_FILL
            if (hp_fill > GameStatus.instance.skillsAndStats.healthLevel)
            {
                hp_fill = GameStatus.instance.skillsAndStats.healthLevel;
            }


            energyRegenWait += Time.deltaTime;
            if (!sprinting && energy_fill < 10 && energyRegenWait > 2) energy_fill += (GameStatus.instance.skillsAndStats.enduranceLevel / 10) * Time.deltaTime;
            if (energy_fill < 0) energy_fill = 0;

            if (resting)
            {
                restTimer -= Time.deltaTime;
            }

            if (restTimer <= 0)
            {
                restTimer = 1;
                resting = false;
            }

            // Grounding check
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Cam fov
            cam.m_Lens.FieldOfView = Mathf.MoveTowards(cam.m_Lens.FieldOfView, targetFov, 50 * Time.deltaTime);


            if (direction.magnitude >= 0.1f && !aiming) moving = true;
            else moving = false;

            if (impact.magnitude > 0.2F) controller.Move(impact * Time.deltaTime);
            // consumes the impact energy each cycle:
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

            if (uiOn)
            {
                camSens.horizontalAimingSpeed = 0;
                camSens.verticalAimingSpeed = 0;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                camSens.horizontalAimingSpeed = camSens.sens.value;
                camSens.verticalAimingSpeed = camSens.sens.value;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (moving && !uiOn)
            {
                Cursor.lockState = CursorLockMode.Locked;
                hitting = false;
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
                animation.SetBool("Walking", true);
            }
            else
            {
                // for animation
                animation.SetBool("Walking", false);
            }

            if (aiming && !uiOn)
            {
                // Player movement
                targetAngle = camTransform.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * 0 * Time.deltaTime);
            }
            if (hitting && !uiOn)
            {
                // Player movement
                targetAngle = camTransform.eulerAngles.y;
                angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * 0 * Time.deltaTime);
            }

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            // Jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                animation.SetTrigger("Jump");
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                energyRegenWait = 0;
            }


            // Sprint
            if (sprintspeed >= 12.0f + GameStatus.instance.skillsAndStats.enduranceLevel / 10) sprintspeed = 12.0f + GameStatus.instance.skillsAndStats.enduranceLevel / 10;
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded && energy_fill > 0)
            {
                energyRegenWait = 0;

                animation.SetBool("Sprinting", true);
                sprinting = true;
                sprintspeed += 6 * Time.deltaTime;
                speed = sprintspeed;
                energy_fill -= ((100 - GameStatus.instance.skillsAndStats.enduranceLevel) / 100) * Time.deltaTime;

                //Skills
                int levelUpCheck = (int)GameStatus.instance.skillsAndStats.enduranceLevel;
                GameStatus.instance.skillsAndStats.enduranceLevel += 0.1f * Time.deltaTime;
                if (levelUpCheck < (int)GameStatus.instance.skillsAndStats.enduranceLevel)
                    GameObject.Find("Canvas").GetComponent<UI_Elements>().StartLevelUpCoroutine("Endurance +1");
            }
            else
            {
                sprintspeed = 6.0f;
                animation.SetBool("Sprinting", false);
                sprinting = false;
                speed = 6.0f;
            }

            // Melee attack
            if (Input.GetKey(KeyCode.Mouse0) && !resting && !uiOn)
            {
                resting = true;
                hitting = true;
                animation.SetTrigger("MeleeSlash");
                //AttackMelee();
            }



            // Spell key check
            for (int i = 0; i < actionBarKeys.Length; i++)
            {
                if (Input.GetKeyDown(actionBarKeys[i]) && !aiming && !uiOn)
                {
                    chargeEffect.Play();
                    ActionBar(i);
                }
            }


            //animaatiota varten
            if (velocity.y < -5)
            {
                animation.SetBool("Falling", true);
            }
            else
            {
                animation.SetBool("Falling", false);
            }
            //animaatiota varten
            if (isGrounded)
            {
                animation.SetBool("Grounded", true);
            }
            else
            {
                animation.SetBool("Grounded", false);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) && rollTimer >= 0.4f && isGrounded && energy_fill >= 1 && !uiOn)
            {
                energyRegenWait = 0;

                //Skills
                int levelUpCheck = (int)GameStatus.instance.skillsAndStats.enduranceLevel;
                GameStatus.instance.skillsAndStats.enduranceLevel += 0.02f;
                if (levelUpCheck < (int)GameStatus.instance.skillsAndStats.enduranceLevel)
                    GameObject.Find("Canvas").GetComponent<UI_Elements>().StartLevelUpCoroutine("Endurance +1");

                animation.SetTrigger("Roll");
                energy_fill -= 0.5f;
                rollSpeed = 25;
                rollTimer = 0;
            }
            if (rollTimer < 0.4f && !uiOn)
            {

                rollSpeed -= 60 * Time.deltaTime;
                rollTimer += Time.deltaTime;
                controller.Move(transform.forward * rollSpeed * Time.deltaTime);
            }
            //Dash Hyppy
            if (rollTimer >= 0.25f && rollTimer < 0.4f && !uiOn)
            {
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    Instantiate(dashParticle, transform.position, Quaternion.identity);
                    rollTimer = 0;
                    rollSpeed = 25;
                    animation.SetTrigger("Jump");
                    //Skills
                    int levelUpCheck = (int)GameStatus.instance.skillsAndStats.enduranceLevel;
                    GameStatus.instance.skillsAndStats.enduranceLevel += 0.05f;
                    if (levelUpCheck < (int)GameStatus.instance.skillsAndStats.enduranceLevel)
                        GameObject.Find("Canvas").GetComponent<UI_Elements>().StartLevelUpCoroutine("Endurance +1");
                }
            }
            //fallVelocity = (fallPosition - transform.position.y) / Time.deltaTime;
            //fallPosition = transform.position.y;
            //if (isGrounded)
            //{
            //    if (fallVelocity > 25 && !spawnDrop)
            //    {
            //        //Debug.Log("Rivi 395");
            //        HurtPlayer(fallVelocity / 10);
            //    }
            //    spawnDrop = false;
            //}


            if (!controller.isGrounded)
            {
                velocity.y += (gravity * 2) * Time.deltaTime;
            }

            controller.Move(velocity * (speed / 3) * Time.deltaTime);
        }

        if (invisible_running)
        {
            invisible.SetActive(true);
            visible.SetActive(false);
            gameObject.tag = "Invisible"; // näkymätön
        }

        if (!invisible_running)
        {
            invisible.SetActive(false);
            visible.SetActive(true);
            gameObject.tag = "Player"; // näkyvä
        }
        if (photonView.IsMine)
        {
            //PICKUP TARVITSEE 2/2 (Oltava update loopissa):
            Collider[] pickupObjects = Physics.OverlapSphere(transform.position, pickupDistance, pickupLayer);
            if (pickupObjects.Length > 0)
            {
                nearPickupable = true;
                objectName = pickupObjects[0].GetComponentInParent<PickupObject>().item.itemName;
                if (Input.GetKeyDown("e"))
                {
                    inventory.AddItem(pickupObjects[0].transform.GetComponent<PickupObject>().item);
                    if (pickupObjects[0].transform.gameObject.GetComponent<PhotonView>() != null)
                    {
                        int pickupObject = pickupObjects[0].transform.gameObject.GetComponent<PhotonView>().ViewID;
                        photonView.RPC("PhotonDestroy", RpcTarget.All, pickupObject);
                    }
                    else
                    {
                        Destroy(pickupObjects[0].gameObject);
                    }
                }
            }
            else
                nearPickupable = false;
            //___________________________^_PICKUP_^______________________________________________________________
        }


        if (Input.GetKeyDown("m") && photonView.IsMine)
        {

            if (!map.activeSelf)
                StartCoroutine(OpenMap(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 100, gameObject.transform.position.z), 4));
            else
                StartCoroutine(CloseMap(normal, 4));

        }
    }


    // Vihollisten lyönti
    public void AttackMelee()
    {
        int meleeLevelUpCheck = (int)GameStatus.instance.skillsAndStats.meleeLevel;

        Collider[] enemiesHit = Physics.OverlapBox(attackArea.position, areaOfAttack, transform.rotation, enemy);
        foreach (Collider enemy in enemiesHit)
        {
            if (!enemy.isTrigger && photonView.IsMine)
            {
                GameStatus.instance.skillsAndStats.meleeLevel += 0.05f;
                enemy.GetComponent<TakeDamage>().DoDamageToEnemy((int)(GameStatus.instance.skillsAndStats.meleeLevel) + itemDamage, gameObject.GetComponent<PhotonView>().ViewID);
                StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().FailText(((int)(GameStatus.instance.skillsAndStats.meleeLevel) + itemDamage).ToString()));
                if ((int)GameStatus.instance.skillsAndStats.meleeLevel > meleeLevelUpCheck)
                    StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().LevelUp("Melee Combat +1"));
            }
        }


        // Resource lyönti
        //Collider[] resourcesHit = Physics.OverlapSphere(attackArea.position, attackRange, tree);
        Collider[] resourcesHit = Physics.OverlapBox(attackArea.position, areaOfAttack, transform.rotation, tree);
        foreach (Collider resource in resourcesHit)
        {
            if (leftHandItemType == Item.ItemType.Axe && photonView.IsMine)
            {
                float random = Random.Range(0, 100);
                int levelUpCheck = (int)GameStatus.instance.skillsAndStats.woodcuttingLevel;

                if (random < (int)GameStatus.instance.skillsAndStats.woodcuttingLevel + 50)
                {
                    photonView.RPC("PhotonDamageResource", RpcTarget.All, resource.name, ((int)(GameStatus.instance.skillsAndStats.meleeLevel) + itemDamage) * ((int)GameStatus.instance.skillsAndStats.woodcuttingLevel) / 5); //damagesta vähennetään enemmän, jos woodcutting level on paska
                    float dmg = ((int)(GameStatus.instance.skillsAndStats.meleeLevel) + itemDamage) * ((int)GameStatus.instance.skillsAndStats.woodcuttingLevel) / 5;
                    StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().FailText(dmg.ToString()));
                    GameStatus.instance.skillsAndStats.woodcuttingLevel += 0.1f;
                    if ((int)GameStatus.instance.skillsAndStats.woodcuttingLevel > levelUpCheck)
                        StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().LevelUp("Woodcutting +1"));
                }
                else
                {
                    GameStatus.instance.skillsAndStats.woodcuttingLevel += 0.02f;
                    StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().FailText("Miss"));
                    if ((int)GameStatus.instance.skillsAndStats.woodcuttingLevel > levelUpCheck)
                        StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().LevelUp("Woodcutting +1"));
                }
            }
        }


    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(attackArea.position, attackArea.position);
        Gizmos.DrawWireCube(attackArea.position, new Vector3(1, 1.7f, 1.6f));
        //Gizmos.DrawWireSphere(attackArea.position, attackRange);
    }

    public void HurtPlayer(float damage)
    {
        if (photonView.IsMine)
        {
            int levelUpCheck = (int)GameStatus.instance.skillsAndStats.healthLevel;
            if (!parry && !dead)
            {
                if (damage > 0)
                {
                    // Blood & knockback
                    AddImpact(-transform.forward+new Vector3(2,0,2), 10.0f);
                    photonView.RPC("InstantiateSpell", RpcTarget.All, 8, lookAt.position, Quaternion.identity, 0, camTransform.forward);
                    GameStatus.instance.skillsAndStats.healthLevel += ((100-GameStatus.instance.skillsAndStats.healthLevel) / 1000);
                }
                // Damage, voi olla -damage eli heal
                hp_fill -= damage;
            }

            if ((int)GameStatus.instance.skillsAndStats.healthLevel > levelUpCheck)
                StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().LevelUp("Health Level +1"));

            if (hp_fill <= 0 && !dead /*Debug:*/ && !GOD_MODE)
            {
                dead = true;
                gameObject.tag = ("dead");
                GameObject droppedInventory = PhotonNetwork.Instantiate("DroppedInventory", transform.position, Quaternion.identity);
                droppedInventory.GetComponent<OtherInventory>().SetItemList(inventory.GetItemList());
                droppedInventory.GetComponent<OtherInventory>().destroyTimerSeconds = 300;
                droppedInventory.GetComponent<OtherInventory>().destroyTimer = true;
                List<Item> emptyList = new List<Item>();
                inventory.SetItemList(emptyList);
                photonView.RPC("PhotonDie", RpcTarget.All, photonView.ViewID);
                controller.enabled = false;
            }
        }

    }

    // Käytetään enabloimaan Invisible ja Parry.
    IEnumerator EnableParry(int spellIndex, float duration, int castID)
    {
        GameObject caster = PhotonView.Find(castID).gameObject;
        GameObject spell = Instantiate(spells[spellIndex], caster.transform.position, Quaternion.identity) as GameObject;
        ParticleSystem parts = spell.GetComponent<ParticleSystem>();
        float lifeDuration = parts.duration + parts.startLifetime;
        Destroy(spell, lifeDuration);

        if (photonView.ViewID == castID && spellIndex == 3)
            parry = true;

        if (photonView.ViewID == castID && spellIndex == 0)
            invisible_running = true;


        float time = 0;

        while (time < duration)
        {
            if (spellIndex != 0)
            {
                spell.transform.rotation = caster.transform.rotation;
                spell.transform.position = caster.transform.position;
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (photonView.ViewID == castID && spellIndex == 3)
            parry = false;

        if (photonView.ViewID == castID && spellIndex == 0)
            invisible_running = false;


    }


    IEnumerator Relentless(int spellIndex, float duration, int castID)
    {
        GameObject caster = PhotonView.Find(castID).gameObject;

        Vector3 playerScale = caster.transform.localScale;
        Vector3 bigScale = caster.transform.localScale * 2;

        GameObject spell = Instantiate(spells[spellIndex], caster.transform.position, Quaternion.identity) as GameObject;
        ParticleSystem parts = spell.GetComponent<ParticleSystem>();
        float lifeDuration = parts.duration + parts.startLifetime;
        Destroy(spell, lifeDuration);

        if (photonView.IsMine)
            GameStatus.instance.skillsAndStats.meleeLevel = GameStatus.instance.skillsAndStats.meleeLevel * (2 + (GameStatus.instance.skillsAndStats.spellCastingLevel / 100));
        //Debug.Log("Melee level boost: " + GameStatus.instance.skillsAndStats.meleeLevel);

        areaOfAttack *= 2;

        float time = 0;
        while (time < duration)
        {
            spell.transform.rotation = caster.transform.rotation;
            spell.transform.position = caster.transform.position;
            caster.transform.localScale = Vector3.Lerp(caster.transform.localScale, bigScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        controller.radius /= 2;
        float BigTime = 0;
        while (BigTime < duration)
        {
            spell.transform.rotation = caster.transform.rotation;
            spell.transform.position = caster.transform.position;
            BigTime += Time.deltaTime;
            yield return null;
        }

        playerScale = caster.transform.localScale / 2;

        float SmallTime = 0;
        while (SmallTime < duration)
        {
            caster.transform.localScale = Vector3.Lerp(caster.transform.localScale, playerScale, SmallTime / duration);
            SmallTime += Time.deltaTime;
            yield return null;
        }

        GameStatus.instance.skillsAndStats.meleeLevel = GameStatus.instance.skillsAndStats.meleeLevel / (2 + (GameStatus.instance.skillsAndStats.spellCastingLevel / 100));
        //Debug.Log("Melee level normalized: " + GameStatus.instance.skillsAndStats.meleeLevel);
        controller.radius *= 2;
        areaOfAttack /= 2;
    }


    IEnumerator ShootSpell(int spellIndex, float duration, float moveSpeed)
    {

        aiming = true;
        animation.SetBool("Charging", true);
        xhair[0].ToggleCrosshair();
        xhair[1].ToggleCrosshair();
        targetFov = 20;
        Vector3 targetPosition = aim;
        Vector3 startPosition = normal;
        float time = 0;

        while (time < duration)
        {
            lookAt.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        float time2 = 0;
        while (time2 < 1.0f)
        {
            time2 += Time.deltaTime;
            yield return null;
        }

        animation.SetBool("Charging", false);
        lookAt.localPosition = targetPosition;


        if (spellIndex == 4)
        {
            object[] obj = new object[1];
            obj[0] = photonView.ViewID;
            GameObject Spell = PhotonNetwork.Instantiate("SpellChicken", transform.position + new Vector3(3, 0, 3), Quaternion.identity, 0, obj);
        }
        else
        {
            photonView.RPC("InstantiateSpell", RpcTarget.All, spellIndex, attackArea.position, Quaternion.identity, (int)moveSpeed, camTransform.forward);
        }
        StartCoroutine(ExitShootSpell(0.5f));
    }

    IEnumerator ExitShootSpell(float duration)
    {
        xhair[0].ToggleCrosshair();
        xhair[1].ToggleCrosshair();
        targetFov = 40;
        float time = 0;
        Vector3 targetPosition = normal;
        Vector3 startPosition = aim;
        while (time < duration)
        {
            lookAt.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        lookAt.localPosition = targetPosition;
        aiming = false;
    }

    IEnumerator ThunderStrike(int spellIndex, float duration)
    {
        aiming = true;
        animation.SetBool("Charging", true);
        GameObject landingPointCursor = Instantiate(landingPoint, transform.position + new Vector3(3,0,3), Quaternion.Euler(new Vector3(90, 0, 0)));

        float time = 0;

        Vector3 localscale = landingPointCursor.transform.localScale;
        Vector3 small = new Vector3(0, 0, 0);

        while (time < duration)
        {
            landingPointCursor.transform.localScale = Vector3.Lerp(localscale, small, time / duration);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position + Camera.main.transform.TransformDirection(Vector3.up) * 2, Camera.main.transform.forward, out hit, 80, groundMask))
            {
                landingPointCursor.transform.position = hit.point + Vector3.up * 0.1f;
            }
            time += Time.deltaTime;
            yield return null;
        }
        Vector3 spawnPoint = landingPointCursor.transform.position;
        spawnPoint.y = spawnPoint.y + 4;
        aiming = false;
        animation.SetBool("Charging", false);
        Destroy(landingPointCursor);
        photonView.RPC("InstantiateSpell", RpcTarget.All, spellIndex, spawnPoint, Quaternion.Euler(new Vector3(0, 90, 0)), 0, camTransform.forward);

    }

    [PunRPC]
    void InstantiateSpell(int spellIndex, Vector3 startPos, Quaternion rotation, int moveSpeed, Vector3 castDirection)
    {

        GameObject spell = Instantiate(spells[spellIndex], startPos, rotation) as GameObject;
        ParticleSystem parts = spell.GetComponent<ParticleSystem>();
        if (spell.GetComponent<ExplosionCollision>() != null)
            spell.GetComponent<ExplosionCollision>().Owner = gameObject;
        if (spell.GetComponent<ShadowSphere>() != null)
        {
            spell.GetComponent<ShadowSphere>().Owner = gameObject;
        }

        float lifeDuration = parts.duration + parts.startLifetime;
        Destroy(spell, lifeDuration);

        if (spellIndex != 8)
        {
            spell.GetComponent<Rigidbody>().AddForce(castDirection * moveSpeed);
        }
    }

    //Kaksi versiota: toinen vaatii castIDn ja toinen ei
    public void ExecCoroutine(int spellIndex, float duration)
    {
        float fDuration = duration;

        if (spellIndex == 2) StartCoroutine(ShootSpell(spellIndex, fDuration, 2000));
        if (spellIndex == 4) StartCoroutine(ShootSpell(spellIndex, fDuration, 0));
        if (spellIndex == 6) StartCoroutine(ShootSpell(spellIndex, fDuration, 500));
        if (spellIndex == 7) StartCoroutine(ShootSpell(spellIndex, fDuration, 0));

    }
    [PunRPC]
    public void ExecCoroutine(int spellIndex, int duration, int castID)
    {
        float fDuration = duration;

        if (spellIndex == 1) StartCoroutine(Relentless(spellIndex, fDuration, castID));

        if (spellIndex == 3) StartCoroutine(EnableParry(spellIndex, fDuration, castID));

        if (spellIndex == 0) StartCoroutine(EnableParry(spellIndex, fDuration, castID));

    }

    [PunRPC]
    void Wield(int ID, string targetItem, object targetType)
    {
        GameObject targetPlayer = PhotonView.Find(ID).gameObject;
        if (targetPlayer.GetComponent<PlayerController>().leftHandItemType == (Item.ItemType)targetType)
        {
            if ((Item.ItemType)targetType == Item.ItemType.Axe)
            {
                targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(false);
            }
            else if ((Item.ItemType)targetType == Item.ItemType.Sword)
            {
                targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(false);
            }
            targetPlayer.GetComponent<PlayerController>().objectInLeftHand = "";
            targetPlayer.GetComponent<PlayerController>().leftHandItemType = Item.ItemType.Empty;

        }
        else if (targetPlayer.GetComponent<PlayerController>().leftHandItemType != Item.ItemType.Empty)
        {
            if (targetPlayer.GetComponent<PlayerController>().leftHandItemType == Item.ItemType.Axe)
                targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(false);
            else if (targetPlayer.GetComponent<PlayerController>().leftHandItemType == Item.ItemType.Sword)
                targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(false);

            if ((Item.ItemType)targetType == Item.ItemType.Axe)
            {
                targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(true);
                leftHandItemType = (Item.ItemType)targetType;
            }
            else if ((Item.ItemType)targetType == Item.ItemType.Sword)
            {
                targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(true);
                leftHandItemType = (Item.ItemType)targetType;
            }

            targetPlayer.GetComponent<PlayerController>().objectInLeftHand = targetItem;
        }
        else if ((Item.ItemType)targetType == Item.ItemType.Axe)
        {
            targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(true);
            targetPlayer.GetComponent<PlayerController>().objectInLeftHand = targetItem;
            targetPlayer.GetComponent<PlayerController>().leftHandItemType = (Item.ItemType)targetType;
        }
        else if ((Item.ItemType)targetType == Item.ItemType.Sword)
        {
            targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(true);
            targetPlayer.GetComponent<PlayerController>().objectInLeftHand = targetItem;
            targetPlayer.GetComponent<PlayerController>().leftHandItemType = (Item.ItemType)targetType;
        }
    }

    //VANHA WIELDI
    //[PunRPC]
    //void Wield(int ID, string targetItem, object targetType)
    //{
    //    GameObject targetPlayer = PhotonView.Find(ID).gameObject;


    //    if (targetPlayer.GetComponent<PlayerController>().objectInLeftHand == targetItem)
    //    {
    //        if ((Item.ItemType)targetType == Item.ItemType.Weapon)
    //        {
    //            targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find(targetItem).gameObject.SetActive(false);
    //        }
    //        targetPlayer.GetComponent<PlayerController>().objectInLeftHand = "";
    //    }
    //    else if (targetPlayer.GetComponent<PlayerController>().objectInLeftHand != "")
    //    {
    //        targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find(targetPlayer.GetComponent<PlayerController>().objectInLeftHand).gameObject.SetActive(false);
    //        if ((Item.ItemType)targetType == Item.ItemType.Weapon)
    //        {
    //            targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find(targetItem).gameObject.SetActive(true);
    //        }
    //        targetPlayer.GetComponent<PlayerController>().objectInLeftHand = targetItem;
    //    }
    //    else if ((Item.ItemType)targetType == Item.ItemType.Weapon)
    //    {
    //        targetPlayer.GetComponent<PlayerController>().leftHandItemSlot.transform.Find(targetItem).gameObject.SetActive(true);
    //        targetPlayer.GetComponent<PlayerController>().objectInLeftHand = targetItem;
    //    }
    //}

    void EquipCharacter()
    {
        if (photonView.Owner.CustomProperties.TryGetValue("objectInLeftHand", out object value))
            if ((Item.ItemType)photonView.Owner.CustomProperties["objectInLeftHand"] != Item.ItemType.Empty)
            {
                if ((Item.ItemType)photonView.Owner.CustomProperties["objectInLeftHand"] == Item.ItemType.Axe)
                    leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(true);
                else if ((Item.ItemType)photonView.Owner.CustomProperties["objectInLeftHand"] == Item.ItemType.Sword)
                    leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(true);
            }
            else
            {
                leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(false);
                leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(false);
            }


    }
    //_______________________WIELD LISÄYS 3/3_______________________________________

    public void DropItem(Item droppedItem)
    {
            object[] dropObj = new object[8];
            dropObj[0] = droppedItem.itemName;
            dropObj[1] = droppedItem.itemType;
            dropObj[2] = droppedItem.healingAmount;
            dropObj[3] = droppedItem.stackSize;
            dropObj[4] = droppedItem.amount;
            dropObj[5] = droppedItem.damage;
            dropObj[6] = droppedItem.extra;
            dropObj[7] = droppedItem.value;

        if(droppedItem.itemName == objectInLeftHand && droppedItem.itemType == leftHandItemType)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("objectInLeftHand", Item.ItemType.Empty);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            objectInLeftHand = "";
            leftHandItemType = Item.ItemType.Empty;
            leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(false);
            leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(false);
        }

            PhotonNetwork.Instantiate("DroppedItem", transform.localPosition + transform.forward * 2, Quaternion.identity, 0, dropObj);
    }

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    //MAPPIIN
    IEnumerator OpenMap(Vector3 targetPosition, float duration)
    {
        mapEnabled = true;
        Vector3 mapStartPos = new Vector3(0, 0, 3.7f);
        Vector3 mapTransitionPos = new Vector3(0, 0, 0);
        Image whiteFade = GameObject.Find("Canvas").transform.Find("White").GetComponent<Image>();
        float fade = 0;
        float time = 0;
        Vector3 startPosition = gameObject.transform.position;
        Vector3 startRotation = normal;

        bool whiteScreen = false;

        map.transform.localPosition = mapTransitionPos;
        while (time < duration)
        {
            if (time > duration / 2)
            {
                if (!whiteScreen && whiteFade.color.a <= 1)
                {
                    fade = whiteFade.color.a + Time.deltaTime * 2;
                    whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, fade);
                }
                else
                {
                    whiteScreen = true;
                    map.SetActive(true);

                }
                if (whiteScreen)
                {
                    fade = whiteFade.color.a - Time.deltaTime * 2;
                    whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, fade);
                }
            }

            lookAt.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            map.transform.localPosition = Vector3.Lerp(mapTransitionPos, mapStartPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, 0);
    }

    IEnumerator CloseMap(Vector3 targetPosition, float duration)
    {
        Vector3 mapStartPos = new Vector3(0, 0, 3.7f);
        Vector3 mapTransitionPos = new Vector3(0, 0, 0);
        Image whiteFade = GameObject.Find("Canvas").transform.Find("White").GetComponent<Image>();
        float fade = 0;
        //aiming = true;
        float time = 0;
        Vector3 startPosition = new Vector3(0, gameObject.transform.position.y + 100, 0);

        bool whiteScreen = false;

        map.transform.localPosition = mapTransitionPos;
        while (time < duration)
        {
            if (time > duration / 3)
            {
                if (!whiteScreen && whiteFade.color.a <= 1)
                {
                    fade = whiteFade.color.a + Time.deltaTime * 2;
                    whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, fade);
                }
                else
                {
                    whiteScreen = true;
                    map.SetActive(false);

                }
                if (whiteScreen)
                {
                    fade = whiteFade.color.a - Time.deltaTime * 2;
                    whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, fade);
                }
            }

            lookAt.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            map.transform.localPosition = Vector3.Lerp(mapStartPos, mapTransitionPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        lookAt.localPosition = targetPosition;
        whiteFade.color = new Color(whiteFade.color.r, whiteFade.color.g, whiteFade.color.b, 0);
        mapEnabled = false;
    }

    IEnumerator Die()
    {
        if (photonView.IsMine)
        {

            float respawnTimer = 0;
            while (respawnTimer < 5)
            {
                respawnTimer += Time.deltaTime;
                yield return null;
            }
            GameObject.Find("WorldManager").GetComponent<WorldManager>().ChangeToSaveLocationWorld();
            transform.position = GameStatus.instance.saveData.location;
            spawnDrop = true;
            photonView.RPC("PhotonSetPlayerActive", RpcTarget.All, photonView.ViewID);
            //Otetaan ase pois kädestä
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("objectInLeftHand", Item.ItemType.Empty);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            objectInLeftHand = "";
            leftHandItemType = Item.ItemType.Empty;
            leftHandItemSlot.transform.Find("Kirves").gameObject.SetActive(false);
            leftHandItemSlot.transform.Find("Miekka").gameObject.SetActive(false);
            GameStatus.instance.skillsAndStats.healthLevel -= 0.1f;
            controller.enabled = true;
            hp_fill = GameStatus.instance.skillsAndStats.healthLevel;
            photonView.RPC("PhotonSetPlayerActive", RpcTarget.All, photonView.ViewID);
            gameObject.tag = ("Player");
            dead = false;
        }
    }
    public void ActionBar(int i)
    {
        if (!mapEnabled)
        {
            int levelUpCheck = (int)GameStatus.instance.skillsAndStats.spellCastingLevel;
            switch (i)
            {
                case 0:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        photonView.RPC("ExecCoroutine", RpcTarget.All, i, 5, photonView.ViewID);
                        bars[i].EnableCooldown(15 - (GameStatus.instance.skillsAndStats.spellCastingLevel / 100)); // Spellmastery: CD reduction
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);

                    }
                    break;

                case 1:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        // JÄTTI
                        photonView.RPC("ExecCoroutine", RpcTarget.All, i, 5, photonView.ViewID);
                        bars[i].EnableCooldown(120 - (GameStatus.instance.skillsAndStats.spellCastingLevel)); // Spellmastery: CD reduction
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;

                case 2:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        
                        // Fire spell
                        //photonView.RPC("ExecCoroutine", RpcTarget.All, i, 2);
                        ExecCoroutine(i, 0.5f);
                        bars[i].EnableCooldown(15 - (GameStatus.instance.skillsAndStats.spellCastingLevel / 10)); // Spellmastery: CD reduction + Dmg increase
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;

                case 3:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        // SHIELD
                        photonView.RPC("ExecCoroutine", RpcTarget.All, i, 5, photonView.ViewID);
                        //ExecCoroutine(i, 5);  
                        bars[i].EnableCooldown(20 - (GameStatus.instance.skillsAndStats.spellCastingLevel / 10)); // Spellmastery: CD reduction
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;

                case 4:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        // chicken
                        ExecCoroutine(i, 0.5f);
                        bars[i].EnableCooldown(60 - (GameStatus.instance.skillsAndStats.spellCastingLevel / 2)); // Spellmastery: CD reduction + Dmg increase (BattleChicken.cs)
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;

                case 5:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        // salamipilvi
                        StartCoroutine(ThunderStrike(i, 2));
                        bars[i].EnableCooldown(60 - (GameStatus.instance.skillsAndStats.spellCastingLevel / 2)); // Spellmastery: CD reduction + Dmg increase
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;

                case 6:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        // Ice spell
                        //photonView.RPC("ExecCoroutine", RpcTarget.All, i, 2);
                        ExecCoroutine(i, 0.5f);
                        bars[i].EnableCooldown(15 - (GameStatus.instance.skillsAndStats.spellCastingLevel / 10)); // Spellmastery: CD reduction + Dmg increase
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;

                case 7:
                    if (bars[i].cdFinished && bars[i].enabled)
                    {
                        ExecCoroutine(i, 0.5f);
                        bars[i].EnableCooldown(120 - (GameStatus.instance.skillsAndStats.spellCastingLevel)); 
                        GameStatus.instance.skillsAndStats.spellCastingLevel += 0.5f - (GameStatus.instance.skillsAndStats.spellCastingLevel / 1000);
                    }
                    break;
            }
            if ((int)GameStatus.instance.skillsAndStats.spellCastingLevel > levelUpCheck)
                StartCoroutine(uiBuildMenu.transform.parent.gameObject.GetComponent<UI_Elements>().LevelUp("Spell Mastery +1"));
        }

    }

    void SpellCheck()
    {
        if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 1 && !spellsActivated[0])
        {
            bars[0].enabled = true;
            bars[0].GetComponent<Image>().fillAmount = 1;
            rightHandItemSlot.SetActive(true);
            spellsActivated[0] = true;
        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 10 && !spellsActivated[1])
        {
            bars[1].enabled = true;
            bars[1].GetComponent<Image>().fillAmount = 1;
            spellsActivated[1] = true;
        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 20 && !spellsActivated[2])
        {
            bars[2].enabled = true;
            bars[2].GetComponent<Image>().fillAmount = 1;
            spellsActivated[2] = true;
        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 30 && !spellsActivated[3])
        {
            bars[3].enabled = true;
            bars[3].GetComponent<Image>().fillAmount = 1;
            spellsActivated[3] = true;
        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 40 && !spellsActivated[4])
        {
            bars[4].enabled = true;
            bars[4].GetComponent<Image>().fillAmount = 1;
            spellsActivated[4] = true;
        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 50 && !spellsActivated[5])
        {
            bars[5].enabled = true;
            bars[5].GetComponent<Image>().fillAmount = 1;
            spellsActivated[5] = true;

        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 60 && !spellsActivated[6])
        {
            bars[6].enabled = true;
            bars[6].GetComponent<Image>().fillAmount = 1;
            spellsActivated[6] = true;

        }
        else if (GameStatus.instance.skillsAndStats.spellCastingLevel >= 70 && !spellsActivated[7])
        {
            bars[7].enabled = true;
            bars[7].GetComponent<Image>().fillAmount = 1;
            spellsActivated[7] = true;
        }
    }

    //// Universaali poistaminen tai jotain vastaavaa :smile:
    //[PunRPC]
    //public void PhotonDestroy(int objectToBeDestroyed)
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonNetwork.Destroy(PhotonView.Find(objectToBeDestroyed).gameObject);
    //    }
    //}

    // Uusi versio 5.5.21
    [PunRPC]
    public void PhotonDestroy(int objectToBeDestroyed)
    {
        if (PhotonView.Find(objectToBeDestroyed).gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(PhotonView.Find(objectToBeDestroyed).gameObject);
        }
    }


    [PunRPC]
    public void PhotonRuntimeDestroy(int objectToBeDestroyed)
    {
        Destroy(PhotonView.Find(objectToBeDestroyed).gameObject);
    }

    [PunRPC]
    public void PhotonDamageResource(string name, float dmg)
    {
        GameObject target = GameObject.Find(name);
        if (target.GetComponent<TreeScript>() != null)
        {
            target.GetComponent<TreeScript>().TakeDamage(dmg);
            transform.GetChild(0).GetComponent<SoundScript>().Woodcutting();
        }
    }

    [PunRPC]
    public void PhotonDie(int ID)
    {
        PhotonView.Find(ID).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        if (photonView.IsMine)
            StartCoroutine("Die");
    }

    [PunRPC]
    public void PhotonSetPlayerActive(int ID)
    {
        if (photonView.IsMine)
        {
            transform.position = GameStatus.instance.saveData.location;
            fallPosition = transform.position.y;
            fallVelocity = 0;
        }

        PhotonView.Find(ID).transform.GetChild(0).gameObject.SetActive(true);

        if (!photonView.IsMine)
            EquipCharacter();
            //PhotonView.Find(ID).transform.GetChild(0).gameObject.GetComponent<PlayerController>().EquipCharacter();
    }

    public void RPCBuild(string prefabName, Vector3 pos)
    {
        photonView.RPC("PhotonBuild", RpcTarget.All, prefabName, pos);
    }

    [PunRPC]
    void PhotonBuild(string prefabName, Vector3 pos)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newBuilding = PhotonNetwork.InstantiateRoomObject(prefabName, pos, Quaternion.identity);
            if (newBuilding.GetComponent<Shelter>() != null)
                GameStatus.instance.buildings.Add(newBuilding.GetComponent<Shelter>().saveLoad);
            //Laittakaa päälle, jos haluatte nuotion tallentuvan saveen:
            //if (newBuilding.GetComponent<Campfire>() != null)
            //    GameStatus.instance.buildings.Add(newBuilding.GetComponent<Campfire>().saveLoad);
        }
    }
}
