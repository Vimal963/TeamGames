using System;
using UnityEngine;
using BestHTTP.SocketIO;
using SimpleJSON;
using UnityEngine.Networking;
using System.Collections;

namespace TeamGame
{
    /// <summary>
    /// Manage connection to server.
    /// here is is connection manager ,connect to releted server,
    /// </summary>
    [RequireComponent(typeof(Network_Emitter), typeof(Network_Listener))]
    public class Network_Gatekeeper : MonoBehaviour
    {
        public static Network_Gatekeeper Instance;

        [SerializeField] private bool enableLogs;
        private SocketManager socketManager;
        public static Socket mainsocket;
        public static Socket gamesocket;

        public static Action OnSuccessfullConnectionToServer;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        /// <summary>
        /// Establish connection to the server.
        /// </summary>
        public void ConnectToServer()
        {
            Debug.Log(" Trying to connect : " + StaticDataHandler.IsConnectedToSocket);
            if (!StaticDataHandler.IsConnectedToSocket)
            {
                if (enableLogs) Debug.Log("MAIN ~~~~~~ Attempting to connect to server.");
                if (StaticDataHandler.IsInternetAccessUnavailable())
                {
                    try
                    {
                        if (enableLogs) Debug.Log("No internet access.");
                    }
                    catch
                    {
                        if (enableLogs) Debug.Log("Failed to raise a toast. Returning control!");
                        return;
                    }
                }
                else
                {
                    socketManager = new SocketManager(new Uri(StaticDataHandler.URL_SERVER + StaticDataHandler.URL_SOCKET));
                    socketManager.Open();

                    if (ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.SingleBet)
                    {
                        if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Thousand)
                        {
                            Debug.Log("You are connecting with THOUSAND socket");
                            gamesocket = socketManager.GetSocket(StaticDataHandler.URL_THOUSAND);
                        }
                        else if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Blink)
                        {
                            Debug.Log("You are connecting with BLINK socket");
                            gamesocket = socketManager.GetSocket(StaticDataHandler.URL_BLINK);
                        }

                        gamesocket.On(SocketIOEventTypes.Connect, OnGameConnect);
                        gamesocket.On(SocketIOEventTypes.Disconnect, OnGameDisconnect);
                        gamesocket.On(SocketIOEventTypes.Error, OnGameError);

                    }
                    else
                    {
                        mainsocket = socketManager.GetSocket(StaticDataHandler.URL_MAIN_SOCKET);

                        mainsocket.On(SocketIOEventTypes.Connect, OnMainConnect);
                        mainsocket.On(SocketIOEventTypes.Disconnect, OnMainDisconnect);
                        mainsocket.On(SocketIOEventTypes.Error, OnMainError);
                    }
                }
            }
        }

        private void OnMainConnect(Socket socket, Packet packet, object[] args)
        {
            if (enableLogs) Debug.Log("MAIN ~~~~~ Connected to server.");
            StaticDataHandler.IsConnectedToSocket = true;

            JSONObject data = new JSONObject
            {
                ["playerId"] = StaticDataHandler.MyPlayerID
            };

            mainsocket.On("joinRoomData", GetGameTypeData);
            mainsocket.Emit("socketConnect", data.ToString());
        }

        private void OnMainDisconnect(Socket socket, Packet packet, object[] args)
        {
            if (enableLogs) Debug.Log("MAIN ~~~~~~ Disconnected from server.");
            StaticDataHandler.IsConnectedToSocket = false;
            AllAction_EventHandler.Instance.ClearDataOnDisconnetPlayer();
            ImportantDataMembers.Instance.ShowWarningPanel();
        }

        private void OnMainError(Socket socket, Packet packet, object[] args)
        {
            if (enableLogs)
            {
                Error error = args[0] as Error;
                Debug.Log("Erorr Code : " + error.Code.ToString());
                switch (error.Code)
                {
                    case SocketIOErrors.User:
                        Debug.Log("MAIN ~~~~~~ Exception in an event handler! : " + error.Message);
                        break;
                    case SocketIOErrors.Internal:
                        Debug.Log("MAIN ~~~~~~ Internal error! Message: " + error.Message);
                        break;
                    default:
                        Debug.Log("MAIN ~~~~~~ Server error! Message: " + error.Message);
                        break;
                }
            }
        }


        private void GetGameTypeData(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Poker GameType Received : " + args[0].ToString());
            JSONNode gameData = JSONNode.Parse(args[0].ToString());

            int gameIndex = gameData["gameType"];
            ChoosePokerGameType(gameIndex);
        }

        public void ChoosePokerGameType(int gameIndex)
        {
            if (gamesocket != null)
            {
                gamesocket.Disconnect();
                gamesocket.Off();
            }

            switch (gameIndex)
            {
                case 1:
                    Debug.Log("You are connecting with THOUSAND socket : " + (GAMES)gameIndex);
                    ImportantDataMembers.Instance.currentSelectedGame = (GAMES)gameIndex;
                    gamesocket = socketManager.GetSocket(StaticDataHandler.URL_THOUSAND);
                    break;
                case 2:
                    Debug.Log("You are connecting with BLINK socket : " + (GAMES)gameIndex);
                    ImportantDataMembers.Instance.currentSelectedGame = (GAMES)gameIndex;
                    gamesocket = socketManager.GetSocket(StaticDataHandler.URL_BLINK);
                    break;
                default:
                    break;
            }

            if (gamesocket != null)
            {
                Debug.Log("GAme Socket : " + gamesocket.ToString());
            }

            gamesocket.On(SocketIOEventTypes.Connect, OnGameConnect);
            gamesocket.On(SocketIOEventTypes.Disconnect, OnGameDisconnect);
            gamesocket.On(SocketIOEventTypes.Error, OnGameError);
        }

        private void OnGameConnect(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("GAME ~~~~~ Connected to server.");

            StaticDataHandler.IsConnectedToSocket = true;
            OnSuccessfullConnectionToServer?.Invoke();
            Network_Emitter.Instance.Emit_CreateAndJoinRoom();
        }

        private void OnGameDisconnect(Socket socket, Packet packet, object[] args)
        {
            if (enableLogs) Debug.Log("GAME ~~~~~~ Disconnected from server.");
            StaticDataHandler.IsConnectedToSocket = false;
            AllAction_EventHandler.Instance.ClearDataOnDisconnetPlayer();
        }

        private void OnGameError(Socket socket, Packet packet, object[] args)
        {
            if (enableLogs)
            {
                Error error = args[0] as Error;
                Debug.Log("Erorr Code : " + error.Code.ToString());
                switch (error.Code)
                {
                    case SocketIOErrors.User:
                        Debug.Log("GAME ~~~~~~ Exception in an event handler! : " + error.Message);
                        break;
                    case SocketIOErrors.Internal:
                        Debug.Log("GAME ~~~~~~ Internal error! Message: " + error.Message);
                        break;
                    default:
                        Debug.Log("GAME ~~~~~~ Server error! Message: " + error.Message);
                        break;
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Debug.Log("Application Pause");
                DisConnectFromServer();
            }
        }

        private void OnApplicationQuit()
        {
            //if (Application.isEditor)
            {
                Debug.Log("Application Quit");
                DisConnectFromServer();
            }
        }

        private void DisConnectFromServer()
        {
            if (mainsocket != null)
            {
                mainsocket.Disconnect();
                mainsocket.Off();
            }
            if (gamesocket != null)
            {
                gamesocket.Disconnect();
                gamesocket.Off();
            }
            ImportantDataMembers.Instance.ShowWarningPanel();
        }

    }
}