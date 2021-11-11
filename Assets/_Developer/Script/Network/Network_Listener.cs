using System;
using UnityEngine;
using BestHTTP.SocketIO;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Assets.SimpleLocalization;

namespace TeamGame
{
    /// <summary>
    /// Receive Data from server. (With Examples.)
    /// here we will code for listen or receive data from server.
    /// make some actions and methods event to read and access dat from server.
    /// </summary>
    [RequireComponent(typeof(Network_Gatekeeper), typeof(Network_Emitter))]
    public class Network_Listener : MonoBehaviour
    {
        public static Network_Listener Instance;

        [SerializeField]
        private bool enableLogs;
        private static Socket socket;

        public Action<object[]>
            onTablePlayerList_ACN,
            onPlayerLeft_ACN,
            onPlayerWaitingList_ACN,
            Aciotn_dealerID,
            Action_SitOut,

            onReceiveThousandCountDown_ACN,
            onReceivedThousandTimer_ACN,
            onReceivedThousandPlayerScore_ACN,

            onReceinveBlinkCountDown_ACN,
            onReceivedBlinkTimer_ACN,
            onReceivedBlinkPLayerScore_ACN

            ;

        public Action<object[]> playerActionReflectOtherPlayer;
        public Action<string, string, string> onReceivedBettinTurnTimer;

        private void Awake()
        {
            if (Instance == null) Instance = this;
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
                if (enableLogs) Debug.Log("DC Listener ~~~~~~ Establishing a link to Gate Keeper socket.");
                socket = Network_Gatekeeper.gamesocket;

                socket.On(StaticDataHandler.LISTEN_JOIN_OR_CREATE_ROOM, OnGetPlayersList);
                socket.On(StaticDataHandler.LISTEN_PLAYER_LEFT, OnPlayerLeft);
                socket.On(StaticDataHandler.LISTEN_PLAYER_WAITING_LIST, OnPlayerWaitingList);
                socket.On(StaticDataHandler.LISTEN_TOTALSTACKS, OnTotalStacks);
                socket.On(StaticDataHandler.LISTEN_SEATOUT, OnSeatOut);
                socket.On(StaticDataHandler.LISTEN_DEALERID, OnDealerID);
                socket.On(StaticDataHandler.LISTEN_PLAYER_READY_FOR_BET, OnReadyToBet);
                socket.On(StaticDataHandler.LISTEN_PLAYER_TURN, OnReceivePlayerTurn);
                socket.On(StaticDataHandler.LISTEN_PLAYER_TURN_TIMER, OnReceivePlayerBetTurnTimer);
                socket.On(StaticDataHandler.LISTEN_PLAYER_ACTION, OnReceivedPlayerAction);
                socket.On(StaticDataHandler.LISTEN_ALL_PLAYER_BET_DONE, OnAllPLayerbetDone);
                socket.On(StaticDataHandler.LISTEN_GAME_START_TIMER, OnGameStartTimer);
                socket.On(StaticDataHandler.LISTEN_PLAYER_WAITING_FOR_NEXTROUND, OnWaitingForNextRound);
                socket.On(StaticDataHandler.LISTEN_PLAYER_WAITING_FOR_OTHERPLAYER, OnWaitingForOtherPlayer);
                socket.On(StaticDataHandler.LISTEN_PLAYER_READY_FOR_GAME, OnGameReadyToStart);
                socket.On(StaticDataHandler.LISTEN_WINNER_PLAYER_DATA, OnWinnerPlayerData);
                socket.On(StaticDataHandler.LISTEN_SITOUTACTION, OnSitOut);


                socket.On(StaticDataHandler.LISTEN_THOUSAND_COUNTDOWN, OnReceiveThousandCountDown);
                socket.On(StaticDataHandler.LISTEN_THOUSAND_REFLECT_TIMER, OnReceiveThousandTimer);
                socket.On(StaticDataHandler.LISTEN_THOUSAND_MY_SCORE, OnReceiveThousandMyScore);


                socket.On(StaticDataHandler.LISTEN_BLINK_COUNTDOWN, OnReceiveBlinkCountDown);
                socket.On(StaticDataHandler.LISTEN_BLINK_REFLECT_TIMER, OnReceiveBlinkTimer);
                socket.On(StaticDataHandler.LISTEN_BLINK_MY_SCORE, OnReceiveBlinkMyScore);
            }

