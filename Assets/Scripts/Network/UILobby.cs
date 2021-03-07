using System;
using UnityEngine;
using UnityEngine.UI;

namespace Network {
    public class UILobby : MonoBehaviour {

        public static UILobby instance;
        
        
        [Header("Host Join")]
        [SerializeField] private InputField joinMatchInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button hostButton;
        [SerializeField] private Canvas lobbyCanvas;

        [Header("Lobby")]
        [SerializeField] private Transform uiPlayerParrent;
        [SerializeField] private GameObject uiPlayerPrefab;
        [SerializeField] private Text matchIdText;
        [SerializeField] private GameObject beginGameButton;

        
        void Start() {
            instance = this;
        }

        public void Host() {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            
            Player.localPlayer.HostGame();
        }

        public void HostSuccess(bool success, string matchId) {
            if (success) {
                lobbyCanvas.enabled = true;
                SpawnPlayerUIPrefab(Player.localPlayer);
                matchIdText.text = matchId;
                beginGameButton.SetActive(true);
            }
            else {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void Join() {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            
            Player.localPlayer.JoinGame(joinMatchInput.text.ToUpper());
        }
        
        public void JoinSuccess(bool success, string matchId) {
            if (success) {
                lobbyCanvas.enabled = true;
                SpawnPlayerUIPrefab(Player.localPlayer);
                matchIdText.text = matchId;
            }
            else {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void SpawnPlayerUIPrefab(Player player) {
            var newUIPlayer = Instantiate(uiPlayerPrefab, uiPlayerParrent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
            newUIPlayer.transform.SetSiblingIndex(player.playerIndex-1);
        }

        public void BeginGame() {
            Player.localPlayer.BeginGame();
        }
    }
}