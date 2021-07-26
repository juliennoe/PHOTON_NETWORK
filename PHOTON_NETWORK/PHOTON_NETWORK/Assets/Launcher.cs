using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;
        /// Le numéro de version de ce client. Les utilisateurs sont séparés les uns des autres par gameVersion (qui vous permet d'apporter des modifications importantes).
        string gameVersion = "1";

        void Awake()
        {
            // Synchronisation du MASTER avec les autres room 
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // Check si je suis connecté ou non, si je suis connecté, je suis en attente des autres, sinon je me connecte
            if (PhotonNetwork.IsConnected)
            {
                // Check si je peux me connecter à une ROOM si non, je suis prevenu
                PhotonNetwork.JoinRandomRoom();
                Debug.Log("CONNECTED");
            }
            else
            {
                // #Critical, nous devons avant tout nous connecter à Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            // #Critical: Premiere étape, rejoindre une ROOM existante, sinon orienté vers OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: Check si je peux rejoindre une ROOM si non, je créer une nouvelle ROOM
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        }

        #endregion
    }
}