using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDialog : MonoBehaviour
{
    public string messageString;
    public Text message;


    private void Start()
    {
        PlayStatus.instance.Intro.AddListener(StartDialog);
    }
    void StartDialog()
    {
        StartCoroutine(WriteDialog());
    }

    IEnumerator WriteDialog()
    {
        message.enabled = true;
        var pause = new WaitForSeconds(0.01f);
        var waitForThreeSeconds = new WaitForSeconds(3f);
        for (int i = 0; i < messageString.Length; i++)
        {
            message.text += messageString[i];
            yield return pause;
        }

        yield return waitForThreeSeconds;
        message.enabled = false;
        message.text = "";
    }
}
