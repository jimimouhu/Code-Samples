using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMessage : MonoBehaviour
{
    public GameObject frontLineMsg;
    public GameObject testMsg;

    public void RunMessage(string msg)
    {
        switch (msg)
        {
            case "frontline":
                StartCoroutine(MessagePopUp(frontLineMsg));
                break;
        }
    }


    IEnumerator MessagePopUp(GameObject msg)
    {
        float time = 0;
        float duration = 0.5f;

        msg.SetActive(true);
        Vector3 minSize = new Vector3(0, 0, 0);
        Vector3 maxSize = new Vector3(5, 5, 0);
        msg.transform.localScale = minSize;

        while (time < duration)
        {
            time += Time.deltaTime;
            msg.transform.localScale = Vector3.Lerp(minSize, maxSize, time / duration);
            yield return null;
        }
        yield return new WaitForSeconds(2);

        StartCoroutine(MessagePopDown(msg));

    }

    IEnumerator MessagePopDown(GameObject msg)
    {
        float time = 0;
        float duration = 0.5f;

        Vector3 minSize = new Vector3(0, 0, 0);
        Vector3 maxSize = msg.transform.localScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            msg.transform.localScale = Vector3.Lerp(maxSize, minSize, time / duration);
            yield return null;
        }

        msg.SetActive(false);
    }

    

}
