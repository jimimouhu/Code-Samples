using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public GameObject leftBtn;
    public GameObject rightBtn;
    PlayStatus playStatus;
    public static PlayerController instance;
    public int instanceID;
    public int choiseID = 0; // 0 = no choise, 1 = left, 2 = right


    private void Start()
    {
        playStatus = GameObject.Find("Boss").GetComponent<PlayStatus>();
        instance = this;
    }
    public void StartChoosing(int index) // instanceID tähän
    {
        // Choose menu starts here
        StartCoroutine(Choose(index));
    }
    public void StartChoise(int index) // instanceID tähän
    {
        // Choose menu starts here
        StartCoroutine(Choise(index));
    }

    public void StartMoving(GameObject obj, Vector3 location, float duration, GameObject target)
    {
        StartCoroutine(MoveObject(obj, location, duration, target));
    }

    public IEnumerator Choose(int index)  // instanceID tähän
    {
        //if (index != 0)
        //{

            yield return new WaitForSeconds(Random.Range(1, 5));

            //this is place holder, make the player press the choice and send the value later
            int rnd = (int)Random.Range(1, 3);

            playStatus.ReceiveChoices(index, rnd);
        //} 
        //else
        //{
        //    // näytä buttonit pelaajan ruudulla
        //    // odota kunnes painettu tai aikaa kulunut 5 sec (random valinta jos ei paina)
        //    // playStatus.ReceiveChoices(instanceID tähän, 1 tai 2)
        //    // poista näppäimet ruudulta
        //}

    }
    public IEnumerator Choise(int index)
    {
        choiseID = 0;
        Debug.Log("JIMMY INDEX: " + instanceID);
        leftBtn.SetActive(true);
        rightBtn.SetActive(true);
        float time = 0;
        float duration = 5.0f;
        while (time < duration && choiseID == 0)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Sending ChoiseID: " + choiseID + " from index: " + index);
        if (choiseID == 0)
            playStatus.ReceiveChoices(index, (int)Random.Range(1, 3));
        else
            playStatus.ReceiveChoices(index, choiseID);

        leftBtn.SetActive(false);
        rightBtn.SetActive(false);
    }

    // for Button
    public void ModifyChoiseID(int value)
    {
        if (choiseID == 0)
            choiseID += value;

        if (value == 0)
            choiseID = 0;

        Debug.Log("ChoiseID on nyt: " + choiseID);
    }

    public IEnumerator MoveObject(GameObject obj, Vector3 location, float duration, GameObject target)
    {
        Vector3 startLocation = obj.transform.position;
        float time = 0;
        while (time < duration)
        {
            Vector3 height = new Vector3(0, obj.transform.position.y, 0);
            obj.transform.position = Vector3.Lerp(startLocation, location, time / duration);
            obj.transform.LookAt(target.transform.position+height);
            time += Time.deltaTime;
            yield return null;
        }

    }



}
