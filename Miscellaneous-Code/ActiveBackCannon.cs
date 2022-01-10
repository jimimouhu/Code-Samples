using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveBackCannon : MonoBehaviour
{
    //public Shoot minigun; // 1
    public HomingMissile grenade; // 2
    //public RayGun raygun;
    // public RayGun raygun;
    public int equipIndex;
    public int equipState;
    public BackBar backBar;


    public void Start()
    {
        ChangeBarText();
    }
    public void Aim()
    {
        //if (equipIndex == 0)
        //{
        //    Debug.Log("EI ASETTA :D");
        //    return;
        //}


        //if (equipIndex == 1) minigun.Fire();
        //if (equipIndex == 2) 
        //if (equipIndex == 3) raygun.StartManeuver();
        if (equipIndex == 1) grenade.active = true;
    }
    public void Shoot()
    {
        grenade.ShootGrenade();
        if (equipIndex == 1) grenade.active = false;
    }

    private void Update()
    {
        if (equipState != equipIndex)
        {
            equipState = equipIndex;
            backBar.GetComponent<BackBar>().ChangeIcon(equipIndex);
        }
        if (equipState == 0) backBar.gameObject.SetActive(false);
        else backBar.gameObject.SetActive(true);




    }

    public void ChangeGunIcon()
    {
        backBar.GetComponent<BackBar>().ChangeIcon(equipIndex);
    }
    public void ChangeBarText()
    {
        if (backBar.GetComponent<BackBar>().GetBarText() != GameStatus.global.keySaveData.keybinds[1].keyCode.ToString())
        {
            string KeyName;
            KeyName = GameStatus.global.keySaveData.keybinds[1].keyCode.ToString();

            if (KeyName.Contains("Mouse0")) KeyName = "Mouse 1";
            if (KeyName.Contains("Mouse1")) KeyName = "Mouse 2";
            if (KeyName.Contains("Mouse2")) KeyName = "Mouse 3";
            if (KeyName.Contains("Mouse3")) KeyName = "Mouse 4";
            if (KeyName.Contains("Mouse4")) KeyName = "Mouse 5";

            backBar.GetComponent<BackBar>().ChangeText(KeyName);
        }
    }

}
