using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "PlayerObject")]
public class PlayerObject : ScriptableObject
{
    [System.Serializable]
    public class PlayerObj
    {
        public WieldedWeapon weapon;
        public int choice = 0;
        public int ID;
        public int honor;
        public int goldCoins;
        public int damageDone = 0;
        public string name;
        public GameObject model;
        public bool godMode;
        public bool isDead;
        public int hauntingDamageBonus = 0;

        public GameObject powerUpGameObject; // Cleaned up after every round
        public bool sendPowerUpToFight;
    }

    public PlayerObj playerObj;

}
