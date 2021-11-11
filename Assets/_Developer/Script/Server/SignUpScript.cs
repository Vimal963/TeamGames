using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleJSON;
using UnityEngine.UI;
using UnityEngine.Networking;
using TeamGame;
using Assets.SimpleLocalization;

public class SignUpScript : MonoBehaviour
{

    /// <summary>
    /// here will code for Register player and get player data and save it in our variables
    /// SendSighUpQuery() method will send query to serve for login player.
    /// SetUserData() it will set player data if player Registration success.
    /// </summary>



    [SerializeField] private InputField inp_email, inp_pass;
    // public TMP_InputField inp_re_pass;
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


    private void SetRegStatusPanel(string msg = "")
    {
        statusPanel.SetActive(true);
        txtStatusInfo.text = msg;


        //this is code for manage Language Localization.

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

            case "User Register successfully":

                localizationKEY = "DYNAMIC.REGSUC";
                break;

            case "Error, Please try again.!":

                localizationKEY = "DYNAMIC.ERROR";
                break;

            case "User Already Register":

                localizationKEY = "DYNAMIC.ALRDYREG";
                break;

            case "Server did not response.":

                localizationKEY = "DYNAMIC.SERVERDIDNOTRESPONSE";
                break;

            case "Something went to wrong.":

                localizationKEY = "DYNAMIC.SOMETHINGWRONG";
                break;

            default:
                localizationKEY = "DYNAMIC.ERROR";
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


    public void OnclickBackButton()
    {
        if (AudioScript.Instant)
        {
            AudioScript.Instant.PlayBtnClickSound();
        }
        UIManager.instance.ShowScreen(ALL_UI_SCREENS.Log_reg_Home);
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
    /// <summary>
    /// check entered data is correct or not
    /// </summary>
    /// <returns></returns>
    private bool IsDetailsCorrect()
    {
        if (inp_email.text == "" || inp_pass.text == ""/* || inp_pName.text == "" || inp_re_pass.text == ""*/)
        {
            SetRegStatusPanel("Please fillup all field");
            return false;
        }
        else return true;

    }

    public void OnClickSignUp()
    {
        if (AudioScript.Instant)
        {
            AudioScript.Instant.PlayBtnClickSound();
        }
        if (IsDetailsCorrect())
        {
            if (!StaticDataHandler.IsInternetAccessUnavailable()) // NetworkReachability.NotReachable != 0)
            {
                StartCoroutine(SendSighUpQuery());
                UIManager.instance.LoadingScreen.SetActive(true);
            }
            else
            {
                SetRegStatusPanel("Check your internet connection.");
            }
        }
    }

    private IEnumerator SendSighUpQuery()
    {
        JSONNode jsonData = new JSONObject
        {
            ["userName"] = inp_email.text,
            ["password"] = inp_pass.text
        };

        Debug.Log("Json To be Passed : " + jsonData.ToString());
        Debug.Log("API Url : " + StaticDataHandler.URL_SIGNUP);
        string url = StaticDataHandler.URL_SIGNUP;

        UnityWebRequest result = UnityWebRequest.Put(url, jsonData.ToString());
        result.method = UnityWebRequest.kHttpVerbPOST;
        result.SetRequestHeader("Content-Type", "application/json");
        result.SetRequestHeader("Accept", "application/json");

        yield return result.SendWebRequest();
        UIManager.instance.LoadingScreen.SetActive(false);

        Debug.Log("Res : " + result.downloadHandler.text);

        JSONNode results = JSONNode.Parse(result.downloadHandler.text);

        string status = results["status"];
        string msg = results["message"];

        if (result.responseCode == 0)
        {
            SetRegStatusPanel(msg);

        }
        else if (result.isNetworkError || result.isHttpError)
        {
            if (string.IsNullOrEmpty(msg))//  (msg = "")
            {
                SetRegStatusPanel("Something went to wrong.");
            }
            else
            {
                SetRegStatusPanel(msg);
            }

        }
        else
        {
            if (status == "1")
            {
                Debug.Log("SignUp success");
                SetUserData(result.downloadHandler.text);
                UIManager.instance.ShowScreen(ALL_UI_SCREENS.Welcome);
            }
            else if (status == "0" || status == "2")
            {
                SetRegStatusPanel(msg);
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

        //ImportantDataMembers._instance.isLoggedIn = true;
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
