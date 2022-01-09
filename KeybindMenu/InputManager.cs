using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public GameObject inputMenu;
    public static InputManager instance;
    public GameStatus gameStatus;

    [SerializeField] private Keybindings keybindings;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(this);

        if (gameStatus.keySaveData.keybinds.Length > 0)
            keybindings.keybindChecks = gameStatus.keySaveData.keybinds;

        //GameStatus.global.StartSaving.AddListener(SendInputMappings);
    }

    public KeyCode GetKeyAction(KeyActions keyAction)
    {
        foreach(Keybindings.KeybindCheck keybindCheck in keybindings.keybindChecks)
        {
            if (keybindCheck.action == keyAction)
            {
                return keybindCheck.keyCode;
            }
        }

        return KeyCode.None;
    }

    public void SetKeyAction(int index, KeyCode keyCode)
    {
        instance.keybindings.keybindChecks[index].keyCode = keyCode;
        SendInputMappings();
    }
    public int ReserveKey(KeyCode kc, int currentIndex)
    {
        for (int i = 0; i < instance.keybindings.keybindChecks.Length; i++)
        {
            if (instance.keybindings.keybindChecks[i].keyCode == kc && i != currentIndex)
            {
                instance.keybindings.keybindChecks[i].keyCode = KeyCode.None;
                return i;
            }
        } return -1;

    }

    public bool GetKey(KeyActions key)
    {
        foreach (Keybindings.KeybindCheck keybindCheck in keybindings.keybindChecks)
        {
            if (keybindCheck.action == key)
            {
                return Input.GetKey(keybindCheck.keyCode);
            }
        }
        return false;
    }

    public bool GetKeyDown(KeyActions key)
    {
        foreach (Keybindings.KeybindCheck keybindCheck in keybindings.keybindChecks)
        {
            if (keybindCheck.action == key)
            {
                return Input.GetKeyDown(keybindCheck.keyCode);
            }
        }
        return false;
    }

    public bool GetKeyUp(KeyActions key)
    {
        foreach (Keybindings.KeybindCheck keybindCheck in keybindings.keybindChecks)
        {
            if (keybindCheck.action == key)
            {
                return Input.GetKeyUp(keybindCheck.keyCode);
            }
        }
        return false;
    }


    public void SendInputMappings()
    {
        gameStatus.keySaveData.keybinds = keybindings.keybindChecks;
    }

    public KeyCode ReturnKeyInfo(int index)
    {
        return instance.keybindings.keybindChecks[index].keyCode;
    }

}
