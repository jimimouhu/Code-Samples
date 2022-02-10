using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "PlayerObject")]
public class PlayerObject : ScriptableObject
{
    [System.Serializable]
    public class PlayerObj
    {
        public int choice = 0;
        public int ID;
        public int honor;
        public int goldCoins;
        public string name;
        public GameObject model;
    }

    public PlayerObj playerObj;

}
