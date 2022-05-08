using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    readonly string TopPlayerDeathArea = "DeathTop";
    readonly string BotPlayerDeathArea = "DeathBot";

    private PhotonView photonView;

    private Rigidbody2D rigidbody;

    Vector2 speed;
    
    public float speedMultiplayer = 1;

    [SerializeField] float speedMin;
    [SerializeField] float speedMax;

    #region UNITY
    public void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody2D>();

        ResetMovement();
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Move();
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        CheckRikochet(coll);

        CheckDeathRegion(coll);
    }
    #endregion

   
    #region RPC

    [PunRPC]
    void Moved(Vector3 pos)
    {
        transform.position = pos;
    }
    #endregion

    public void ResetMovement()
    {
        speed.x = Random.Range(speedMin, speedMax);
        speed.y = Random.Range(speedMin, speedMax);
    }

    private void CheckRikochet(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            speed.y = -speed.y;
        }
        else if (coll.gameObject.CompareTag("Bound"))
        {
            speed.x = -speed.x;
        }
    }

    private void CheckDeathRegion(Collision2D coll)
    {
        if (coll.gameObject.CompareTag(TopPlayerDeathArea)) 
        {
            PingPongGameManager.Instance.TopPlayerDeath();
        }
        else if (coll.gameObject.CompareTag(BotPlayerDeathArea)) 
        {
            PingPongGameManager.Instance.BotPlayerDeath();
        }

    }

    private void Move()
    {
        Vector3 newPos =
             transform.position + speedMultiplayer * Time.deltaTime * new Vector3(speed.x, speed.y);

        photonView.RPC("Moved", RpcTarget.All, newPos);
    }


}
