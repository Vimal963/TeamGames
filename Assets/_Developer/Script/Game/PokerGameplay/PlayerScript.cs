using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

namespace TeamGame
{

    /// <summary>
    /// this script atteched to PlayerPreafb in bet screen
    /// here we will set player info like which player betting,whis\ch is Dealer etc
    /// SetPlayerBetProgress() is show's player ber progress
    /// OnSitOutCall() used when some player do sit out for next round.
    /// </summary>

    public class PlayerScript : MonoBehaviour
    {
        public string PlayerID, PlayerName;

        public bool isJoinedSuccess;

        public Image playerImg;
        public Text txtPlayerName;
        public Text txtPlayerBet;
        public GameObject progressParent;
        public Image filedProgress;
        public GameObject border;
        public GameObject rays;
        public GameObject crownWinner;
        public GameObject dealerIcon;
        public GameObject sitOutIcon;


        [SerializeField] private Color checkColor, raiseColor, foldColor, callColor;
        private float turnTime;


        private void OnEnable()
        {
            Network_Listener.Instance.onReceivedBettinTurnTimer += SetPlayerBetProgress;
            Network_Listener.Instance.playerActionReflectOtherPlayer += PlayerActionReflectOtherPlayer;
            Network_Listener.Instance.Aciotn_dealerID += OnDealerCall;
            Network_Listener.Instance.Action_SitOut += OnSitOutCall;

        }

        private void OnDisable()
        {
            Network_Listener.Instance.onReceivedBettinTurnTimer -= SetPlayerBetProgress;
            Network_Listener.Instance.playerActionReflectOtherPlayer -= PlayerActionReflectOtherPlayer;
            Network_Listener.Instance.Aciotn_dealerID -= OnDealerCall;
            Network_Listener.Instance.Action_SitOut -= OnSitOutCall;
        }

        private void Start()
        {
            progressParent.SetActive(false);
        }

        public void OnDealerCall(object[] args)
        {
            JSONNode playerList = JSONNode.Parse(args[0].ToString());
            Debug.Log("GET Dealer IDDI iID : " + playerList.ToString());

            string dealerID = playerList["delarId"];

            if (!dealerID.Equals(PlayerID))
            {
                dealerIcon.SetActive(false);
                return;
            }
            else
            {
                dealerIcon.SetActive(true);
            }
        }

        public void OnSitOutCall(object[] args)
        {
            JSONNode json = JSONNode.Parse(args[0].ToString());

            string sitOutID = json["playerId"];
            bool isSitOut = bool.Parse(json["sitOut"].Value);

            Debug.Log("GET Sit Out ID : " + sitOutID + " MY PLayer ID : " + StaticDataHandler.MyPlayerID + "   Gen P ID" + PlayerID);

            Debug.Log("COndi : " + (sitOutID == PlayerID));
            if (sitOutID == PlayerID)
            {
                sitOutIcon.SetActive(isSitOut);
            }
        }

        public void SetPlayerBetProgress(string id, string timer, string totalTime)
        {
            if (!id.Equals(PlayerID))
            {
                rays.SetActive(false);
                progressParent.SetActive(false);
                filedProgress.fillAmount = 0;
                return;
            }
            else
            {
                rays.SetActive(true);
                turnTime = float.Parse(totalTime);
                progressParent.SetActive(true);
                filedProgress.fillAmount += 1 / turnTime;
            }
        }

        public void SetMyBet()
        {
            rays.SetActive(false);
            progressParent.SetActive(false);
            filedProgress.fillAmount = 0;
        }


        /// <summary>
        /// used to show what player had done like chips of bet.,raise,fold,etc
        /// </summary>
        /// <param name="args"></param>
        public void PlayerActionReflectOtherPlayer(object[] args)
        {
            JSONNode playerList = JSONNode.Parse(args[0].ToString());
            Debug.Log("Player Action Get : " + playerList.ToString());

            string playerID = playerList["playerId"];
            string stackVal = playerList["stack"];
            stackVal = StaticDataHandler.GetTwoDecimalString(stackVal);

            if (playerID == PlayerID)
            {
                if (StaticDataHandler.GetTwoDecimalValue(stackVal) != 0)
                {
                    GameObject obj = Instantiate(BetScreenUI.Instance.betAnimPrefab);
                    obj.transform.SetParent(BetScreenUI.Instance.betSceneParent, false);
                    obj.transform.position = GameObject.Find(playerID).transform.position;
                    obj.GetComponentInChildren<Text>().text = "$" + stackVal;
                    obj.transform.DOMove(BetScreenUI.Instance.betSceneParent.position, 1.3f)
                        .OnComplete(() => Destroy(obj))
                        .SetEase(Ease.Linear);
                }

                if (playerList["check"])
                {
                    border.GetComponent<Image>().color = checkColor;
                    txtPlayerBet.color = checkColor;
                    txtPlayerBet.text = "Check";
                }
                else if (playerList["raise"])
                {
                    border.GetComponent<Image>().color = raiseColor;
                    txtPlayerBet.color = raiseColor;
                    txtPlayerBet.text = "Raise $" + stackVal;
                    BetScreenUI.Instance.betSlider.minValue = StaticDataHandler.GetTwoDecimalValue(stackVal);
                }
                else if (playerList["fold"])
                {
                    border.GetComponent<Image>().color = foldColor;
                    txtPlayerBet.color = foldColor;
                    txtPlayerBet.text = "Fold";
                }
                else if (playerList["call"])
                {
                    border.GetComponent<Image>().color = callColor;
                    txtPlayerBet.color = callColor;
                    txtPlayerBet.text = "Called $" + stackVal;
                }

                rays.SetActive(false);
                border.SetActive(true);
                progressParent.SetActive(false);
                filedProgress.fillAmount = 0;
            }
        }

    }
}