using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


/// <summary>
/// TEAM enum for selection of team from given list
/// </summary>
public enum TEAM { Usa, China, Practice }


/// <summary>
/// GAMES enum for selection of Games from given list
/// </summary>
public enum GAMES { Random, Thousand, Blink, Red, Spinner, Boom }


/// <summary>
/// BETTING_STYLE for selection BET type
/// </summary>
public enum BETTING_STYLE { SingleBet, PokerRule, Tournament }


/// <summary>
/// here you chan choese language
/// </summary>
public enum LANGUAGE { English, Chinese }


/// <summary>
/// her is a list of scene used in game(for online games only) practice scene not mentioned here
/// </summary>
public enum SCENE { Home, Poker, Clock, Cat }

namespace TeamGame
{

    /// <summary>
    /// here will define all globel memners and data members that will need in entire game
    /// also will get and store out stacks from backend by using GetOnLIneStackList() method below
    /// </summary>

    public class ImportantDataMembers : MonoBehaviour
    {
        public static ImportantDataMembers Instance;

        /// <summary>
        /// these are some objects and members are be used in games later
        /// </summary>

        public TEAM currentPickedTeam;
        public GAMES currentSelectedGame;
        public BETTING_STYLE currentBettingStyle;
        public LANGUAGE currentLanguage;
        public SCENE currentScene;

        public string currentSelectedStack;

        public ALL_UI_SCREENS PreviousScreen;

        public GameObject WarningPanel;
        public bool isInitGame;
        public bool isLoggedIn;

        public List<Sprite> ProfilePics = new List<Sprite>();
        public List<Sprite> PokerProfilePics = new List<Sprite>();
        public List<string> OnlineStackList;


        private void Awake()
        {
            //PlayerSettings.WebGL.emscriptenArgs = "-s TOTAL_MEMORY=55MB";

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }

            Input.multiTouchEnabled = false;
            //isInitGame = true;
            isLoggedIn = false;
            currentLanguage = (LANGUAGE)StaticDataHandler.Language;
        }


        void Start()
        {
            InvokeRepeating("WaitForInternet", 0, 1);
        }

        void WaitForInternet()
        {
            if (!StaticDataHandler.IsInternetAccessUnavailable())
            {
                StartCoroutine(GetOnLIneStackList());
                CancelInvoke("WaitForInternet");
            }
        }


        /// <summary>
        /// get stacks(chips) list from backand
        /// </summary>
        /// <returns></returns>
        private IEnumerator GetOnLIneStackList()
        {
            string url = StaticDataHandler.URL_STACK;
            UnityWebRequest result = UnityWebRequest.Post(url, "{}");
            yield return result.SendWebRequest();

            Debug.Log("Res Code : " + result.responseCode);
            Debug.Log("STACK RESPONSE JSON : " + result.downloadHandler.text);

            if (result.responseCode == 0)
            {
                Debug.Log("Server did not respond.");
            }
            else if (result.isNetworkError || result.isHttpError)
            {
                Debug.Log("Something went to wrong..");
            }
            else
            {
                SetStacksData(result.downloadHandler.text);
            }
        }

        private void SetStacksData(string response)
        {
            JSONNode jsonNode = JSON.Parse(response);
            OnlineStackList = new List<string>();

            for (int i = 0; i < jsonNode.Count; i++)
            {
                string stackVal = "$" + jsonNode[i]["stack"].Value;

                OnlineStackList.Add(stackVal);
            }
        }


        /// <summary>
        /// this will used for some server warning like yo are disconneted or somthing info
        /// </summary>
        public void ShowWarningPanel()
        {
            if (currentScene != SCENE.Home)
            {
                if (GameObject.Find("WarningPanel") == null)
                {
                    GameObject warningPanel = Instantiate(WarningPanel, GameObject.Find("Canvas").transform);
                    warningPanel.name = "WarningPanel";
                    warningPanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { StaticDataHandler.LoadHomeScene(); });
                }
            }
        }
    }


    /// <summary>
    /// this will used for setup winscreen data
    /// </summary>
    [System.Serializable]
    public class WinScreen
    {
        private bool winScrrenShow;
        public GameObject winScreenDialog, connectedImg, disConnectedImg;
        public Text txtScore, txtCountDown;

        public void OnWinScreenEnable()
        {
            if (winScrrenShow) AudioScript.Instant.PlayDrumSound();
            if (ImportantDataMembers.Instance.isLoggedIn)
            {
                connectedImg.SetActive(true);
                disConnectedImg.SetActive(false);
            }
            else
            {
                connectedImg.SetActive(false);
                disConnectedImg.SetActive(true);
            }
        }

        public bool WinScreenShow
        {
            get
            {
                return winScrrenShow;
            }
            set
            {
                winScrrenShow = value;
                winScreenDialog.SetActive(winScrrenShow);
                if (winScrrenShow && txtCountDown)
                {
                    txtCountDown.text = "" + 5;
                }
                OnWinScreenEnable();
                if (winScreenDialog.GetComponent<Animation>())
                {
                    Debug.Log("OKKOOK");
                    //winScreenDialog.GetComponent<Animation>().Play();
                }
            }
        }
    }


    /// <summary>
    /// here will store all UI panels.Objects of this call declaires in UIManager script
    /// </summary>
    [System.Serializable]
    public class Panels
    {
        public GameObject panelObject;
        public ALL_UI_SCREENS uiScreenName;
        public Animation animation;
        public AnimationClip clip_show, clip_hide;
    }

}
