using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public InputField RoomNameInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public GameObject PlayerListEntryPrefab;
        public GameObject StartButton;

        private Dictionary<int, GameObject> playerListEntries;

        const byte MaximumPlayers = 2;

        #region UNITY

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        IEnumerator ts()
        {
            if(!PlayerPrefs.HasKey("NickName"))
                yield break;

            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.PeerCreated);

            SetActivePanel(SelectionPanel.name);
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("NickName");
            PhotonNetwork.ConnectUsingSettings();
        }

        private void Start()
        {
            var vs = ts();
            StartCoroutine(vs);
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            CreateRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            CreateRoom();
        }

        public override void OnJoinedRoom()
        {
            SetActivePanel(InsideRoomPanel.name);
            
            if (!PhotonNetwork.IsMasterClient)
                StartButton.SetActive(false);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (var p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerAttributes>().Initialize(p.ActorNumber, p.NickName);

                playerListEntries.Add(p.ActorNumber, entry);
            }
          
            
        }

        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (newPlayer == PhotonNetwork.MasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                StartButton.SetActive(true);
            }
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerAttributes>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {

            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);        
            StartButton.SetActive(true);

            if (playerListEntries.ContainsKey(otherPlayer.ActorNumber))
            {
                Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
                playerListEntries.Remove(otherPlayer.ActorNumber);
            }
        }      

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }

        private void CreateRoom()
        {
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + PhotonNetwork.LocalPlayer.NickName : roomName;

            RoomOptions options = new RoomOptions { MaxPlayers = MaximumPlayers, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnCreateRoomButtonClicked()
        {
            PhotonNetwork.JoinRoom(RoomNameInputField.text);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

            if (!playerName.Equals(string.Empty))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                PlayerPrefs.SetString("NickName", playerName);
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnStartGameButtonClicked()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (PhotonNetwork.CurrentRoom.PlayerCount != MaximumPlayers)
            {
                Debug.Log("Not Enough Players");
                return;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("GameScene");
        }

        #endregion

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }
    }
}