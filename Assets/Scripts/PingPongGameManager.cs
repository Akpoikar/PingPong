using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class PingPongGameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Objects")]
    public GameObject Ball;
    public GameObject PlayerPrefab;
    public GameObject PlayerParent;
    public GameObject ScoreObject;

    [Header("Positions")]
    public Transform EnemyPos;
    public Transform LocalPos;
    public Transform BallStartPos;

    [Header("UI Objects")]
    public GameObject MainUi;

    public static PingPongGameManager Instance { get; private set; }

    #region UNITY

    private void Start()
    {
        StartGame();
    }

    public void Awake()
    {
        Instance = this;
    }

    #endregion

    #region PUN CALLBACKS
    public override void OnDisconnected(DisconnectCause cause)
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("PingPongMenuScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.Disconnect();
    }

    #endregion

    private void StartGame()
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);

        Vector3 position;

        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
            position = LocalPos.position;
        else
            position = EnemyPos.position;
        

        PhotonNetwork.Instantiate("Player", position, Quaternion.Euler(0.0f, 0, 0.0f), 0);

        MainUi.GetComponent<MainUi>().SetNames();

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnBall());
        }
    }

    public void TopPlayerDeath()
    {
        Debug.Log("TopPlayer dead");
        ScoreObject.GetComponent<Score>().BotPlayerWon();
        ResetGame();
    }  
    
    public void BotPlayerDeath()
    {
        Debug.Log("BotPlayer dead");
        ScoreObject.GetComponent<Score>().TopPlayerWon();
        ResetGame();
    }

    private void ResetGame()
    {
        Ball.transform.position = BallStartPos.position;
        Ball.GetComponent<BallMovement>().ResetMovement();
    }

    #region Couroutines
    private IEnumerator SpawnBall()
    {
        yield return new WaitForSeconds(1);
        Vector3 force = -BallStartPos.position.normalized * 1000.0f;
        object[] instantiationData = { force };
        Ball = 
            PhotonNetwork.InstantiateRoomObject(
                "Ball",
                BallStartPos.position, 
                Quaternion.Euler(0.0f, 0, 0.0f),
                0,
                instantiationData);
    }
    #endregion
}
