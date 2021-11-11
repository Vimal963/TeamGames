using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace TeamGame
{
    /// <summary>
    /// PlayerDetails class used to store player details like,name,id ,chips etc
    /// </summary>
    [Serializable]
    public class PlayerDetails
    {
        public string PlayerID;
        public string PlayerName;
        public float PlayerChips;

        public bool isJoinedSuccess;
        public bool isOwner;
        public bool isSetted;
        public bool isPlaying;
        public bool isWaiting;

        public Sprite PlayerSprite;
    }

    /// <summary>
    /// this is use to player spawn here and set it place on bet table and manage if some player left game or table
    /// </summary>
    public class AllAction_EventHandler : MonoBehaviour
    {
        public static AllAction_EventHandler Instance;

        public List<string> OnlinePlayerIds = new List<string>();
        public List<PlayerDetails> PlayerDetailsList = new List<PlayerDetails>();


        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            StartCoroutine(WaitForInstance());
        }

        /// <summary>
        /// register and de-register all event of socket
        /// </summary>
        /// <returns></returns>

        private IEnumerator WaitForInstance()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            Network_Listener.Instance.onTablePlayerList_ACN += GetTablePlayerList;
            Network_Listener.Instance.onPlayerLeft_ACN += OnPlayerLeft;
            //Network_Listener.Instance.onPlayerWaitingList_ACN += OnPlayerWaitingList;
        }

        private void OnDisable()
        {
            Network_Listener.Instance.onTablePlayerList_ACN -= GetTablePlayerList;
            Network_Listener.Instance.onPlayerLeft_ACN -= OnPlayerLeft;
            //Network_Listener.Instance.onPlayerLeft_ACN -= OnPlayerWaitingList;
        }


        /// <summary>
        /// reset all data when player disconnect from socket
        /// </summary>
        public void ClearDataOnDisconnetPlayer()
        {
            OnlinePlayerIds.Clear();
            PlayerDetailsList.Clear();
            if (FindObjectOfType<BetScreenUI>() != null) BetScreenUI.Instance.playerScripts.Clear();
            StaticDataHandler.inRound = false;
        }


        /// <summary>
        /// get all player on bet table as come as on table
        /// </summary>

        int myIndexInList = 0;
        private void GetTablePlayerList(object[] args)
        {
            PlayerDetailsList.Clear();  // this is for when user come from minigame to bet table
            OnlinePlayerIds.Clear();    // this is for when user come from minigame to bet table

            JSONNode playerList = JSONNode.Parse(args[0].ToString());
            if (playerList["playerData"].Count > 8) return;

            for (int i = 0; i < playerList["playerData"].Count; i++)
            {
                string playerID = playerList["playerData"][i]["playerId"];
                string playerName = playerList["playerData"][i]["name"];
                string chips = playerList["playerData"][i]["chips"];
                string profilePic = playerList["playerData"][i]["profilePicture"];
                bool isPlaying = playerList["playerData"][i]["isPlaying"];
                bool isWaiting = playerList["playerData"][i]["isWaiting"];


                if (playerID == StaticDataHandler.MyPlayerID)
                {
                    myIndexInList = i;
                    //StaticDataHandler.MyChips = StaticDataHandler.GetTwoDecimalValue(chips);
                }

                if (!OnlinePlayerIds.Contains(playerID))
                {
                    PlayerDetails playerDetails = new PlayerDetails
                    {
                        PlayerID = playerID,
                        PlayerName = playerName,
                        PlayerChips = StaticDataHandler.GetTwoDecimalValue(chips),
                        PlayerSprite = ImportantDataMembers.Instance.PokerProfilePics[int.Parse(profilePic)],
                        isPlaying = isPlaying,
                        isWaiting = isWaiting
                    };

                    PlayerDetailsList.Add(playerDetails);
                    OnlinePlayerIds.Add(playerID);
                }
            }

            SpawnPlayers();
        }

        /// <summary>
        /// use to spawn player on bet table
        /// </summary>
        private void SpawnPlayers()
        {
            int j = 0;
            if (BetScreenUI.Instance)
            {
                foreach (PlayerScript ps in BetScreenUI.Instance.playerScripts) Destroy(ps.gameObject);
                BetScreenUI.Instance.playerScripts.Clear();

                for (int i = myIndexInList; i < PlayerDetailsList.Count; i++)
                {
                    PlaceSpawnPlayer(i, j);
                    j += 1;
                }

                for (int i = 0; i < myIndexInList; i++)
                {
                    PlaceSpawnPlayer(i, j);
                    j += 1;
                }
            }
        }

        /// <summary>
        /// metho use to set spawned player postiton on bet table
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="positionIndex"></param>
        private void PlaceSpawnPlayer(int playerIndex, int positionIndex)
        {
            if (BetScreenUI.Instance)
            {
                //if (PlayerDetailsList[i].isSetted) return;
                PlayerDetailsList[playerIndex].isSetted = true;
                GameObject player = Instantiate(BetScreenUI.Instance.playerPrefab);
                player.name = PlayerDetailsList[playerIndex].PlayerID;
                player.transform.SetParent(BetScreenUI.Instance.playerContainer[positionIndex], false);
                player.transform.localPosition = Vector3.zero;

                player.GetComponent<PlayerScript>().PlayerID = PlayerDetailsList[playerIndex].PlayerID;
                player.GetComponent<PlayerScript>().PlayerName = PlayerDetailsList[playerIndex].PlayerName;
                player.GetComponent<PlayerScript>().playerImg.sprite = PlayerDetailsList[playerIndex].PlayerSprite;
                player.GetComponent<PlayerScript>().txtPlayerName.text = PlayerDetailsList[playerIndex].PlayerName;
                player.GetComponent<PlayerScript>().txtPlayerBet.text = "Bet $" + StaticDataHandler.GetTwoDecimalString(ImportantDataMembers.Instance.currentSelectedStack);
                //player.GetComponent<PlayerScript>().txtPlayerChips.text = "$" + StaticDataHandler.GetTwoDecimalString(PlayerDetailsList[playerIndex].PlayerChips);

                BetScreenUI.Instance.playerScripts.Add(player.GetComponent<PlayerScript>());
            }
        }



        /// <summary>
        /// OnPlayerLeft use to remove player from bettable, this will call when listen socket event "LISTEN_PLAYER_LEFT" of staticdatahandler class
        /// </summary>
        /// <param name="args"></param>
        private void OnPlayerLeft(object[] args)
        {
            JSONNode playerList = JSONNode.Parse(args[0].ToString());

            try
            {
                string playerID = playerList["playerId"];

                int index = OnlinePlayerIds.IndexOf(playerID);
                PlayerDetailsList.RemoveAt(index);
                OnlinePlayerIds.RemoveAt(index);

                GameObject leftplayer = GameObject.Find(playerID);
                if (leftplayer != null)
                {
                    BetScreenUI.Instance.playerScripts.Remove(leftplayer.GetComponent<PlayerScript>());
                    Destroy(leftplayer);
                }
            }
            catch
            {
                throw new Exception();
            }
        }

    }
}
