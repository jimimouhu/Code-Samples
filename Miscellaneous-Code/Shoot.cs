using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviourPunCallbacks
{
    Camera cam;
    public GameObject playerParentObj;

    public AudioClip[] audios;
    AudioSource audioSource;

    public bool myCharacter;
    public LayerMask rayTargets;

    public ParticleSystem flash;
    public GameObject impact;
    public Image loadCircle;
    public Text ohText;
    public float shootRange = 100f;
    private float interval = 0.0f;
    private float heat = 0;

    public LineRenderer lr;
    
    // Stats
    public float fireRate = 1000f; // arvo 200-1000, suurempi = nopeampi nannitys
    float fireTime; // fireTime = 100 / fireRate
    public float heatRate = 10f; // arvo 10-100, suurempi = nopeampi kuumentuminen
    public float damage = 1f; // arvo 1-10(?)

    private bool overheat = false;
    public GameObject shootPoint;

    public bool shooting;

    private void Start()
    {
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        // Stats setup
        fireTime = 100f / fireRate;
        loadCircle.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        shooting = false;
        overheat = false;
        if (loadCircle) loadCircle.fillAmount = 0;
        ohText.gameObject.SetActive(false);
        loadCircle.gameObject.SetActive(false);
    }

    void Update()
    {
        if(interval < 5)
            interval += Time.deltaTime;

        heat -= Time.deltaTime * 3;

        if (heat < 0) heat = 0;
        if (heat >= 10) 
        {
            heat = 10;
            overheat = true;
            ohText.gameObject.SetActive(true);
        }
        if (heat <= 5) 
        {
            overheat = false;
            ohText.gameObject.SetActive(false);
        } 

        loadCircle.fillAmount = heat / 10;

        if (loadCircle.fillAmount > 0.0f)
            loadCircle.gameObject.SetActive(true);
        else
            loadCircle.gameObject.SetActive(false);

        if (shooting) Fire();

    }


    public void Fire()
    {
        if (!overheat && gameObject.activeSelf == true)
        {
            if (interval >= fireTime)
            {

                flash.Play();
                shooting = true;
                interval = 0.0f;
                heat += Time.deltaTime * heatRate;
            }
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shootRange, rayTargets, QueryTriggerInteraction.Ignore) && interval == 0)
            {
                RaycastHit hit2;
                if (Physics.Raycast(shootPoint.transform.position, hit.point - shootPoint.transform.position, out hit2,shootRange, rayTargets, QueryTriggerInteraction.Ignore))
                {
                    if (hit2.collider.gameObject.GetComponent<TakeDamage>())
                    {
                        hit2.collider.gameObject.GetComponent<TakeDamage>().DoDamage(damage, playerParentObj);// Igorin lisäys se photon ID
                    }
                    else if (hit2.collider.gameObject.GetComponent<HitboxHandler>())
                    {
                        hit2.collider.gameObject.GetComponent<HitboxHandler>().parent.GetComponent<TakeDamage>().DoDamage(damage, playerParentObj);// Igorin lisäys se photon ID
                    }
                    else if (hit.transform.CompareTag("Boss1")) hit2.collider.gameObject.GetComponentInParent<Boss1>().TakeDamage(damage);
                    else if (hit.transform.CompareTag("Boss1Minion")) hit2.collider.gameObject.GetComponent<Boss1Minion>().TakeDamage(damage);
                    GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("ShootMinigun", RpcTarget.All, hit2.point, true);
                }

            } else if(interval == 0)
            {
                GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("ShootMinigun", RpcTarget.All, shootPoint.transform.position + cam.transform.forward * shootRange, false);
            }
        }
    }


    public void InstantiateEffects(Vector3 position, bool hit)
    {
        if (hit)
        {
            GameObject impactObj = Instantiate(impact, position, Quaternion.identity);
            Destroy(impactObj, 0.2f);
        }
        StopCoroutine("TracerBullet");
        StartCoroutine("TracerBullet",position);
        audioSource.clip = audios[UnityEngine.Random.Range(0, 2)];
        audioSource.Play();
    }

    IEnumerator TracerBullet(Vector3 position)
    {
        float timer = 0;
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, shootPoint.transform.position);
        lr.SetPosition(1, position);
        while(timer < 0.1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        lr.positionCount = 0;
        lr.enabled = false;
    }
}
