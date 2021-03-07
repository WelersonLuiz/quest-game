using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mirror;
using UnityEngine;
using Random = System.Random;

namespace Network {

    [Serializable]
    public class Match {
        public string matchId;
        public SyncListGameObject players = new SyncListGameObject();

        public Match(string matchId, GameObject player) {
            this.matchId = matchId;
            players.Add(player);
        }
        
        public Match(){}
    }

    [Serializable] public class SyncListGameObject : SyncList<GameObject> { }
    [Serializable] public class SyncListMatch : SyncList<Match> { }

    public class MatchMaker : NetworkBehaviour {

        public static MatchMaker instance;
        public SyncListMatch matches = new SyncListMatch();
        public SyncList<string> matchIds = new SyncList<string>();
        [SerializeField] GameObject turnManagerPrefab;
        
        void Start() {
            instance = this;
        }

        public bool HostGame(string matchId, GameObject player, out int playerIndex) {
            playerIndex = -1;
            
            if (!matchIds.Contains(matchId)) {
                matchIds.Add(matchId);
                matches.Add(new Match(matchId, player));
                Debug.Log("Match generated");
                playerIndex = 1;
                return true;
            }

            Debug.Log("Match ID already exists");
            return false;
        }
        
        public bool JoinGame(string matchId, GameObject player, out int playerIndex) {
            playerIndex = -1;

            if (matchIds.Contains(matchId)) {

                foreach (var match in matches.Where(match => match.matchId == matchId)) {
                    match.players.Add(player);
                    playerIndex = match.players.Count;
                    break;
                }
                
                Debug.Log("Match joined");
                return true;
            }

            Debug.Log("Match ID does not exists");
            return false;
        }

        public void BeginGame(string matchId) {
            var newTurnManager = Instantiate(turnManagerPrefab);
            NetworkServer.Spawn(newTurnManager);
            newTurnManager.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
            var turnManager = newTurnManager.GetComponent<TurnManager>();

            foreach (var match in matches.Where(match => match.matchId == matchId)) {
                foreach (var player in match.players.Select(player => player.GetComponent<Player>())) {
                    turnManager.AddPlayer(player);
                    player.StartGame();
                }
            }
        }
        
        public static string GetRandomMatchId() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[5];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var matchId = new string(stringChars);
            
            Debug.Log($"Random Match Id: {matchId}");
            return matchId;
        }
    }

    public static class MatchExtensions {
        public static Guid ToGuid(this string id) {
            var provider = new MD5CryptoServiceProvider();
            var inputBytes = Encoding.Default.GetBytes(id);
            var hashBytes = provider.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }
}