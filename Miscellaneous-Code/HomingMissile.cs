using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomingMissile : MonoBehaviourPunCallbacks
{
    public Rigidbody missile;
    public ParticleSystem flash;
    public LayerMask groundMask;
    public GameObject cursor;
    GameObject landingPointCursor;
    public GameObject bombPrefab;
    public Transform shootPoint;
    public Image loadCircle;
    bool startedAiming;
    bool loading;
    bool cursorActivated;
    public bool active;

    // Stats
    public float loadTime; // esim 1-3sec
    public float blastRadius = 20.0f; // 10-20
    public float damage = 5f; // 
    public int bombCapacity = 5; // esim 1-5
    int bombsRemaining;


    private void Start()
    {
        bombsRemaining = bombCapacity;
        loadTime = bombCapacity;
        loadCircle.gameObject.SetActive(false);
        landingPointCursor = Instantiate(cursor, transform.position, Quaternion.Euler(90, 0, 0));
        landingPointCursor.SetActive(false);
    }

    private void OnDisable()
    {
        loading = false;
        if (loadCircle)
        {
            loadCircle.fillAmount = 0;
            loadCircle.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (bombsRemaining == 0 && !loading && !active)
        {
            StartCoroutine(LoadCannon(loadTime));
        }
        if (bombsRemaining > 0 && !loading && active)
        {
            AimGrenade();
        }



    }

    public void AimGrenade()
    {
        landingPointCursor.SetActive(true);
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position + Camera.main.transform.TransformDirection(Vector3.up) * 2, Camera.main.transform.forward, out hit, 800, groundMask))
        {
            landingPointCursor.transform.position = hit.point + Vector3.up * 0.1f;
            landingPointCursor.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
    public void ShootGrenade()
    {
        if (bombsRemaining > 0 && !loading && active) 
        {
            Vector3 spawnPoint = landingPointCursor.transform.position;
            spawnPoint.y = spawnPoint.y + 4;
            Vector3 landing = landingPointCursor.transform.position;
            Vector3 Vo = CalculateVelocity(landing, shootPoint.position, 1f);

            float[] shootPointPos = { shootPoint.transform.position.x, shootPoint.transform.position.y, shootPoint.transform.position.z };
            float[] vel = { Vo.x, Vo.y, Vo.z };

            GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("InstantiateGrenade", RpcTarget.All, shootPointPos, vel);
            landingPointCursor.SetActive(false);
            bombsRemaining--;
        }
    }

    IEnumerator LoadCannon(float duration)
    {
        loading = true;
        loadCircle.gameObject.SetActive(true);
        loadCircle.fillClockwise = false;
        loadCircle.fillAmount = 0;
        float time = 0;
        while (time < duration)
        {
            loadCircle.fillAmount += Time.deltaTime / duration;
            time += Time.deltaTime;
            yield return null;
        }
        bombsRemaining = bombCapacity;
        loading = false;
        loadCircle.fillClockwise = true;
        loadCircle.fillAmount = 0;
        loadCircle.gameObject.SetActive(false);

    }

    Vector3 CalculateVelocity(Vector3 landing, Vector3 origin, float time)
    {
        Vector3 distance = landing - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;
        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time; // 2f
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }
    public void LaunchMissile(float[] startPos, float[] velocity, GameObject player)// Igorin lisäys se photon ID
    {
        GameObject obj = Instantiate(bombPrefab, new Vector3(startPos[0], startPos[1], startPos[2]), Quaternion.identity);
        obj.GetComponent<Explosive>().player = player;// Igorin lisäys se photon ID
        obj.GetComponent<Rigidbody>().velocity = new Vector3(velocity[0], velocity[1], velocity[2]);
        flash.Play();
    }


}
