using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTrigger : MonoBehaviour
{
    //This script is for  MagicBall.cs projectile only
    public GameObject player;
    public int OwnerPhotonID; // Igorin lisäys
    public LayerMask enemy;
    public GameObject explosion;
    MagicBall mb;

    private void Start()
    {
        mb = player.GetComponent<Slots>().magicSlot.transform.Find("MagicBall").GetComponent<MagicBall>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag != "Player" && other.tag != "Untagged") 
        if(!other.isTrigger)
        {
            Vector3 explosionPos = transform.position;
        
            Explosion(explosionPos);
            GameObject exp = Instantiate(explosion, gameObject.transform.position, explosion.transform.rotation) as GameObject;
            exp.GetComponent<ParticleSystem>().Play();
            ParticleSystem.MainModule particle = exp.GetComponent<ParticleSystem>().main;
            Destroy(exp, particle.duration);
            Destroy(gameObject);
        }
        if (other.transform.GetComponent<TakeDamage>())
            other.transform.GetComponent<TakeDamage>().DoDamage(mb.damage, player); // Igorin lisäys se photon ID

        if (other.transform.CompareTag("Boss1"))
            other.transform.GetComponent<Boss1>().TakeDamage(mb.damage);

        if (other.transform.CompareTag("Boss1Minion"))
            other.transform.GetComponent<Boss1Minion>().TakeDamage(mb.damage);
    }

    void Explosion(Vector3 position)
    {
        Collider[] hitArea = Physics.OverlapSphere(position, mb.blastRadius, enemy);
        foreach (Collider obj in hitArea)
        {
            if (obj.transform.GetComponent<TakeDamage>())
                obj.transform.GetComponent<TakeDamage>().DoDamage(mb.damage, player);// Igorin lisäys se photon ID

            if (obj.transform.CompareTag("Boss1"))
                obj.transform.GetComponent<Boss1>().TakeDamage(mb.damage);

            if (obj.transform.CompareTag("Boss1Minion"))
                obj.transform.GetComponent<Boss1Minion>().TakeDamage(mb.damage);
        }
    }
}
