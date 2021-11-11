using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using Assets.SimpleLocalization;

namespace TeamGame
{
    public class LoginScript : MonoBehaviour
    {
        /// <summary>
        /// here will code for login player and get player data and save it in our variables
        /// SendLoginQuery() method will send query to serve for login player.
        /// SetUserData() it will set player data if player login success.
        /// </summary>

        [SerializeField] private InputField inp_email, inp_pass;
        //[SerializeField] private GameObject LoadingScreen;
        [SerializeField] private GameObject statusPanel;
        [SerializeField] private Text txtStatusInfo;


        private void OnEnable()
        {
            txtStatusInfo.text = "";
            statusPanel.SetActive(false);
            //LoadingScreen.SetActive(false);
            inp_email.text = "";
            inp_pass.text = "";
        }

        private void SetLoginStatusPanel(string msg = "")
        {
            statusPanel.SetActive(true);
            txtStatusInfo.text = msg;


            Text txt = txtStatusInfo;

            if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
            {
                txt.font = Example.INSTANCE.mChineseSimplyfied;
            }
            else
            {
                txt.font = Example.INSTANCE.mEnglishTextMultiBold;
            }

            string localizationKEY = "";
            switch (msg)
            {
                case "Please fillup all field":

                    localizationKEY = "DYNAMIC.FILLUPFIELD";
                    break;

                case "Check your internet connection.":

                    localizationKEY = "DYNAMIC.CHECKINTERNET";
                    break;

                case "Server did not response.":

                    localizationKEY = "DYNAMIC.SERVERDIDNOTRESPONSE";
                    break;

                case "Something went to wrong.":

                    localizationKEY = "DYNAMIC.SOMETHINGWRONG";
                    break;

                default:
                    localizationKEY = "DYNAMIC.SOMETHINGWRONG";
                    break;
            }
            txt.text = LocalizationManager.Localize(localizationKEY);


        }

        public void onClickStatusBackButoon()
        {
            statusPanel.SetActive(false);
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
        }

        public void OnClickReset()
        {
            inp_email.text = "";
            inp_pass.text = "";
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
        }

        public void OnclickBackButton()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.Log_reg_Home);
        }

        private bool IsDetailsCorrect()
        {
            if (inp_email.text == "" || inp_pass.text == "")
            {
                SetLoginStatusPanel("Please fillup all field");
                return false;
            }
            else return true;
        }

        public void OnClickLogin()
        {
            if (IsDetailsCorrect())
            {
                if (!StaticDataHandler.IsInternetAccessUnavailable()) //check internet is ON
                {
                    StartCoroutine(SendLoginQuery());
                    UIManager.instance.LoadingScreen.SetActive(true);
                }
                else
                {
                    SetLoginStatusPanel("Check your internet connection.");
                }
            }
        }


        private IEnumerator SendLoginQuery()
        {
            JSONNode jsonData = new JSONObject
            {
                ["userName"] = inp_email.text,
                ["password"] = inp_pass.text
            };

            Debug.Log("Json To Fire : " + jsonData.ToString());
            string url = StaticDataHandler.URL_LOGIN;

            UnityWebRequest result = UnityWebRequest.Put(url, jsonData.ToString());
            result.method = UnityWebRequest.kHttpVerbPOST;
            result.SetRequestHeader("Content-Type", "application/json");
            result.SetRequestHeader("Accept", "application/json");

            yield return result.SendWebRequest();
            UIManager.instance.LoadingScreen.SetActive(false);

            JSONNode results = JSONNode.Parse(result.downloadHandler.text);
            string status = results["status"];
            string message = results["message"];

            Debug.Log("Result : " + result.downloadHandler.text);

            if (result.responseCode == 0)
            {
                SetLoginStatusPanel("Server did not respond.");
                Debug.Log("Server did not respond.");
            }
            else if (result.isNetworkError || result.isHttpError)
            {
                SetLoginStatusPanel(message);
                Debug.Log("Something went to wrong.. : " + message);
            }
            else
            {
                if (status == "1")
                {
                    Debug.Log("Login success");
                    SetUserData(result.downloadHandler.text);
                    UIManager.instance.ShowScreen(ALL_UI_SCREENS.Welcome);
                }
                else
                {
                    SetLoginStatusPanel("Login error Please try again");
                    Debug.Log("Login error Please try again");
                }
            }
        }

        private void SetUserData(string response)
        {
            JSONNode jsonNode = JSON.Parse(response);

            string status = jsonNode["status"];
            string playerId = jsonNode["userData"]["data"]["playerId"];
            string userName = jsonNode["userData"]["data"]["userName"];
            string playerChips = jsonNode["userData"]["data"]["chips"];
            string profilePic = jsonNode["userData"]["data"]["profilePicture"];
            string playerName = jsonNode["userData"]["data"]["displayName"];
            string language = jsonNode["userData"]["data"]["language"];

            StaticDataHandler.IsLoggedIn = true;
            StaticDataHandler.MyPlayerID = playerId;
            StaticDataHandler.MyUserName = userName;
            StaticDataHandler.MyPlayerName = playerName;
            StaticDataHandler.MyProfilePic = int.Parse(profilePic);
            StaticDataHandler.MyChips = float.Parse(playerChips);
            StaticDataHandler.Language = int.Parse(language);

            UIManager.instance.logoutButton.SetActive(true);
        }

        private TMP_InputField currentSelectedField;
        public void SelectInputField(TMP_InputField inputField)
        {

        }

        public void SetText(string text)
        {
            currentSelectedField.text = text;
        }

    }
}
