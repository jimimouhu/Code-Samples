using UnityEngine;
using Photon.Pun;


public class ShadowSphere : MonoBehaviourPunCallbacks
{

    // TÄMÄ ON NYT KAIKKIEN DAMAGE ISKUJEN COLLIDER SCRIPT, PARTICLECOLLISION.CS = TURHA
    float doDamage;
    public LayerMask enemyLayer;
    public GameObject Owner;
    public GameObject explosion_fire;
    public GameObject explosion_frost;
    private GameObject exp;
    private float radius;
    bool goOn;
    public bool thunder;

    private void Start()
    {
        if (gameObject.tag == "Frost")
        {
            gameObject.GetComponentInChildren<ExplosionCollision>().Owner = Owner;
        }
    }
    void Update()
    {
        // Shadow & Thunder
        if (Owner != null)
        {
            goOn = true;
        }

        switch (gameObject.tag)
        {
            case "Fire":
                radius = 1.0f;
                doDamage = 10 + (GameStatus.instance.skillsAndStats.spellCastingLevel / 2);
                Collider[] fire = Physics.OverlapSphere(transform.position, radius, enemyLayer);
                foreach (Collider enemy in fire)
                {
                    if (!enemy.isTrigger && goOn)
                    {
                        enemy.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, Owner.GetComponent<PhotonView>().ViewID);
                        InstantiateExplosion(explosion_fire);
                        
                    }
                } 
                break;
            case "Thunder":
                radius = 4.0f;
                doDamage = GameStatus.instance.skillsAndStats.spellCastingLevel / 100; 
                Collider[] thunder = Physics.OverlapSphere(transform.position, radius, enemyLayer);
                foreach (Collider enemy in thunder)
                {
                    if (!enemy.isTrigger && goOn)
                    {
                        enemy.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, Owner.GetComponent<PhotonView>().ViewID);
                    }
                }
                break;
            case "Frost":
                radius = 0.5f;
                doDamage = 10 + (GameStatus.instance.skillsAndStats.spellCastingLevel / 2);
                Collider[] frost = Physics.OverlapSphere(transform.position, radius, enemyLayer);
                foreach (Collider enemy in frost)
                {
                    if (!enemy.isTrigger && goOn)
                    {
                        enemy.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, Owner.GetComponent<PhotonView>().ViewID);
                        InstantiateExplosion(explosion_frost);
                    }
                }
                break;
            case "Shadow":
                radius = 5.0f;
                doDamage = GameStatus.instance.skillsAndStats.spellCastingLevel / 200;
                Collider[] shadow = Physics.OverlapSphere(transform.position, radius, enemyLayer);
                foreach (Collider enemy in shadow)
                {
                    if (!enemy.isTrigger && goOn)
                    {
                        enemy.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, Owner.GetComponent<PhotonView>().ViewID);
                        Owner.gameObject.GetComponent<PlayerController>().HurtPlayer(-doDamage / 2);
                    }
                }
                break;
        }
    }

    void InstantiateExplosion(GameObject effect)
    {
        Destroy(gameObject);
        exp = Instantiate(effect, gameObject.transform.position, Quaternion.identity) as GameObject;
        exp.GetComponent<ExplosionCollision>().Owner = Owner;
        exp.GetComponent<ParticleSystem>().Play();
        ParticleSystem.MainModule particle = exp.GetComponent<ParticleSystem>().main;
        Destroy(exp, particle.duration);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }

}
