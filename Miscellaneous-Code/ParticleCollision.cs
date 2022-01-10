using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParticleCollision : MonoBehaviourPunCallbacks
{
    // TURHA SCRIPT ATM

    float doDamage;
    public GameObject effect;
    public GameObject aoe;
    private GameObject exp;
    public GameObject Owner;
    private void Start()
    {
        doDamage = 10 + (GameStatus.instance.skillsAndStats.spellCastingLevel / 2);
        gameObject.GetComponentInChildren<ExplosionCollision>().Owner = Owner;
    }


    void OnCollisionEnter(Collision col)
    {
            if (col.gameObject.layer == 10)
            {
                //Debug.Log("Spelli osui! Damage: " + doDamage);
                Destroy(gameObject);
                exp = Instantiate(effect, gameObject.transform.position, Quaternion.identity) as GameObject;
                exp.GetComponent<ExplosionCollision>().Owner = Owner; 
                exp.GetComponent<ParticleSystem>().Play();
                ParticleSystem.MainModule particle = exp.GetComponent<ParticleSystem>().main;
                Destroy(exp, particle.duration);
                col.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, Owner.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}
