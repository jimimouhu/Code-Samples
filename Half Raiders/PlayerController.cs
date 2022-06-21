using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Boss boss;
    public WieldedWeapon weapon;
    public GameObject emoteBar;
    public Image choiceTimeSlider;
    public Image teamHpImage;
    public GameObject choiceScreen;
    public GameObject leftBtn;
    public GameObject rightBtn;
    public PlayStatus playStatus;
    public static PlayerController instance;
    public int instanceID;
    public int choiceID = 0; // 0 = no choise, 1 = left, 2 = right
    public int hauntingDamageBonus = 0;
    private int damageToDeal = 0;
 

    // to get the damage from wielded weapon
    public WieldedWeapon ApplyWeapon(int minDamage, int maxDamage, int critChance, WieldedWeapon.ElementEnum element, bool isMyWeapon)
    {
        var po = Instantiate(weapon);


        po.element = element;
        po.minDamage = minDamage;
        po.maxDamage = maxDamage;
        po.critChance = critChance;
        if (isMyWeapon) weapon = po;
        return po;

    }

    private void Start()
    {
        instance = this;
    }

    public void StartMoving(GameObject obj, Vector3 location, float duration, GameObject target, bool crowd = default)
    {
        StartCoroutine(MoveObject(obj, location, duration, target, crowd));
    }

    //this is place holder for bots
    public void BotChoice(int index)
    {

        int rnd = (int)Random.Range(1, 3);

        playStatus.ReceiveChoices(index, rnd);
    }
    
    // for Button
    public void ModifyChoiceID(int value)
    {
        choiceID = value;
    }

    public IEnumerator ChoiceScreen(int index, bool isDead) // if player is dead, this coroutine executes the haunting mode.
    {
        if (!isDead)
        {
            choiceTimeSlider.fillAmount = 1;
            float time = 0;
            float duration = 10.0f;
            while (time < duration)
            {
                choiceTimeSlider.fillAmount = Mathf.Lerp(1.0f, 0.0f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            if (choiceID == 0)
                playStatus.ReceiveChoices(index, (int)Random.Range(1, 3));
            else
                playStatus.ReceiveChoices(index, choiceID);
        }
        else
        {
            float time = 0;
            float duration = 10.0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        GameManager.global.StartAttack.Invoke();

    }
    IEnumerator MoveObject(GameObject obj, Vector3 location, float duration, GameObject target, bool crowd = default)
    {
        if (crowd)
        {
            //obj.transform.GetChild(0).GetComponent<CrowdAnimaionController>().StartWalking();
        }

        Vector3 startLocation = obj.transform.position;
        float time = 0;
        while (time < duration)
        {
            Vector3 height = new Vector3(0, obj.transform.position.y, 0);
            obj.transform.position = Vector3.Lerp(startLocation, location, time / duration);
            obj.transform.LookAt(target.transform.position + height);
            time += Time.deltaTime;
            yield return null;
        }

        if (crowd)
        {
            //obj.transform.GetChild(0).GetComponent<CrowdAnimaionController>().StopWalking();
        }
        Debug.Log("STOPPED");
    }

    public int DamageEnemy(PlayerObject player)
    {
        damageToDeal = (int)Random.Range(player.playerObj.weapon.minDamage, player.playerObj.weapon.maxDamage + 1);

        switch (player.playerObj.weapon.element)
        {
            case WieldedWeapon.ElementEnum.Normal:
                if (CriticalStrike(player.playerObj.weapon.critChance))
                    damageToDeal = (int)Random.Range(player.playerObj.weapon.maxDamage + 1, (player.playerObj.weapon.maxDamage * 2) + 1);
                break;

            case WieldedWeapon.ElementEnum.Fire:
                if (CriticalStrike(player.playerObj.weapon.critChance + boss.fireWeakness))
                    damageToDeal = (int)Random.Range(player.playerObj.weapon.maxDamage + 1, (player.playerObj.weapon.maxDamage * 2) + 1);
                break;

            case WieldedWeapon.ElementEnum.Ice:
                if (CriticalStrike(player.playerObj.weapon.critChance + boss.iceWeakness))
                    damageToDeal = (int)Random.Range(player.playerObj.weapon.maxDamage + 1, (player.playerObj.weapon.maxDamage * 2) + 1);
                break;

            case WieldedWeapon.ElementEnum.Electric:
                if (CriticalStrike(player.playerObj.weapon.critChance + boss.electricWeakness))
                    damageToDeal = (int)Random.Range(player.playerObj.weapon.maxDamage + 1, (player.playerObj.weapon.maxDamage * 2) + 1);
                break;
        }

        damageToDeal += player.playerObj.hauntingDamageBonus;
        return damageToDeal;
    }

    bool CriticalStrike(int chance)
    {
        if (chance != 0)
        {
            int hit = (int)Random.Range(1, 101);
            if (hit <= chance)
                return true;
        }

        return false;

    }

    public void UpdateTeamHp()
    {
        teamHpImage.fillAmount = (float)((playStatus.players.Count-playStatus.deadPlayers.Count) * 100f / playStatus.players.Count) / 100f;
    }
}
