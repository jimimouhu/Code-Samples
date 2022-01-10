using UnityEngine;
using UnityEngine.UI;

public class ActionBarController : MonoBehaviour
{
    public Image bar;
    public bool cooldownRunning;
    public bool cdRun;
    float cdTime;
    public bool cdFinished;

    private void Awake()
    {
        cdFinished = true;
    }


    void Update()
    {

        if (cdRun)
        {
            Debug.Log("cd running");
            bar.fillAmount = 0;
            cooldownRunning = true;
        }

        if (cooldownRunning)
        {
            cdRun = false;
            bar.fillAmount += 1.0f / cdTime * Time.deltaTime;

            if (bar.fillAmount == 1.0f)
            {
                cdFinished = true;
                cooldownRunning = false;
            }
        }



    }

    public void EnableCooldown(float duration)
    {
        cdFinished = false;
        cdTime = duration;
        cdRun = true;
        Debug.Log("enabled cd");
    }



}

