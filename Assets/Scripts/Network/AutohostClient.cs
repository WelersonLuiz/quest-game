using Mirror;
using UnityEngine;

namespace Network {
    public class AutoHostClient : MonoBehaviour {
        [SerializeField] private NetworkManager networkManager;

        void Start() {
            if (Application.isBatchMode) {
                Debug.Log("Server Starting");
            }
        }
        
        public void StartClient(){
            Debug.Log("Connecting Client");
            networkManager.StartClient();
        }
        
        public void JoinLocal() {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }
    }
}