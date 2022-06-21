using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HauntingManager : MonoBehaviour
{
    public bool haunting = false;

    public bool hauntTargetAlive = false;
    public PlayerObject hauntingTarget;


    public PlayStatus playStatus;
    public Image backgroundImage;
    public Image haamu1Image;
    public Image haamuTextImage;

    public Sprite haamu1;   // Grin
    public Sprite haamu2;   // Confused
    public Sprite haamu3;   // Sunglasses 
    public Sprite haamu4;   // Crazy smile


    public GameObject hauntingDamageButtons;

    public List<PlayerObject> listOfFrontlinePlayers;

    public Transform cameraTransform;
    public Vector3 cameraDefaultPosition;

    private void Start()
    {
        haunting = false;
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        cameraDefaultPosition = new Vector3(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z);
    }

    private void Update()
    {
        /*
        if (haunting == true)
        {
            if(hauntTargetAlive == false)
            {
                StartCoroutine(playStatus.RotateCameraInTime(cameraDefaultPosition, listOfFrontlinePlayers[0].playerObj.model.transform, 1f));
            }
        }
        */
    }


    // TÄYTYY MUISTAA DISABLOIDA HAUNTINGIN EXTRA UI KUN EI KÄYTETÄ, JOTTA VÄLTYTÄÄN UI KOSKETUS ERROREILTA!


    public void StartHaunting(float inSeconds)
    {
        haunting = true;

        Debug.Log("HAAMUILU ALKAA!");
        // Player died, waiting a second to see the death and then starting haunting
        StartCoroutine(WaitSomeTime(inSeconds));

        // Haunting starting here
        // Some cool screen effect
        StartCoroutine(FadeBackgroundImage(false));
        HaamuEntrance();
        haamuTextImage.gameObject.SetActive(true);
        hauntingDamageButtons.SetActive(true);
    }


    // This is called in PlayStatus to update the list of frontline players after each round
    // We can then use the camera to zoom in onto Frontline players and switch views with arrow buttons to choose a player to haunt
    public void UpdateFrontlineList(List<PlayerObject> frontlineList)
    {
        listOfFrontlinePlayers.Clear();

        for (int i = 0; i < frontlineList.Count; i++)
        {
            listOfFrontlinePlayers.Add(frontlineList[i]);
        }
    }

    /*
    public void GiveHauntingBonusDamage(int bonusDamage)
    {
        hauntingTarget.playerObj.hauntingDamageBonus = bonusDamage;
    }
    */

    public void ChangeHauntingTarget(PlayerObject player)
    {
        hauntingTarget = player;
    }

    public void GiveHauntingBonusDamage1()
    {
        hauntingTarget.playerObj.hauntingDamageBonus = 1;
        Debug.Log("PELAAJAA BOOSTATTU HAAMUILULLA  ID: " + hauntingTarget.playerObj.ID);

        haamu1Image.sprite = haamu1;
    }
    public void GiveHauntingBonusDamage2()
    {
        hauntingTarget.playerObj.hauntingDamageBonus = 2;

        haamu1Image.sprite = haamu3;
    }
    public void GiveHauntingBonusDamage3()
    {
        hauntingTarget.playerObj.hauntingDamageBonus = 3;

        haamu1Image.sprite = haamu4;
    }


    public void ResetHauntingBonusDamage()
    {
        // MULTIPLAYER PELISSÄ TÄYTYY VAIN VÄHENTÄÄ OMALLA TAI KEKSIÄ TOINEN RATKAISU!
        hauntingTarget.playerObj.hauntingDamageBonus = 0;

        haamu1Image.sprite = haamu2;
    }


    // Cycle through Frontline players
    public void LeftButtonClick()
    {
        // Move camera to look at player transform

    }

    public void RightButtonClick()
    {

    }


    // Confirm the Frontline player to be haunted
    public void ConfirmHauntedButtonClick()
    {

    }

    public void ResetCameraPosition()
    {

    }


    // Tällä funktiolla haamu sisään ja ulos
    public void HaamuEntrance()
    {
        Animator animator = haamu1Image.GetComponent<Animator>();

        if (animator != null)
        {
            bool haamuIsIn = animator.GetBool("haamu_in");

            animator.SetBool("haamu_in", !haamuIsIn);
        }

    }


    // Perus feidaus! Vois olla animaationa ehkä kontrolloidumpi :D Anyways it's a placeholder for now
    IEnumerator FadeBackgroundImage(bool fade)
    {
        backgroundImage.gameObject.SetActive(true);
        // fade from opaque to transparent
        if (fade)
        {
            //Haamun animaatio väärinpäin, eli poistuu
            HaamuEntrance();

            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                backgroundImage.color = new Color(1, 1, 1, i);
                yield return null;
            }


            backgroundImage.gameObject.SetActive(false);
            haamu1Image.gameObject.SetActive(false);
            haamuTextImage.gameObject.SetActive(false);
            hauntingDamageButtons.SetActive(false);
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha, not fully opaque as we want the screen to be viewable
                backgroundImage.color = new Color(1, 1, 1, i*0.9f);
                yield return null;
            }
            haamu1Image.gameObject.SetActive(true);
        }
    }


    IEnumerator WaitSomeTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

}
