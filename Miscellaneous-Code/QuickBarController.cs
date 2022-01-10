using UnityEngine;
using UnityEngine.UI;


public class QuickBarController : MonoBehaviour
{
    ActionBarController[] bars = new ActionBarController[8];
    bool[] activatedBars = new bool[8];
    int scrollWheel;
    int scrollMax;
    public Image bar;

    private void Awake()
    {
        for (int a = 0; a < bars.Length; a++)
        {
            bars[a] = GameObject.Find("bar" + a.ToString()).GetComponent<ActionBarController>();

        }
    }

    private void Start()
    {
        scrollWheel = 0;
        
    }
    void Update()
    {

        // Spellin valinta
        scrollWheel += Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel"));
        scrollWheel = Mathf.Clamp(scrollWheel, 0, 7);

        

        if (bars[scrollWheel].gameObject.activeSelf)
        {
            gameObject.GetComponent<Image>().sprite = bars[scrollWheel].GetComponent<Image>().sprite;
        }

        gameObject.GetComponent<Image>().fillAmount = bars[scrollWheel].GetComponent<Image>().fillAmount;



        // Mouse2 listener
        if (Input.GetKey(KeyCode.Mouse1) && !GameStatus.instance.player.GetComponent<PlayerController>().aiming && !GameStatus.instance.player.GetComponent<PlayerController>().uiOn)
        {
            GameStatus.instance.player.GetComponent<PlayerController>().ActionBar(scrollWheel);
        }
    }

}