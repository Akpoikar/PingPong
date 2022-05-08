using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text topPlayerScoreText;
    [SerializeField] Text botPlayerScoreText;

    PhotonView photonView;

    public int topPlayerScore = 0;
    public  int botPlayerScore = 0;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

    }

    public void TopPlayerWon()
    {
       
        photonView.RPC("TopPlayerWonRPC", RpcTarget.All);
    }   
    
    public void BotPlayerWon()
    {
        photonView.RPC("BotPlayerWonRPC", RpcTarget.All);
    }

    [PunRPC]
    void TopPlayerWonRPC()
    {
        topPlayerScore++;
        topPlayerScoreText.text = topPlayerScore.ToString();
    }

    [PunRPC]
    void BotPlayerWonRPC()
    {

        botPlayerScore++;
        botPlayerScoreText.text = botPlayerScore.ToString();
    }

}
