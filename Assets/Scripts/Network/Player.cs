using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network {
    public class Player : NetworkBehaviour {
        
        public static Player localPlayer;
        private NetworkMatchChecker _matchChecker;
        [SyncVar] public string _matchId;
        [SyncVar] public int playerIndex;

        void Start() {
            _matchChecker = GetComponent<NetworkMatchChecker>();
            
            if (isLocalPlayer) {
                localPlayer = this;
            } else {
                UILobby.instance.SpawnPlayerUIPrefab(this);
            }
        }

        
        /*
         * HOST GAME
         */
         
        public void HostGame() {
            var matchId = MatchMaker.GetRandomMatchId();
            CmdHostGame(matchId);
        }

        [Command]
        void CmdHostGame(string matchId) {
            _matchId = matchId;

            if (MatchMaker.instance.HostGame(matchId, gameObject, out playerIndex)) {
                Debug.Log("Game Hosted sucessfully");
                _matchChecker.matchId = matchId.ToGuid();
                TargetHostGame(true, matchId, playerIndex);
            } else {
                Debug.Log("Game Host failed");
                TargetHostGame(false, matchId, playerIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string matchId, int playerIdx) {
            playerIndex = playerIdx;
            _matchId = matchId;
            Debug.Log($"MatchID: {matchId} -- {_matchId}");
            UILobby.instance.HostSuccess(success, matchId);
        }
        
        /*
         * JOIN GAME
         */
        
        public void JoinGame(string matchId) {
            CmdJoinGame(matchId);
        }

        [Command]
        void CmdJoinGame(string matchId) {
            _matchId = matchId;

            if (MatchMaker.instance.JoinGame(matchId, gameObject, out playerIndex)) {
                Debug.Log("Game Joined sucessfully");
                _matchChecker.matchId = matchId.ToGuid();
                TargetJoinGame(true, matchId, playerIndex);
            } else {
                Debug.Log("Game Join failed");
                TargetJoinGame(false, matchId, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string matchId, int playerIdx) {
            playerIndex = playerIdx;
            _matchId = matchId;
            Debug.Log($"MatchID: {matchId} -- {_matchId}");
            UILobby.instance.JoinSuccess(success, matchId);
        }

        /*
         * BEGIN GAME
         */
        
        public void BeginGame() {
            CmdBeginGame();
        }

        [Command]
        void CmdBeginGame() {
            MatchMaker.instance.BeginGame(_matchId);
            Debug.Log("Game begining"); 
        }

        public void StartGame() {
            TargetBeginGame();
        }

        [TargetRpc]
        void TargetBeginGame() {
            Debug.Log($"MatchID: {_matchId} | Begining");
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }
    }
}