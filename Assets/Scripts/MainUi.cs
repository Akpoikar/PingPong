using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MainUi : MonoBehaviour
{
    public GameObject SpriteUI;
    public Sprite[] Sprites;

    PhotonView photonView;

    [SerializeField] Text topPlayerName;
    [SerializeField] Text botPlayerName;

    #region UNITY
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            Sprite sprite = Sprites[Random.Range(0, Sprites.Length)];

            photonView.RPC("SetAll", RpcTarget.All, sprite.name);
        }
    }
    #endregion
    public void SetNames()
    {
        botPlayerName.text = PhotonNetwork.PlayerList[0].NickName;

        topPlayerName.text = PhotonNetwork.PlayerList[1].NickName;   
    }

    [PunRPC]
    void SetAll(string tempString)
    {
        Debug.Log(tempString);
        foreach (var item in Sprites)
        {
            if(item.name == tempString)
                SpriteUI.GetComponent<Image>().sprite = item;
        }
        
    }
}
