using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ExplosionCollision : MonoBehaviourPunCallbacks
{

    // Tämä koodi vaikuttaa Firespellin räjähdyksen jälkeisiin kipinöihin + frost spell aoe + kipinät
    float doDamage;
    GameObject enemyHit;
    bool frost;
    public GameObject Owner;


    void OnParticleCollision(GameObject other)
    {

        switch (transform.tag)
        {
            case "Fire":
                    doDamage = (GameStatus.instance.skillsAndStats.spellCastingLevel / 10);
                frost = false;
                break;
            case "Frost":
                    doDamage = (GameStatus.instance.skillsAndStats.spellCastingLevel / 30);
                frost = true;
                break;
        }

        enemyHit = other;
        enemyHit.transform.GetComponent<TakeDamage>().DoDamageToEnemy(doDamage, Owner.GetComponent<PhotonView>().ViewID);

        if (frost) 
        {
            //enemyHit.transform.GetComponent<LStatePatternEnemy>().ExecGlaze();

            /*     // Glaze jäädytys efekti johonkin AI koodeihin
            public void ExecGlaze()
            {
            StartCoroutine(Glaze(2));
            }
            IEnumerator Glaze(float duration)
            {
            float original = navMeshAgent.speed;
            float glaze = 0.0f;
            navMeshAgent.speed = glaze;
            float time = 0;
            while (time < duration)
            {
            time += Time.deltaTime;
            yield return null;
            }
            navMeshAgent.speed = original;
            } */
        }

    }



}
