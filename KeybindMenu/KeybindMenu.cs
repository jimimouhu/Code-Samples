using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeybindMenu : MonoBehaviour
{
    public GameObject shoulderCannon;
    public GameObject backCannon;
    GameObject player;
    public Text infoText;
    private InputManager inputManager;
    public Button[] btn;
    int btnIndex;
    bool inputting;

    public bool showKeys;

    public Button saveButton;
    public Slider mouseSensitivitySlider;

    // Store the unchanged buttons if the user decides to not save:
    KeyCode[] oldKeys = new KeyCode[11];
    //Keybindings.KeybindCheck[] oldKeys = new Keybindings.KeybindCheck[11];

    private void OnEnable()
    {
        if (GameStatus.global.myCharacter) 
        {
            player = GameStatus.global.myCharacter;
            shoulderCannon = player.GetComponent<Slots>().ShoulderCannonSlot;
            backCannon = player.GetComponent<Slots>().backCannonSlot;
        } 

        if (showKeys)
        {
            for (int i = 0; i < btn.Length; i++)
            {
                string KeyName;
                KeyName = inputManager.ReturnKeyInfo(i).ToString();

                if (KeyName.ToString().Contains("Mouse0")) KeyName = "Mouse 1";
                if (KeyName.ToString().Contains("Mouse1")) KeyName = "Mouse 2";
                if (KeyName.ToString().Contains("Mouse2")) KeyName = "Mouse 3";
                if (KeyName.ToString().Contains("Mouse3")) KeyName = "Mouse 4";
                if (KeyName.ToString().Contains("Mouse4")) KeyName = "Mouse 5";


                btn[i].transform.GetChild(1).GetComponent<Text>().text = KeyName;
            }
            for (int i = 0; i < GameStatus.global.keySaveData.keybinds.Length; i++)
            {
                oldKeys[i] = GameStatus.global.keySaveData.keybinds[i].keyCode;

            }
        }

    }

    private void OnDisable()
    {
        if (showKeys)
        {
            for (int i = 0; i < oldKeys.Length; i++)
            {
                int resState = inputManager.ReserveKey(oldKeys[i], i);
                inputManager.SetKeyAction(i, oldKeys[i]);
            }
        }

        if (GameStatus.global.myCharacter)
        {
            shoulderCannon.GetComponent<ActiveShoulderCannon>().ChangeBarText();
            backCannon.GetComponent<ActiveBackCannon>().ChangeBarText();
        }


    }

    private void Awake()
    {
        inputManager = InputManager.instance;
    }

    private void Start()
    {
        inputting = false;
        infoText.text = "Pick action to assign a key for it"; // btn.text
        infoText.enabled = true;
        saveButton.onClick.AddListener(delegate 
        {
            GameStatus.global.SaveKeys();
            for (int i = 0; i < GameStatus.global.keySaveData.keybinds.Length; i++)
            {
                oldKeys[i] = GameStatus.global.keySaveData.keybinds[i].keyCode;
            }
            if (GameStatus.global.uiManager)
            {
                CrystalSlots crystalSlot = GameStatus.global.uiManager.transform.Find("CrystalSlots").GetComponent<CrystalSlots>();
                crystalSlot.btn1.GetComponentInChildren<Text>().text = GameStatus.global.keySaveData.keybinds[6].keyCode.ToString();
                crystalSlot.btn2.GetComponentInChildren<Text>().text = GameStatus.global.keySaveData.keybinds[7].keyCode.ToString();
                crystalSlot.btn3.GetComponentInChildren<Text>().text = GameStatus.global.keySaveData.keybinds[8].keyCode.ToString();
            }
            
            
        });
        mouseSensitivitySlider.value = GameStatus.global.keySaveData.mouseSensitivityValue;
        
        // Placeholder
        if (showKeys)
        {
            for (int i = 0; i < btn.Length; i++)
            {
                string KeyName;
                KeyName = inputManager.ReturnKeyInfo(i).ToString();

                if (KeyName.ToString().Contains("Mouse0")) KeyName = "Mouse 1";
                if (KeyName.ToString().Contains("Mouse1")) KeyName = "Mouse 2";
                if (KeyName.ToString().Contains("Mouse2")) KeyName = "Mouse 3";
                if (KeyName.ToString().Contains("Mouse3")) KeyName = "Mouse 4";
                if (KeyName.ToString().Contains("Mouse4")) KeyName = "Mouse 5";
            }
            for (int i = 0; i < GameStatus.global.keySaveData.keybinds.Length; i++)
            {
                oldKeys[i] = GameStatus.global.keySaveData.keybinds[i].keyCode;

            }
        }
    }
    private void Update()
    {
        if (inputting)
        {

            infoText.text = "Press key to assign it to: " + btn[btnIndex].GetComponentInChildren<Text>().text;
            infoText.enabled = true;
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {


                if (Input.GetKey(vKey))
                {
                    string KeyName = vKey.ToString();

                    if (vKey.ToString().Contains("Mouse0")) KeyName = "Mouse 1";
                    if (vKey.ToString().Contains("Mouse1")) KeyName = "Mouse 2";
                    if (vKey.ToString().Contains("Mouse2")) KeyName = "Mouse 3";
                    if (vKey.ToString().Contains("Mouse3")) KeyName = "Mouse 4";
                    if (vKey.ToString().Contains("Mouse4")) KeyName = "Mouse 5";

                    int resState = inputManager.ReserveKey(vKey, btnIndex);
                    if (resState != -1)
                    {
                        inputting = false;
                        infoText.text = btn[btnIndex].GetComponentInChildren<Text>().text + " assigned to " + KeyName + ". "
                            + btn[resState].GetComponentInChildren<Text>().text + " is now unbound.";
                        btn[resState].GetComponent<Image>().color = Color.red;
                        btn[btnIndex].transform.GetChild(1).GetComponent<Text>().text = KeyName;
                        btn[resState].transform.GetChild(1).GetComponent<Text>().text = "";

                    } 
                    else
                    {
                        inputting = false;
                        infoText.text = btn[btnIndex].GetComponentInChildren<Text>().text + " assigned to " + KeyName;
                        btn[btnIndex].transform.GetChild(1).GetComponent<Text>().text = KeyName;
                    }
                    btn[btnIndex].GetComponent<Image>().color = Color.white;
                    inputManager.SetKeyAction(btnIndex, vKey);

                }
            }
        }
        GameStatus.global.keySaveData.mouseSensitivityValue = mouseSensitivitySlider.GetComponent<Slider>().value;

    }

    public void KeyBind(int i)
    {
        infoText.text += btn[i].GetComponentInChildren<Text>().text;
        btnIndex = i;
        inputting = true;
    }


    void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToEscMenu()
    {
        transform.GetComponentInParent<UI_Manager>().UI_Pressed(ActivateUI.Window.EscMenu);
    }

}
