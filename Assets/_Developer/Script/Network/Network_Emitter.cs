using UnityEngine;
using BestHTTP.SocketIO;
using SimpleJSON;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamGame
{
    /// <summary>
    /// Send Data to server.
    /// here we will do code for send data to server
    /// </summary>
    [RequireComponent(typeof(Network_Gatekeeper), typeof(Network_Listener))]
    public class Network_Emitter : MonoBehaviour
    {

        public static Network_Emitter Instance;

        [SerializeField]
        private bool enableLogs;
        private static Socket socket;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {

        }

        private void OnEnable()
        {
            Network_Gatekeeper.OnSuccessfullConnectionToServer += LinkToGateKeeperSocket;
        }

        private void OnDisable()
        {
            Network_Gatekeeper.OnSuccessfullConnectionToServer -= LinkToGateKeeperSocket;
        }

        /// <summary>
        /// Linking to socket of Network_Gatekeeper.
        /// </summary>
        private void LinkToGateKeeperSocket()
        {
            if (Network_Gatekeeper.gamesocket != null)
            {
                if (enableLogs) Debug.Log("DC Emitter ~~~~~~ Establishing a link to Gate Keeper socket.");
                socket = Network_Gatekeeper.gamesocket;
            }
        }

        public void Emit_CreateAndJoinRoom()
        {
            string betType;
            if (ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.SingleBet) betType = "single";
            else if (ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.PokerRule) betType = "poker";
            else betType = "poker";

            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID,
                ["gameType"] = betType,
                ["stack"] = StaticDataHandler.GetTwoDecimalValue(ImportantDataMembers.Instance.currentSelectedStack),
                ["team"] = ImportantDataMembers.Instance.currentPickedTeam.ToString()
            };
            Debug.Log("EMIT For Create room ====== JSON is : " + data.ToString());
            socket.Emit(StaticDataHandler.EMIT_JOIN_OR_CREATE_ROOM, data.ToString());
        }

        public void Emit_OnPlayerLeft()
        {
            AllAction_EventHandler.Instance.ClearDataOnDisconnetPlayer();

            if (socket != null)
            {
                JSONObject data = new JSONObject
                {
                    ["playerId"] = StaticDataHandler.MyPlayerID
                };
                Debug.Log("Game Socket Disconnected");
                socket.Emit(StaticDataHandler.EMIT_PLAYER_LEFT, data.ToString());
                socket.Disconnect();
                socket.Off();
                //Network_Gatekeeper.Instance.DisConnectFromServer();
            }
            if (Network_Gatekeeper.mainsocket != null)
            {
                JSONObject data = new JSONObject
                {
                    ["playerId"] = StaticDataHandler.MyPlayerID
                };
                Debug.Log("Main Socket Disconnected");
                Network_Gatekeeper.mainsocket.Emit(StaticDataHandler.EMIT_PLAYER_LEFT, data.ToString());
                Network_Gatekeeper.mainsocket.Disconnect();
                Network_Gatekeeper.mainsocket.Off();
                //Network_Gatekeeper.Instance.DisConnectFromServer();
            }

            StaticDataHandler.IsConnectedToSocket = false;
        }

        public void Emit_WinnerPlayerData()
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID
            };

            print("EMIT DATA TO GET WIN DATA ==== " + data.ToString());
            BetScreenUI.Instance.totalStacks.transform.GetComponentInChildren<Text>().text = "$" + StaticDataHandler.totalStacksForRound;
            socket.Emit(StaticDataHandler.EMIT_WINNER_PLAYER_DATA, data.ToString());
        }

        public void Emit_SitOutForNextRound()
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID,
                ["sitOutNextRound"] = StaticDataHandler.sitOutForNextRound
            };

            Debug.Log("EMIT for Player SIT OUT Bet Json Is : " + data.ToString());
            socket.Emit(StaticDataHandler.EMIT_PLAYER_PLAYER_BET, data.ToString());
        }

        public void Emit_OnPlayerBet(float stack, string action)
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID,
                ["stacks"] = stack,
                ["playerOption"] = action
            };

            Debug.Log("EMIT for Player Bet Json Is : " + data.ToString());
            socket.Emit(StaticDataHandler.EMIT_PLAYER_PLAYER_BET, data.ToString());
        }

        public void ChangeGameAfterWinner()
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID,
                ["connectWithNextRound"] = true
            };

            socket.Emit(StaticDataHandler.EMIT_CHANGE_POKERGAME, data.ToString());
        }

        public void Emit_StartCountDown()
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID
            };

            socket.Emit(StaticDataHandler.EMIT_START_GAME_COUNT_DOWN, data.ToString());
        }

        public void Emit_ThousandScore(string score)
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID,
                ["score"] = score
            };

            Debug.Log("Stop The Clock score ======> : " + data.ToString());
            socket.Emit(StaticDataHandler.EMIT_ThousandScore, data.ToString());
        }

        public void Emit_BlinkScore(string score)
        {
            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID,
                ["score"] = score
            };

            Debug.Log("Blink score ======> : " + data.ToString());
            socket.Emit(StaticDataHandler.EMIT_BlinkScore, data.ToString());
        }

    }
}