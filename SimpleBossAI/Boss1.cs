using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Boss1 : MonoBehaviourPunCallbacks
{
    public RandomDrop randomDrop;
    Collider[] hitArea;
    public ParticleSystem impactEffect;
    public Animator animator;
    public EnemyHealthBar healthBar;
    public GameObject attackEffect;
    public GameObject minion;
    public GameObject damageText;
    GameObject playerTarget;
    public Transform hitPosition;
    public Vector3 hidePoint;
    public Vector3 exposePoint;
    public Vector3 lookPoint;
    public NavMeshAgent agent;
    public LayerMask groundMask, playerMask, playerIDMask;
    private bool attacked;
    private bool dead;
    public bool hidePointSet;
    public bool attacking;
    public bool playerInSight;
    public bool playerInAttackRange;
    public bool hostCheck;
    private bool targetAlreadyChanged;
    public float sightRange;
    public float attackRange;
    public float hidePointRange;
    public float exposePointRange;
    public float hitTimer;
    public float health = 10000f;
    public float damageTakenForHide;
    private float damage;
    private int randomHit;
    private int playersRange;


    // GAME END JUTUT
    public GameObject teleportToEndArea;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //players = new List<GameObject>();
        if (NetworkManager.global.MasterCheck() == true) hostCheck = true;

        targetAlreadyChanged = true;


    }


    private void Start()
    {
        // T�nne after boss jutun resetointi

        teleportToEndArea = GameObject.FindGameObjectWithTag("FinalPortal");
        teleportToEndArea.SetActive(false);

        healthBar.gameObject.SetActive(true);
    }


    private void FixedUpdate()
    {
        if (hostCheck)
        {
            agent.speed = 25 + ((10000 - health) / 1000);
        }
        CountPlayers();


    }
    private void Update()
    {
        if (hostCheck)
        {
            if (!dead)
            {
                // Hide
                if (damageTakenForHide >= 2000 && health > 2000 && !hidePointSet && !dead) Hide();
                // Chase
                if (playerInSight && !hidePointSet && !attacking && !dead) Chase();
                // Idle
                if (!playerInSight) agent.SetDestination(new Vector3(-300, -130, 790));
                // Attack
                playerInAttackRange = Physics.CheckSphere(hitPosition.position, attackRange, playerMask);
                if (playerInSight && playerInAttackRange && !hidePointSet)
                {
                    if (!attacking)
                        if (hitTimer >= 1.5f)
                            Attack();
                }
                hitTimer += Time.deltaTime;
            }
            // Dead
            if (dead) agent.SetDestination(transform.position);

        }

    }
    void Hide()
    {
        Debug.Log("Hiding");
        if (Physics.Raycast(transform.position, -transform.up, 2f, groundMask))
        {

            hidePointSet = true;
            agent.SetDestination(transform.position);
            Debug.Log("Hiding");
            photonView.RPC("PhotonHide", RpcTarget.All);
            Invoke(nameof(InvokeHideEffect), 0.5f);
            Invoke(nameof(Expose), 15f - ((10000 - health) / 1000));
            Invoke(nameof(SpawnMinions), 3f);
        }
    }
    void Expose()
    {
        photonView.RPC("PhotonExpose", RpcTarget.All);
        float randomX = Random.Range(-exposePointRange, exposePointRange);
        float randomZ = Random.Range(-exposePointRange, exposePointRange);

        exposePoint = new Vector3(playerTarget.transform.position.x + randomX, playerTarget.transform.position.y, playerTarget.transform.position.z + randomZ);

        if (Physics.Raycast(exposePoint, -transform.up, 2f, groundMask)) transform.position = exposePoint;

        Invoke(nameof(InvokeHideEffect), 0.2f);
        Debug.Log("exposing");
        hidePointSet = false;
    }

    void Chase()
    {
        agent.SetDestination(playerTarget.transform.position);
    }


    [PunRPC]
    void PhotonHide()
    {
        damageTakenForHide = 0;
        animator.ResetTrigger("Expose");
        animator.SetTrigger("Hide");
        healthBar.gameObject.SetActive(false);
    }
    [PunRPC]
    void PhotonExpose()
    {
        animator.SetTrigger("Expose");
        healthBar.gameObject.SetActive(true);

        int rnd = Random.Range(0, 2);
        if (rnd == 1) PhotonNetwork.Instantiate("HealingCrystal", transform.position, Quaternion.identity);

    }

    void SpawnMinions()
    {
        int minionCount = hitArea.Length + 1 + (((int)((10000 - health) / 1000)) / 2);

        for (int i = 0; i < minionCount; i++)
        {
            float randomX = Random.Range(exposePointRange, exposePointRange * 2);
            float randomZ = Random.Range(exposePointRange, exposePointRange * 2);
            Vector3 targetLocation = hitArea[Random.Range(0, hitArea.Length - 1)].transform.position;
            Vector3 spawnPoint = new Vector3(targetLocation.x + randomX, transform.position.y, targetLocation.z + randomZ);

            PhotonNetwork.Instantiate("Boss1Minion", spawnPoint, Quaternion.identity);
        }
    }

    [PunRPC]
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Attack()
    {
        randomHit = Random.Range(1, 4);
        attacking = true;
        Debug.Log("Random hit: " + randomHit);
        if (randomHit == 1)
        {
            damage = 150 + ((10000 - health) / 100);
            photonView.RPC("SetDamage", RpcTarget.All, damage);
            animator.SetTrigger("Attack");
        }
        else
        {
            damage = 100 + ((10000 - health) / 150);
            photonView.RPC("SetDamage", RpcTarget.All, damage);
            animator.SetTrigger("LegAttack");
        }

    }

    public void RandomAttack()
    {
        Collider[] hitArea = Physics.OverlapSphere(hitPosition.position, 15f, playerMask);
        foreach (Collider obj in hitArea)
        {
            if (obj.transform.CompareTag("Player"))
            {
                Debug.Log("hitted player!");
                obj.transform.GetComponentInChildren<HealthManager>().HurtPlayer(damage);
            }
        }
        InstantiateEffect(0);
        attacking = false;
        hitTimer = 0;
    }

    // any player related damage taken
    public void TakeDamage(float dmg)
    {
        if (!hidePointSet) photonView.RPC("PhotonTakeDamage", RpcTarget.All, dmg);

        Vector3 offsetPosition = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
        DamagePopup popup = Instantiate(damageText, offsetPosition, Quaternion.identity).GetComponent<DamagePopup>();
        popup.SetDamageText(dmg);
        popup.ChangeTextColor(Color.red);
    }

    [PunRPC]
    void PhotonTakeDamage(float dmg)
    {
        health -= dmg;
        damageTakenForHide += dmg;
        Debug.Log("boss takes damage");

        if (health <= 0)
        {
            healthBar.gameObject.SetActive(false);
            animator.SetTrigger("Death");
            dead = true;

            // T�nne after boss fight jutut
            teleportToEndArea.SetActive(true);


            Invoke(nameof(DropLoot), 4f);
            Invoke(nameof(DestroyObject), 5f);
        }
    }


    void CountPlayers()
    {// in sight area   obj.gameObject.GetComponent<Slots>() != null
        hitArea = Physics.OverlapSphere(transform.position, sightRange, playerIDMask, QueryTriggerInteraction.Collide);
        foreach (Collider obj in hitArea)
        {
            //players.Add(obj.gameObject);
            //Debug.Log("added " + obj.gameObject.name + " to kill list :))");
            if (!playerInSight)
            {
                sightRange = 300f;
                playerTarget = obj.gameObject;
                targetAlreadyChanged = false;
                playerInSight = true;
            }

        }

        if (hitArea.Length == 0) ResetEncounter();

        if (!targetAlreadyChanged)
        {
            Invoke(nameof(ChangeTarget), Random.Range(3, 10));
            targetAlreadyChanged = true;
        }
    }
    void ChangeTarget() //random
    {// randomizes the target to attack from players list

        if (hitArea.Length > 0)
        {
            Debug.Log("CHANGE TARGET");
            Debug.Log("PLAYER LIST COUNT: " + hitArea.Length);

            int rnd = Random.Range(0, hitArea.Length);
            playerTarget = hitArea[rnd].gameObject;
            Debug.Log("target is now players[" + rnd + "].");
            targetAlreadyChanged = false;
        }

    }

    void ResetEncounter()
    {
        sightRange = 150f;
        health = 10000f;
        damageTakenForHide = 0;
        playerInSight = false;
        targetAlreadyChanged = true;
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
    private void DropLoot()
    {
        randomDrop.Drop();
    }
    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPosition.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    void InvokeHideEffect()
    {
        InstantiateEffect(1);
    }
    void InstantiateEffect(int i)
    {
        if (i == 0)
        {
            GameObject exp = Instantiate(attackEffect, hitPosition.position + new Vector3(0, 6, 0), Quaternion.identity) as GameObject;
            exp.GetComponent<ParticleSystem>().Play();
            ParticleSystem.MainModule particle = exp.GetComponent<ParticleSystem>().main;
            Destroy(exp, particle.duration);
        }
        else
        {
            GameObject exp = Instantiate(attackEffect, transform.position, Quaternion.identity) as GameObject;
            exp.transform.localScale *= 2;
            exp.GetComponent<ParticleSystem>().Play();
            ParticleSystem.MainModule particle = exp.GetComponent<ParticleSystem>().main;
            Destroy(exp, particle.duration);
        }
    }


}
