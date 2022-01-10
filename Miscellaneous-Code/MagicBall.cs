using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MagicBall : MonoBehaviourPunCallbacks
{
    AnimationController animation;
    public Transform shootPoint;
    public Image CooldownCircle;
    public GameObject bombPrefab;
    bool loading;
    public float damage = 5f;
    public float blastRadius = 20f;
    // Start is called before the first frame update
    void Start()
    {
        animation = transform.parent.parent.GetComponent<AnimationController>();
    }

    private void OnDisable()
    {
        loading = false;
        if (CooldownCircle)
        {
            CooldownCircle.fillAmount = 0;
            CooldownCircle.gameObject.SetActive(false);
        }
    }

    public void StartManeuver()
    {
        if (!loading)
            ShootSpell();
    }

    public void ShootSpell()
    {
        animation.StartCoroutine("useWand");
        Vector3 direction = Camera.main.transform.forward;
        float[] startPos = { shootPoint.position.x, shootPoint.position.y, shootPoint.position.z };
        float[] vel = { direction.x, direction.y, direction.z };
        GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("InstantiateMagicBall", RpcTarget.All, startPos, vel);
        StartCoroutine(Cooldown(1.5f));
    }

    IEnumerator Cooldown(float duration)
    {

        loading = true;
        CooldownCircle.gameObject.SetActive(true);
        CooldownCircle.fillClockwise = false;
        CooldownCircle.fillAmount = 0;
        float time = 0;
        while (time < duration)
        {
            CooldownCircle.fillAmount += Time.deltaTime / duration;
            time += Time.deltaTime;
            yield return null;
        }
        loading = false;
        CooldownCircle.gameObject.SetActive(false);
    }

    public void LaunchMissile(float[] startPos, float[] direction, GameObject player)// Igorin lisäys se photon IDs
    {
        GameObject obj = Instantiate(bombPrefab, new Vector3(startPos[0], startPos[1], startPos[2]), Quaternion.identity) as GameObject;
        obj.GetComponent<ExplosiveTrigger>().player = player;// Igorin lisäys
        ParticleSystem parts = obj.GetComponent<ParticleSystem>();
        float lifeDuration = parts.duration + parts.startLifetime;
        Destroy(obj, lifeDuration);
        obj.GetComponent<Rigidbody>().velocity = new Vector3(direction[0], direction[1], direction[2]) * 50.0f;
        //CooldownCircle.gameObject.SetActive(false);
        loading = false;
    }
}
