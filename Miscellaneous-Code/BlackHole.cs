using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackHole : MonoBehaviourPunCallbacks
{
    AnimationController animation;
    public LayerMask groundMask;
    public GameObject cursor;
    GameObject landingPointCursor;
    // public GameObject explosion;
    public GameObject bombPrefab;
    public Transform shootPoint;
    public Image CooldownCircle;
    bool loading;
    public float radius;
    public float pullStrenth;

    private void Start()
    {
        animation = transform.parent.parent.GetComponent<AnimationController>();
        landingPointCursor = Instantiate(cursor, transform.position, Quaternion.Euler(90, 0, 0));
        landingPointCursor.SetActive(false);
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


    public void AimSpell()
    {
        if (!loading)
        {
            landingPointCursor.SetActive(true);
            RaycastHit hit;
            if (Physics.Raycast(shootPoint.position + Camera.main.transform.TransformDirection(Vector3.up) * 2, Camera.main.transform.forward, out hit, 800, groundMask))
            {
                landingPointCursor.transform.position = hit.point + Vector3.up * 0.1f;
                landingPointCursor.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
        }

    }

    public void ShootSpell()
    {
        if (!loading)
        {
            Vector3 landing = landingPointCursor.transform.position;
            GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("InstantiateBlackHole", RpcTarget.All, landing, radius, pullStrenth);
            landingPointCursor.SetActive(false);
            StartCoroutine(Cooldown(1.5f));
            animation.StartCoroutine("useWand");
        }

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

    public void LaunchMissile(Vector3 landingPoint, float rad, float pullStrn)
    {
        GameObject obj = Instantiate(bombPrefab, landingPoint, bombPrefab.transform.rotation) as GameObject;
        obj.GetComponent<PullObject>().radius = rad;
        obj.GetComponent<PullObject>().pullStrength = pullStrn;
        obj.GetComponent<ParticleSystem>().Play();
        ParticleSystem.MainModule particle = obj.GetComponent<ParticleSystem>().main;
        Destroy(obj, particle.duration);
    }


}
