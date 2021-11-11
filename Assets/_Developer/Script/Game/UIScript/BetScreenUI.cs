using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamGame
{
    /// <summary>
    /// this is betScreen manager
    /// here we will code for set player's  buttons like call,check,fold ,ber progress,etc
    /// </summary>

    public class BetScreenUI : MonoBehaviour
    {
        public static BetScreenUI Instance;

        public GameObject WaitingScreen;
        public GameObject playerPrefab, betAnimPrefab, TotalChipsWinPrefab;
        public Transform betSceneParent;
        public Transform[] playerContainer;
        public List<PlayerScript> playerScripts;

        public Image myPic;
        public Text myNameTxt, myChipsTxt;
        public Text roundStartTxt;
        public GameObject totalStacks;
        public GameObject singBetItems, pokerRulesItmes, ticMark;

        public GameObject sliderValue;
        public Slider betSlider;

        public Button callButton, checkButton, foldButton, raiseButton;


        private void Awake()
        {
            Instance = GetComponent<BetScreenUI>();
        }


        private void Start()
        {
            if (StaticDataHandler.inRound)
            {
                ShowWaitingStatus(true, "Waiting for other players");
                Network_Emitter.Instance.Emit_WinnerPlayerData();
            }
            else
            {
                ShowWaitingStatus(true, "Waiting for other players");
            }

            ImportantDataMembers.Instance.currentScene = SCENE.Poker;

            myPic.sprite = ImportantDataMembers.Instance.PokerProfilePics[StaticDataHandler.MyProfilePic];
            myNameTxt.text = StaticDataHandler.MyPlayerName;

            singBetItems.SetActive(ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.SingleBet);
            pokerRulesItmes.SetActive(ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.PokerRule);
            ticMark.SetActive(StaticDataHandler.sitOutForNextRound);

            betSlider.minValue = float.Parse(ImportantDataMembers.Instance.currentSelectedStack);
            betSlider.maxValue = StaticDataHandler.MyChips;
            sliderValue.SetActive(false);

            betSlider.interactable = false;
            callButton.interactable = false;
            checkButton.interactable = false;
            foldButton.interactable = false;
            raiseButton.interactable = false;
        }

        public void ShowWaitingStatus(bool show, string info = "")
        {
            //            Debug.Log("Show staus here..");
            if (WaitingScreen)
            {
                if (show)
                {
                    WaitingScreen.SetActive(true);

                    WaitingScreen.GetComponentInChildren<Text>().text = info;

                    Text txt = WaitingScreen.GetComponentInChildren<Text>();

                    if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
                    {
                        txt.font = Example.INSTANCE.mChineseSimplyfied;
                    }
                    else
                    {
                        txt.font = Example.INSTANCE.mEnglishTextMultiBold;
                    }

                    string localizationKEY = "";
                    switch (info)
                    {
                        case "Waiting for other players":
                            localizationKEY = "DYNAMIC.WAITINGFORPLAYER";
                            break;
                        case "Processing...":
                            localizationKEY = "";
                            break;
                        case "Waiting for next round":
                            localizationKEY = "DYNAMIC.WAITINGFORNEXTROUND";
                            break;

                        default:
                            break;
                    }
                    txt.text = LocalizationManager.Localize(localizationKEY);

                }
                else
                {
                    WaitingScreen.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Waiting screen NUll");
            }
        }


        private void Update()
        {
            myChipsTxt.text = "$" + StaticDataHandler.GetTwoDecimalString(StaticDataHandler.MyChips);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnClickBack(1);
            }
        }

        public void OnClickBack(int val = 0)
        {
            if (val == 0) if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            Network_Emitter.Instance.Emit_OnPlayerLeft();
            StaticDataHandler.LoadHomeScene();
        }

        public void OnPlayerTurn(string id, bool check, bool raise, bool fold, bool call)
        {
            if (id == StaticDataHandler.MyPlayerID)
            {
                betSlider.interactable = true;
                checkButton.interactable = check;
                raiseButton.interactable = raise;
                foldButton.interactable = fold;
                callButton.interactable = call;
            }
            else
            {
                betSlider.interactable = false;
                checkButton.interactable = false;
                raiseButton.interactable = false;
                foldButton.interactable = false;
                callButton.interactable = false;
            }
        }

        public void SetOutForNextRound()
        {
            StaticDataHandler.sitOutForNextRound = !StaticDataHandler.sitOutForNextRound;
            ticMark.SetActive(StaticDataHandler.sitOutForNextRound);
            Network_Emitter.Instance.Emit_SitOutForNextRound();
        }

        public void OnBetSliderChangeValue()
        {
            sliderValue.SetActive(true);
            sliderValue.GetComponentInChildren<Text>().text = betSlider.value.ToString();
        }

        public void OnPointerUpBetSlider()
        {
            sliderValue.SetActive(false);
        }

        public void OnClickCheckBtn()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            OnPlayerTurn("", false, false, false, false);
            //if (playerScripts[0].PlayerID == StaticDataHandler.MyPlayerID) playerScripts[0].SetMyBet();

            Network_Emitter.Instance.Emit_OnPlayerBet(0, "check");
        }

        public void OnClickRaiseBtn()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            OnPlayerTurn("", false, false, false, false);
            StaticDataHandler.MyChips -= StaticDataHandler.GetTwoDecimalValue(betSlider.value);
            betSlider.minValue = StaticDataHandler.GetTwoDecimalValue(betSlider.value);
            //if (playerScripts[0].PlayerID == StaticDataHandler.MyPlayerID) playerScripts[0].SetMyBet();

            Network_Emitter.Instance.Emit_OnPlayerBet(StaticDataHandler.GetTwoDecimalValue(betSlider.value), "raise");
        }

        public void OnClickFoldBtn()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            OnPlayerTurn("", false, false, false, false);
            //if (playerScripts[0].PlayerID == StaticDataHandler.MyPlayerID) playerScripts[0].SetMyBet();

            Network_Emitter.Instance.Emit_OnPlayerBet(0, "fold");
        }

        public void OnClickCallButton(int val = 0)
        {
            if (val == 0) if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            OnPlayerTurn("", false, false, false, false);
            StaticDataHandler.MyChips -= StaticDataHandler.GetTwoDecimalValue(ImportantDataMembers.Instance.currentSelectedStack);
            //if (playerScripts[0].PlayerID == StaticDataHandler.MyPlayerID) playerScripts[0].SetMyBet();

            Network_Emitter.Instance.Emit_OnPlayerBet(StaticDataHandler.GetTwoDecimalValue(ImportantDataMembers.Instance.currentSelectedStack), "call");
        }

    }
}