            if (Network_Gatekeeper.mainsocket != null)
            {
                Network_Gatekeeper.mainsocket.On(StaticDataHandler.LISTEN_CHANGE_POKERGAME_TYPE, ChangePokerGameAfterWin);
            }
        }

        private void OnGetPlayersList(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("==========>  Player List Received : " + args[0].ToString());
            if (onTablePlayerList_ACN == null) print("Can't set player in table because action is null");
            else onTablePlayerList_ACN?.Invoke(args);
        }

        private void OnPlayerLeft(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("LISTEN PlayerLeft Data Received : " + args[0].ToString());
            if (onPlayerLeft_ACN == null) print("Can't left player list because action is null");
            else onPlayerLeft_ACN?.Invoke(args);
        }

        private void OnPlayerWaitingList(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("LISTEN PlayerLeft Data Received : " + args[0].ToString());
            if (onPlayerWaitingList_ACN == null) print("Can't get player waiting list because action is null");
            else onPlayerWaitingList_ACN?.Invoke(args);
        }

        private void OnTotalStacks(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("total stacks => " + args[0].ToString());
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());
            string totalStacks = jsonString["totalStacks"];
            totalStacks = StaticDataHandler.GetTwoDecimalString(totalStacks);

            StaticDataHandler.totalStacksForRound = totalStacks;
            BetScreenUI.Instance.totalStacks.transform.GetComponentInChildren<Text>().text = "$" + totalStacks;
        }

        private bool seatOut = false;
        private void OnSeatOut(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("seatOut => " + args[0].ToString());
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());
            seatOut = jsonString["seatOut"];
        }


        private void OnDealerID(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.PokerRule)
            {
                Debug.Log("Listen Dealer ID => " + args[0].ToString());
                JSONNode jsonString = JSONNode.Parse(args[0].ToString());
                Aciotn_dealerID?.Invoke(args);
            }
            else
            {
                Debug.Log("ESLE OF DREALE BECAUSE IT's SINGLE BET STYLE");
            }
        }

        private void OnSitOut(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.SingleBet)
            {
                Debug.Log("Listen SIT OUT ID => " + args[0].ToString());
                JSONNode jsonString = JSONNode.Parse(args[0].ToString());
                Action_SitOut?.Invoke(args);
            }
            else
            {
                Debug.Log("ESLE OF DREALE BECAUSE IT's POKER BET STYLE");
            }
        }



        private void OnReadyToBet(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Player Ready To Start Bet Received : " + args[0].ToString());
            StaticDataHandler.MyChips -= StaticDataHandler.GetTwoDecimalValue(ImportantDataMembers.Instance.currentSelectedStack);
            BetScreenUI.Instance.ShowWaitingStatus(false);
        }

        private void OnReceivePlayerTurn(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Player Bet Turn Received : " + args[0].ToString());
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());
            string playerId = jsonString["playerId"];
            bool check = jsonString["check"];
            bool raise = jsonString["raise"];
            bool fold = jsonString["fold"];
            bool call = jsonString["call"];

            BetScreenUI.Instance.OnPlayerTurn(playerId, check, raise, fold, call);
        }

        private void OnReceivePlayerBetTurnTimer(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Player Bet Turn Timer Received : " + args[0].ToString());

            JSONNode playerList = JSONNode.Parse(args[0].ToString());

            string playerID = playerList["playerId"];
            string timer = playerList["timer"];
            string totalTime = playerList["totalTime"];

            onReceivedBettinTurnTimer?.Invoke(playerID, timer, totalTime);

            if (timer == "0" && playerID == StaticDataHandler.MyPlayerID)
            {
                BetScreenUI.Instance.OnClickCallButton(1);
            }
        }

        private void OnReceivedPlayerAction(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Player Action  Data Received : " + args[0].ToString());
            playerActionReflectOtherPlayer?.Invoke(args);
        }

        private void OnAllPLayerbetDone(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("All player bet done : " + args[0].ToString());
            //Invoke(nameof(AllPlayerBetDone), 2);
        }

        private void AllPlayerBetDone()
        {
            Debug.Log("App player bet doen method call");
            BetScreenUI.Instance.ShowWaitingStatus(true, "Processing...");
        }

        private void OnGameStartTimer(Socket socket, Packet packet, object[] args)
        {
            //            Debug.Log("timer => " + args[0].ToString());
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());
            string timer = jsonString["timer"];

            PlayerDetails player = AllAction_EventHandler.Instance.PlayerDetailsList.Find(x => x.PlayerID == StaticDataHandler.MyPlayerID);
            //  Debug.Log("--------> is Player waiting state : " + player.isWaiting);

            if (!player.isWaiting)
            {
                BetScreenUI.Instance.ShowWaitingStatus(false, "");
            }
            else
            {
                BetScreenUI.Instance.ShowWaitingStatus(true, "Waiting for next round");
            }

            StaticDataHandler.inRound = false;
            BetScreenUI.Instance.roundStartTxt.gameObject.SetActive(true);
            //   BetScreenUI.Instance.roundStartTxt.text = "Next round starts in " + timer + "  seconds";




            Text txt = BetScreenUI.Instance.roundStartTxt;

            string localizationKEY = "DYNAMIC.NEXTROUNDSTARTIN";
            string localizationKEY1 = "DYNAMIC.SECONDS";
            if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
            {
                txt.font = Example.INSTANCE.mChineseSimplyfied;
            }
            else
            {
                txt.font = Example.INSTANCE.mEnglishTextMultiBold;
            }

            txt.text = LocalizationManager.Localize(localizationKEY) + " " + timer.ToString() + " " + LocalizationManager.Localize(localizationKEY1);







            if (timer == "0") BetScreenUI.Instance.roundStartTxt.gameObject.SetActive(false);
        }

        private void OnWaitingForNextRound(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Ready To Waiting For Next Round : " + args[0].ToString());
            BetScreenUI.Instance.roundStartTxt.gameObject.SetActive(false);
            BetScreenUI.Instance.ShowWaitingStatus(true, "Waiting for next round");
        }

        private void OnWaitingForOtherPlayer(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Ready To Waiting For Other Player : " + args[0].ToString());
            if (BetScreenUI.Instance)
            {
                BetScreenUI.Instance.roundStartTxt.gameObject.SetActive(false);
                BetScreenUI.Instance.ShowWaitingStatus(true, "Waiting for other players");
            }
        }

        private void OnGameReadyToStart(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Ready To Load Game Scene : " + args[0].ToString());

            PlayerDetails player = AllAction_EventHandler.Instance.PlayerDetailsList.Find(x => x.PlayerID == StaticDataHandler.MyPlayerID);
            Debug.Log("--------> is Player waiting state : " + player.isWaiting);

            if (!player.isWaiting && !StaticDataHandler.sitOutForNextRound)
            {
                if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Thousand)
                {
                    SceneManager.LoadScene("1_StopTheClock");
                }
                else if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Blink)
                {
                    SceneManager.LoadScene("1_Cat Blink");
                }
            }
        }

        private void OnWinnerPlayerData(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Winner Data : " + args[0].ToString());
            BetScreenUI.Instance.ShowWaitingStatus(false);
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());

            float systemWinChips = jsonString["systemAmount"];
            int winnerIndex = 0;

            if (jsonString["playerData"].Count > 0)
            {
                //for (int i = 0; i < jsonString["playerData"].Count; i++)
                //{
                //}
                string winnerId = jsonString["playerData"][0]["playerId"];
                string chips = jsonString["playerData"][0]["chips"];

                for (int i = 0; i < BetScreenUI.Instance.playerScripts.Count; i++)
                {
                    if (BetScreenUI.Instance.playerScripts[i].PlayerID == winnerId) { winnerIndex = i; break; }
                }

                print("winning index == " + winnerIndex + ", Chips => " + StaticDataHandler.GetTwoDecimalValue(chips));
                if (winnerId == StaticDataHandler.MyPlayerID) StaticDataHandler.MyChips += StaticDataHandler.GetTwoDecimalValue(chips);
                BetScreenUI.Instance.playerScripts[winnerIndex].rays.SetActive(true);
                BetScreenUI.Instance.playerScripts[winnerIndex].crownWinner.SetActive(true);

                GameObject winChipAnim = Instantiate(BetScreenUI.Instance.TotalChipsWinPrefab, BetScreenUI.Instance.betSceneParent);
                winChipAnim.GetComponent<WinChipsAnimController>().WinChipsAnimation(StaticDataHandler.GetTwoDecimalValue(chips), StaticDataHandler.GetTwoDecimalValue(systemWinChips), winnerIndex);
                //BetScreenUI.Instance.totalStacks.transform.GetComponentInChildren<Text>().text = "$" + StaticDataHandler.GetTwoDecimalString(chips);
                //BetScreenUI.Instance.totalStacks.transform.DOMove(BetScreenUI.Instance.playerScripts[winnerIndex].gameObject.transform.position, 2);
            }

            StartCoroutine(ASD(winnerIndex));
        }

        IEnumerator ASD(int winnerIndex)
        {
            print("WINNER INDES : " + winnerIndex + "   :   " + BetScreenUI.Instance.playerScripts.Count);

            BetScreenUI.Instance.totalStacks.SetActive(false);
            yield return new WaitForSeconds(2);
            yield return new WaitForSeconds(0.5f);
            //BetScreenUI.Instance.totalStacks.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BetScreenUI.Instance.totalStacks.SetActive(true);
            BetScreenUI.Instance.totalStacks.transform.GetComponentInChildren<Text>().text = "$" + StaticDataHandler.totalStacksForRound;

            BetScreenUI.Instance.playerScripts[winnerIndex].rays.SetActive(false);
            BetScreenUI.Instance.playerScripts[winnerIndex].crownWinner.SetActive(false);
        }

        private void ChangePokerGameAfterWin(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("New Game Data : " + args[0].ToString());
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());
            int gameIndex = jsonString["gameType"];
            Network_Gatekeeper.Instance.ChoosePokerGameType(gameIndex);
        }

        //=========================================================================================================
        //====================  STOP THE CLOCK GAME EVENTS   ======================================================
        //=========================================================================================================


        private void OnReceiveThousandCountDown(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Thousand)
            {
                Debug.Log("THOUSAND Count Down Received : " + args[0].ToString());
                onReceiveThousandCountDown_ACN?.Invoke(args);
            }
        }

        private void OnReceiveThousandTimer(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Thousand)
            {
                Debug.Log("THOUSAND Timer Data Received : " + args[0].ToString());
                onReceivedThousandTimer_ACN?.Invoke(args);
            }
        }

        private void OnReceiveThousandMyScore(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Thousand)
            {
                Debug.Log("THOUSAND Score : " + args[0].ToString());
                onReceivedThousandPlayerScore_ACN?.Invoke(args);
            }
        }

        //=========================================================================================================
        //==================== SCARE CAT GAME EVENTS   ===============================================================
        //=========================================================================================================


        private void OnReceiveBlinkCountDown(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Blink)
            {
                //  Debug.Log("Blink Count Down Received : " + args[0].ToString());
                onReceinveBlinkCountDown_ACN?.Invoke(args);
            }
        }

        private void OnReceiveBlinkTimer(Socket socket, Packet packet, object[] args)
        {
            if (ImportantDataMembers.Instance.currentSelectedGame == GAMES.Blink)
            {
                //   Debug.Log("BLINK  Timer Data Received : " + args[0].ToString());
                onReceivedBlinkTimer_ACN?.Invoke(args);
            }
        }

        private void OnReceiveBlinkMyScore(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("BLINK Score : " + args[0].ToString());
            onReceivedBlinkPLayerScore_ACN?.Invoke(args);
        }

        //======================================================================================================================================================
        //======================================================================================================================================================

    }
}
