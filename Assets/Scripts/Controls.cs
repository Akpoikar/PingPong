using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    private PhotonView photonView;
    Rigidbody2D rigidbody;

    #region UNITY
    void Start()
    {
       
        Input.multiTouchEnabled = false;
        rigidbody = GetComponent<Rigidbody2D>();  
    }
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!photonView.AmOwner)
        {
            return;
        }
        if (this.photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        if ((Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(1))
        {
            Vector2 pos;
            pos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            pos = new Vector2(pos.x, transform.position.y);
            rigidbody.position = pos;
        }

    }

    #endregion

}
