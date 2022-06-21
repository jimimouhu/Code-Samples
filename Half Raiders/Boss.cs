using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    // boss side-choosing RNG. Default min-max: 1-100 (random.range 1, 101)
    public int bossRNG_min = 1;
    public int bossRNG_max = 101;

    // Boss vulnerability values
    public int fireWeakness;
    public int iceWeakness;
    public int electricWeakness;
    public bool poisonWeakness;

    public int maxHealth;
    public int currentHealth;
    public IBossEventListener listener;
    public GameObject bossObject;

    BossAnimationController anim;
    public PlayStatus playStatus;

    // Boss hp canvas
    public Image bossHPimage;


    private void Start()
    {
        anim = transform.Find("Model").transform.GetComponent<BossAnimationController>();
        currentHealth = maxHealth;
        bossHPimage.fillAmount = 1.0f;
    }
    public void ManageHealth(int value)
    {
        currentHealth += value;
        bossHPimage.fillAmount = (float)(currentHealth * 100f / maxHealth) / 100f;

        if (currentHealth <= 0)
            listener.BossDied(this);
    }

    public IEnumerator MoveBoss(Vector3 pos, float duration, System.Action<int> RndAttack)
    {
        anim.StartWalking();
        transform.forward = new Vector3(0, 0, 0);
        Vector3 basicPos = transform.position;
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(basicPos, pos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        anim.StopWalking();
        RndAttack(BossRndAttack());
    }

    public IEnumerator MoveBoss(Vector3 pos, float duration)
    {
        anim.StartWalking();
        transform.forward = new Vector3(0, 0, 0);
        Vector3 basicPos = transform.position;
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(basicPos, pos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        anim.StopWalking();
    }

    public int BossRndAttack()
    {
        int sideRnd = (int)Random.Range(bossRNG_min, bossRNG_max);
        if(sideRnd <= 50)
        {
            anim.TurnLeft();
        }
        else
        {
            //anim.TurnLeft(); // Always turn left because debugging
            anim.TurnRight();
        }
        return sideRnd;
    }

}
