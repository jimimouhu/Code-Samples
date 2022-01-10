using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayGun : MonoBehaviourPunCallbacks
{
    public LayerMask rayTargets;
    Camera cam;
    public GameObject playerParentObj;
    public GameObject loadEffect;
    RaycastHit hit;
    public GameObject rayEffect;
    public Image loadCircle;
    public bool loading;
    public bool shooting;
    public bool loadingEffectOn;
    public Transform shootPoint;
    public float damage;
    public float shootRange;
    public float loadTime; // min 1.5sec, max 3sec
    float loader;
    public LineRenderer lr;

    public void Start()
    {
        cam = Camera.main;
    }

    private void OnDisable()
    {
        loading = false;
        if (loadCircle)
        {
            loadCircle.fillAmount = 0;
            loadCircle.gameObject.SetActive(false);
            loader = 0;
        }
    }

    private void Update()
    {
        if (loading)
        {
            Load();

            if (loadingEffectOn)
            {
                GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("LoadRaygun", RpcTarget.All, shootPoint.transform.position);
                loadingEffectOn = false;
            }

        }
        if (shooting) Shoot();

    }

    void Load()
    {
        loadCircle.gameObject.SetActive(true);
        loader += Time.deltaTime;
        loadCircle.fillAmount = loader / loadTime;

        if (loader >= loadTime) loader = loadTime;
    }
    void Shoot()
    {
        //if (loader > loadTime / 100 * 15) // 15% of the total loadtime
        //{
        //    if (Physics.Raycast(shootPoint.transform.position, Camera.main.transform.forward, out hit, shootRange))
        //    {
        //        if (hit.transform.GetComponent<TakeDamage>())
        //            hit.collider.gameObject.GetComponent<TakeDamage>().DoDamage(damage * ((loader * 100 / loadTime) / 100), playerParentObj);// Igorin lisäys se photon ID

        //        GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("ShootRaygun", RpcTarget.All, hit.point);
        //    }
        //    else GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("ShootRaygun", RpcTarget.All, shootPoint.transform.position + Camera.main.transform.forward * shootRange);
        //}


        if (loader > loadTime / 100 * 15) // 15% of the total loadtime
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shootRange, rayTargets, QueryTriggerInteraction.Ignore))
            {
                RaycastHit hit2;
                if (Physics.Raycast(shootPoint.transform.position, hit.point - shootPoint.transform.position, out hit2, shootRange, rayTargets, QueryTriggerInteraction.Ignore))
                {
                    if (hit2.collider.gameObject.GetComponent<TakeDamage>())
                    {
                        hit2.collider.gameObject.GetComponent<TakeDamage>().DoDamage(damage * ((loader * 100 / loadTime) / 100), playerParentObj);// Igorin lisäys se photon ID
                    }
                    else if (hit2.collider.gameObject.GetComponent<HitboxHandler>())
                    {
                        hit2.collider.gameObject.GetComponent<HitboxHandler>().parent.GetComponent<TakeDamage>().DoDamage(damage, playerParentObj);// Igorin lisäys se photon ID
                    }
                    else if (hit.transform.CompareTag("Boss1")) hit2.collider.gameObject.GetComponentInParent<Boss1>().TakeDamage(damage * ((loader * 100 / loadTime) / 100));
                    else if (hit.transform.CompareTag("Boss1Minion")) hit2.collider.gameObject.GetComponent<Boss1Minion>().TakeDamage(damage * ((loader * 100 / loadTime) / 100));
                }
                GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("ShootRaygun", RpcTarget.All, hit.point);
            }
            else GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("ShootRaygun", RpcTarget.All, shootPoint.transform.position + cam.transform.forward * shootRange);

        }
        ResetLoading();
    }
    void ResetLoading()
    {
        shooting = false;
        loader = 0;
        loadCircle.fillAmount = 0;
        loadCircle.gameObject.SetActive(false);
    }
    public void InstantiateEffects(Vector3 position)
    {
        StartCoroutine(TracerBullet(position));
        GameObject impactObj = Instantiate(rayEffect, shootPoint.position, Quaternion.identity);
        impactObj.transform.LookAt(position);
        impactObj.GetComponent<ParticleSystem>().Play();
        ParticleSystem.MainModule particle = impactObj.GetComponent<ParticleSystem>().main;
        Destroy(impactObj, particle.duration);
    }

    public void PlayLoadEffect(Vector3 position)
    {
        GameObject impactObj = Instantiate(loadEffect, shootPoint.position, Quaternion.identity);
        impactObj.GetComponent<ParticleSystem>().Play();
        ParticleSystem.MainModule particle = impactObj.GetComponent<ParticleSystem>().main;
        StartCoroutine(moveEffect());

        IEnumerator moveEffect()
        {
            while (loading)
            {
                impactObj.transform.position = shootPoint.position;
                if (loader < loadTime) impactObj.transform.localScale *= (1f + Time.deltaTime / 2);
                yield return null;
            }
            Destroy(impactObj);
        }
    }



    IEnumerator TracerBullet(Vector3 position)
    {
        if (!lr.enabled)
        {
            lr.enabled = true;
            float timer = 0;
            lr.positionCount = 2;
            lr.SetPosition(0, shootPoint.transform.position);
            lr.startWidth = 1;
            lr.endWidth = 0.5f;
            // grow the Ray
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                lr.SetPosition(1, Vector3.Lerp(shootPoint.position, position, timer / 0.3f));
                yield return null;
            }
            StartCoroutine(ReduceSizeOfRay());
        }
    }
    IEnumerator ReduceSizeOfRay()
    {
        float startSize = lr.startWidth;
        float endSize = lr.endWidth;
        float timer = 0;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            lr.startWidth = Mathf.Lerp(startSize, 0, timer / 0.25f);
            lr.endWidth = Mathf.Lerp(endSize, 0, timer / 0.25f);
            yield return null;
        }
        lr.positionCount = 0;
        lr.enabled = false;
    }
}


