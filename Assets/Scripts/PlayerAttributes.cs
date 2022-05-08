using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttributes : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;
    
    private int ownerId;
    string nickName;
    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        nickName = playerName;
        PlayerNameText.text = nickName;
    }

    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            
        }
        else
        {
            ExitGames.Client.Photon.Hashtable initialProps = new
                ExitGames.Client.Photon.Hashtable() 
            { { "NickName", nickName } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);
        }
    }
}
