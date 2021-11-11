using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

namespace TeamGame
{
    /// <summary>
    /// here we will make code for which screen currenlty show
    /// method call for player logout
    /// controlling Device Escap button like player press escap in homescreen UI then move that player to wallet scren,
    /// ShowScreenAnimation() method used to show specificPanel orUI screen
    /// </summary>


    public enum ALL_UI_SCREENS
    {
        Welcome,
        ConnectWallet,
        HomeScreen,
        PickTeam,
        PickTeamError,
        PickBettingStyle,
        Tournament,
        PickGame,
        PickStack,
        GamePlay,
        Loading,
        underProgress,
        login,
        register,
        Log_reg_Home,
        LeaderBoard,
        PlayerProfile
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public GameObject bg1, bg2, bg3;
        public GameObject menuPanel;
        public List<Panels> allPanels;
        public ALL_UI_SCREENS lunchScreen;
        public ALL_UI_SCREENS currentScreen;

        public GameObject logoutButton;
        public GameObject logoutConformationDialog;
        public GameObject settingPanel;
        public GameObject LoadingScreen;

        // Start is called before the first frame update
        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            StaticDataHandler.ResetSomeData();
            ImportantDataMembers.Instance.currentScene = SCENE.Home;

            AudioScript.Instant.PlayBGMusic();
            settingPanel.SetActive(false);
            logoutConformationDialog.SetActive(false);
            menuPanel.SetActive(false);
            if (!StaticDataHandler.IsLoggedIn)
            {
                ShowScreen(ALL_UI_SCREENS.Log_reg_Home);
                logoutButton.SetActive(false);
            }
            else
            {
                logoutButton.SetActive(true);
                if (ImportantDataMembers.Instance.isInitGame)
                {
                    ImportantDataMembers.Instance.isInitGame = false;
                    ShowScreen(ALL_UI_SCREENS.Welcome);
                }
                else
                {
                    ShowScreen(ImportantDataMembers.Instance.PreviousScreen);
                }
            }
        }

        public void OnLogOut()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            logoutConformationDialog.SetActive(true);
        }

        public void OnClickLogout_NO()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            logoutConformationDialog.SetActive(false);
        }

        public void OnClickLogout_YES()
        {
            if (AudioScript.Instant)
                AudioScript.Instant.PlayBtnClickSound();

            int musicPrefsVal = PlayerPrefs.GetInt(AudioScript.musicKey, 1);
            int soundPrefsVal = PlayerPrefs.GetInt(AudioScript.soundKey, 1);

            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt(AudioScript.soundKey, soundPrefsVal);
            PlayerPrefs.SetInt(AudioScript.musicKey, musicPrefsVal);



            ImportantDataMembers.Instance.isLoggedIn = false;
            ShowScreen(ALL_UI_SCREENS.Log_reg_Home);
            logoutConformationDialog.SetActive(false);
            logoutButton.SetActive(false);
        }

        public void onClickMenuButton()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            menuPanel.SetActive(true);
        }


        public void onClickSettingButton()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            settingPanel.SetActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (currentScreen)
                {
                    case ALL_UI_SCREENS.Log_reg_Home:
                        //Application.Quit();
                        break;

                    case ALL_UI_SCREENS.login:
                        if (LoadingScreen.activeInHierarchy)
                        {
                            LoadingScreen.SetActive(false);
                        }
                        break;

                    case ALL_UI_SCREENS.register:
                        if (LoadingScreen.activeInHierarchy)
                        {
                            LoadingScreen.SetActive(false);
                        }
                        break;

                    case ALL_UI_SCREENS.Welcome:
                        //Application.Quit();
                        break;

                    case ALL_UI_SCREENS.ConnectWallet:
                        ShowScreen(ALL_UI_SCREENS.Welcome);
                        break;

                    case ALL_UI_SCREENS.LeaderBoard:
                        ShowScreen(ALL_UI_SCREENS.HomeScreen);
                        break;

                    case ALL_UI_SCREENS.HomeScreen:
                        ShowScreen(ALL_UI_SCREENS.ConnectWallet);
                        break;

                    case ALL_UI_SCREENS.PickTeam:
                        ShowScreen(ALL_UI_SCREENS.HomeScreen);
                        break;

                    case ALL_UI_SCREENS.PickTeamError:
                        FindObjectOfType<PickTeamErrorUI>().OnClickClose(1);
                        break;

                    case ALL_UI_SCREENS.PickBettingStyle:
                        FindObjectOfType<PickBetingStyleUI>().OnClickBack(1);
                        break;

                    case ALL_UI_SCREENS.Tournament:
                        FindObjectOfType<TournamentUI>().OnClickBack(1);
                        break;

                    case ALL_UI_SCREENS.PickGame:
                        FindObjectOfType<PickGameUI>().OnClickBack(1);
                        break;

                    case ALL_UI_SCREENS.PickStack:
                        FindObjectOfType<PickStackUI>().OnClickBack();
                        break;

                    default:
                        //Application.Quit();
                        break;
                }
            }
        }


        public void OnClickLogin()
        {
            //  HideAllScreen();

        }

        public void OnClickSignUp()
        {
            // HideAllScreen();
        }

        public void ShowScreen(ALL_UI_SCREENS uiScreen)
        {
            StartCoroutine(ShowScreenAnimation(uiScreen));
        }

        private IEnumerator ShowScreenAnimation(ALL_UI_SCREENS uiScreen)
        {
            Panels current_Panel = allPanels.Where(n => n.uiScreenName == currentScreen).FirstOrDefault();
            //  current_Panel.panelObject.SetActive(true);
            if (current_Panel.animation != null)
            {
                current_Panel.animation.Play(current_Panel.clip_hide.name);
                yield return new WaitForSeconds(0.5f);
            }
            foreach (Panels item in allPanels)
            {
                item.panelObject.SetActive(false);
            }
            currentScreen = uiScreen;
            Panels next_Panel = allPanels.Where(n => n.uiScreenName == uiScreen).FirstOrDefault();
            next_Panel.panelObject.SetActive(true);

            if (next_Panel.animation != null)
            {
                next_Panel.animation.Play(next_Panel.clip_show.name);
            }

            menuPanel.SetActive(false);

            if (uiScreen == ALL_UI_SCREENS.Welcome || uiScreen == ALL_UI_SCREENS.login || uiScreen == ALL_UI_SCREENS.register)
            {
                bg1.SetActive(true);
                bg2.SetActive(false);
                bg3.SetActive(false);
            }
            else if (uiScreen == ALL_UI_SCREENS.ConnectWallet || uiScreen == ALL_UI_SCREENS.HomeScreen || uiScreen == ALL_UI_SCREENS.PickTeam || uiScreen == ALL_UI_SCREENS.LeaderBoard)
            {
                bg1.SetActive(false);
                bg2.SetActive(true);
                bg3.SetActive(false);
            }
            else if (uiScreen == ALL_UI_SCREENS.PickBettingStyle || uiScreen == ALL_UI_SCREENS.PickGame || uiScreen == ALL_UI_SCREENS.PickStack || uiScreen == ALL_UI_SCREENS.PlayerProfile)
            {
                bg1.SetActive(false);
                bg2.SetActive(false);
                bg3.SetActive(true);
            }
        }

    }
}