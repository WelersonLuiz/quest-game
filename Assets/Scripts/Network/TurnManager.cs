using System.Collections.Generic;
using Mirror;

namespace Network {
    public class TurnManager : NetworkBehaviour {
        
        private List<Player> players = new List<Player>();
        
        public void AddPlayer(Player player) {
            players.Add(player);
        }
        
    }
}
