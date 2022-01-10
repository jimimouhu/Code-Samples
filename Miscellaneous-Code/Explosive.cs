using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviourPunCallbacks
{
    //This script is for HomingMissile.cs projectile only
    public GameObject player;
    public int OwnerPhotonID;// Igorin lisäys se photon ID
    public LayerMask enemy;
    public GameObject fireExplosion;
    HomingMissile hm;

    private void Start()
    {
        //GameObject hmLauncher = GameObject.Find("Grenade");
        hm = player.GetComponent<Slots>().backCannonSlot.transform.Find("Grenade").GetComponent<HomingMissile>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag != "Player" && other.tag != "Untagged") 
        if (!other.isTrigger)
        {
            Collider[] hitArea = Physics.OverlapSphere(transform.position, hm.blastRadius);
            foreach (Collider obj in hitArea)
            {
                if (obj.transform.GetComponent<TakeDamage>())
                    obj.transform.GetComponent<TakeDamage>().DoDamage(hm.damage, player);// Igorin lisäys se photon ID
                else if (obj.gameObject.GetComponent<HitboxHandler>())
                {
                    obj.gameObject.GetComponent<HitboxHandler>().parent.GetComponent<TakeDamage>().DoDamage(hm.damage, player);// Igorin lisäys se photon ID
                }
                else if (obj.transform.CompareTag("Boss1"))
                    obj.transform.GetComponent<Boss1>().TakeDamage(hm.damage);
                else if (obj.transform.CompareTag("Boss1Minion"))
                    obj.transform.GetComponent<Boss1Minion>().TakeDamage(hm.damage);
            }

            GameObject exp = Instantiate(fireExplosion, gameObject.transform.position + new Vector3(0, 1, 0), Quaternion.identity) as GameObject;
            exp.transform.localScale = exp.transform.localScale * (hm.blastRadius / 3.33f);
            exp.GetComponent<ParticleSystem>().Play();
            ParticleSystem.MainModule particle = exp.GetComponent<ParticleSystem>().main;
            Destroy(exp, particle.duration);
            Destroy(gameObject);
        }


    }

}
