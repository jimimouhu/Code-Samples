using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class BattleChicken : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public Animation anime;
    public NavMeshAgent agent;
    bool aggro;
    public LayerMask enemyLayer;
    public float attackRange;
    public GameObject explosion;
    public float doDamage;
    public float lifeTime;
    bool dead;
    int photonID;
    GameObject caster;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        doDamage = 20 + (GameStatus.instance.skillsAndStats.spellCastingLevel / 2);
        lifeTime = 0;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        anime.Play("Armature_Run");
        // aika jonka jälkeen kana poksahtaa pois
        lifeTime += Time.deltaTime;
        if (lifeTime >= 10.0f && !dead)
        {
            photonView.RPC("InstantiateExplosion", RpcTarget.All, transform.position, Quaternion.identity);
        }

        if (!aggro && !dead) { FollowPlayer(); }

        Collider[] enemiesFound = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        foreach (Collider enemy in enemiesFound)
        {
            if (!enemy.isTrigger && !dead)
            {
                aggro = true;
                agent.SetDestination(enemy.transform.position);
            }

        }
        Collider[] enemiesHit = Physics.OverlapSphere(transform.position, 2.0f, enemyLayer);
        foreach (Collider enemy in enemiesHit)
        {
            if (!enemy.isTrigger)
            {
                photonView.RPC("InstantiateExplosion", RpcTarget.All, transform.position, Quaternion.identity);
                enemy.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, caster.GetComponent<PhotonView>().ViewID);
                Collider[] enemiesWave = Physics.OverlapSphere(transform.position, 4.0f, enemyLayer);
                foreach (Collider enemyInWave in enemiesWave)
                {
                    if (!enemyInWave.isTrigger)
                        enemyInWave.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage / 2, caster.GetComponent<PhotonView>().ViewID);
                }
                
            }

        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] obj = info.photonView.InstantiationData;
        photonID = (int)obj[0];
        caster = PhotonView.Find(photonID).gameObject;
    }

    void FollowPlayer()
    {
        if (caster != null && !dead) agent.SetDestination(caster.transform.position + new Vector3(Random.Range(-3.0f, 3.0f), 0, Random.Range(3.0f, -3.0f)));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, 2.0f);
    }

    [PunRPC]
    void InstantiateExplosion(Vector3 startPos, Quaternion rotation)
    {
        GameObject spell = Instantiate(explosion, startPos, rotation) as GameObject;
        dead = true;
        if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
        ParticleSystem parts = spell.GetComponent<ParticleSystem>();
        float lifeDuration = parts.duration + parts.startLifetime;
        Destroy(spell, lifeDuration);
    }
}
