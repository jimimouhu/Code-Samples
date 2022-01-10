using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveShoulderCannon : MonoBehaviour
{
    public Shoot minigun; // 1
    public HomingMissile grenade; // 2
    public RayGun raygun;
    // public RayGun raygun;
    public int equipIndex;
    public int equipState;
    public ShoulderBar shoulderBar;

    public void Start()
    {
        ChangeBarText();
    }

    public void Shoot()
    {
        if (equipIndex == 0) 
        {
            Debug.Log("EI ASETTA :D");
            return;
        }


        if (equipIndex == 1) minigun.shooting = true;
        if (equipIndex == 3) 
        {
            raygun.loading = true;
            raygun.loadingEffectOn = true;
        }

    }

    public void StopShoot()
    {
        if (equipIndex == 1) minigun.shooting = false;
        if (equipIndex == 3) 
        {
            raygun.loading = false;
            raygun.shooting = true;
        } 

    }
    private void Update()
    {
        if (equipState != equipIndex)
        {
            equipState = equipIndex;
            shoulderBar.GetComponent<ShoulderBar>().ChangeIcon(equipIndex);
        }

        if (equipState == 0) shoulderBar.gameObject.SetActive(false);
        else shoulderBar.gameObject.SetActive(true);


    }

    public void ChangeGunIcon()
    {
        shoulderBar.GetComponent<ShoulderBar>().ChangeIcon(equipIndex);
    }

    public void ChangeBarText()
    {
        if (shoulderBar.GetComponent<ShoulderBar>().GetBarText() != GameStatus.global.keySaveData.keybinds[0].keyCode.ToString())
        {
            string KeyName;
            KeyName = GameStatus.global.keySaveData.keybinds[0].keyCode.ToString();

            if (KeyName.Contains("Mouse0")) KeyName = "Mouse 1";
            if (KeyName.Contains("Mouse1")) KeyName = "Mouse 2";
            if (KeyName.Contains("Mouse2")) KeyName = "Mouse 3";
            if (KeyName.Contains("Mouse3")) KeyName = "Mouse 4";
            if (KeyName.Contains("Mouse4")) KeyName = "Mouse 5";

            shoulderBar.GetComponent<ShoulderBar>().ChangeText(KeyName);
        }
    }

}
