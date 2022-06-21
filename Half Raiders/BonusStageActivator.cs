using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusStageActivator : MonoBehaviour
{
    public PlayStatus playersinBonus;
    public Boss bosshelth;
    // Start is called before the first frame update
    void Awake()
    {
        playersinBonus.prizePool = playersinBonus.prizePool * 2;
        playersinBonus.playerCount = Random.Range(5,GameManager.global.MaxPlayersInBonusStage);
        bosshelth.maxHealth = playersinBonus.playerCount;
    }


}
