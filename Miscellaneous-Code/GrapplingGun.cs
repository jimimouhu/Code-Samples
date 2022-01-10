using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrapplingGun : MonoBehaviourPunCallbacks
{
    RaycastHit hit;
    public LayerMask mask;
    public float shootDistance = 100; // 50-100?
    public float jumpForce = 10; // 5-10?
    public bool flying;
    public Vector3 loc;
    public Transform shootPoint, player;
    public bool shooting;
    public LineRenderer lr;

    Vector3 endingPosition;

    private void Start()
    {
        player.GetComponent<PlayerMovementScript>();
        lr.enabled = false;
    }


    public void Shoot()
    {
        if (!shooting)
        {
            if (Physics.Raycast(shootPoint.transform.position + Camera.main.transform.TransformDirection(Vector3.up) * 2, Camera.main.transform.forward, out hit, shootDistance, mask))
            {
                loc = hit.point;
                player.GetComponent<PlayerMovementScript>().Grapping(loc);
                shooting = true;
                GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("InstantiateRope", RpcTarget.All, shootPoint.position, loc);
            }
        } 
        else
        {
            //player.GetComponent<PlayerMovementScript>().DrawRope(loc, jumpForce);
            //GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("InstantiateRope", RpcTarget.All, shootPoint.position, loc);
            player.GetComponent<PlayerMovementScript>().JointJump(jumpForce);
        }

    }
    private void Update()
    {
        if(shooting)
            DrawRope(shootPoint.position, endingPosition);

    }


    public void StopShoot()
    {
        shooting = false;
        player.GetComponent<PlayerMovementScript>().StopGrapping();
        lr.positionCount = 0;
        lr.enabled = false;
        GameStatus.global.myCharacter.GetComponent<RPCHandler>().photonView.RPC("DestroyRope", RpcTarget.All);
    }


    public void DrawRope(Vector3 startPos, Vector3 endPos)
    {
        endingPosition = endPos;
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }
    public void DestroyRope()
    {
        shooting = false;
        lr.positionCount = 0;
        lr.enabled = false;
    }




}
