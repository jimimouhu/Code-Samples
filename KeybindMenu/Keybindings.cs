using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class Keybindings : ScriptableObject
{

    [System.Serializable]
    public class KeybindCheck
    {
        public KeyActions action;
        public KeyCode keyCode;
    }


    public KeybindCheck[] keybindChecks;

}
