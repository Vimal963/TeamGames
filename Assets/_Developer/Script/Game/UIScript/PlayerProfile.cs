using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace TeamGame
{
    public class PlayerProfile : MonoBehaviour
    {

        /// <summary>
        /// it is a player profile will manage player name,profile pic,sound setting.language localization selection also in here done here
        /// UpdateUSerData() use to update player data if player had chage something in his.her data
        /// </summary>

        [SerializeField] private GameObject ProfilePicturePanel;
        [SerializeField] private GameObject connectedImg, disConnectedImg;
        [SerializeField] private Image profilePic;
        [SerializeField] private InputField userNameInputField, dispNameInputField;
        [SerializeField] private GameObject btnENG, btnCHINA;
        [SerializeField] private GameObject soundOff, soundOn;

        private void OnEnable()
        {
            ProfilePicturePanel.SetActive(false);
            connectedImg.SetActive(ImportantDataMembers.Instance.isLoggedIn);
            disConnectedImg.SetActive(!ImportantDataMembers.Instance.isLoggedIn);

            profilePic.sprite = ImportantDataMembers.Instance.ProfilePics[StaticDataHandler.MyProfilePic];
            userNameInputField.text = StaticDataHandler.MyUserName;
            dispNameInputField.text = StaticDataHandler.MyPlayerName;
            ImportantDataMembers.Instance.currentLanguage = (LANGUAGE)StaticDataHandler.Language;

            SetLanguageBtns();
            SetSoundBtns();
        }

        private void SetLanguageBtns()
        {
            if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.English)
            {
                btnENG.SetActive(true);
                btnCHINA.SetActive(false);
            }
            else
            {
                btnENG.SetActive(false);
                btnCHINA.SetActive(true);
            }

        }


        private void SetSoundBtns()
        {
            if (PlayerPrefs.GetInt(AudioScript.soundKey) == 1)
            {
                AudioScript.Instant.soundAS.volume = 1;
                AudioScript.Instant.musicAS.volume = 1;
                AudioScript.Instant.drumAS.volume = 1;

                soundOn.SetActive(true);
                soundOff.SetActive(false);
            }
            else
            {
                AudioScript.Instant.soundAS.volume = 0;
                AudioScript.Instant.musicAS.volume = 0;
                AudioScript.Instant.drumAS.volume = 0;

                soundOn.SetActive(false);
                soundOff.SetActive(true);
            }
        }

        public void OnCLickLanguage(int val = 0)
        {
            if (val == 0)
            {
                if (AudioScript.Instant)
                {
                    AudioScript.Instant.PlayBtnClickSound();
                }
            }

            if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.English)
                ImportantDataMembers.Instance.currentLanguage = LANGUAGE.Chinese;
            else
                ImportantDataMembers.Instance.currentLanguage = LANGUAGE.English;
            StaticDataHandler.Language = (int)ImportantDataMembers.Instance.currentLanguage;

            Assets.SimpleLocalization.LocalizationManager.Language = ImportantDataMembers.Instance.currentLanguage.ToString();
            SetLanguageBtns();
        }

        public void OnClickSound()
        {
            if (PlayerPrefs.GetInt(AudioScript.soundKey) == 0) PlayerPrefs.SetInt(AudioScript.soundKey, 1);
            else PlayerPrefs.SetInt(AudioScript.soundKey, 0);

            SetSoundBtns();
            AudioScript.Instant.PlayBtnClickSound();
            AudioScript.Instant.PlayBGMusic();
        }

        public void OnClickBack(int val = 0)
        {
            if (val == 0)
            {
                if (AudioScript.Instant)
                {
                    AudioScript.Instant.PlayBtnClickSound();
                }
            }
            StartCoroutine(UpdateUSerData());
        }


        private IEnumerator UpdateUSerData()
        {
            JSONNode jsonData = new JSONObject
            {
                ["language"] = StaticDataHandler.Language.ToString(),
                ["displayName"] = dispNameInputField.text,
                ["profilePicture"] = StaticDataHandler.MyProfilePic.ToString(),
                ["playerId"] = StaticDataHandler.MyPlayerID
            };

            StaticDataHandler.MyPlayerName = dispNameInputField.text;
            string url = StaticDataHandler.URL_USERUPDATE;

            UnityWebRequest result = UnityWebRequest.Put(url, jsonData.ToString());
            result.method = UnityWebRequest.kHttpVerbPOST;
            result.SetRequestHeader("Content-Type", "application/json");
            result.SetRequestHeader("Accept", "application/json");
            yield return result.SendWebRequest();

            print("Profile Update Data ==> " + result.downloadHandler.text);
            JSONNode results = JSONNode.Parse(result.downloadHandler.text);

            string message = results["status"];
            if (message == "1")
            {
                Debug.Log("Profile Update Successfully");
            }
            else
            {
                Debug.Log("Profile Update Failed");
            }

            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }


        private TMP_InputField currentSelectedField;
        public void SelectInputField(TMP_InputField inputField)
        {

        }

        public void SetText(string text)
        {
            currentSelectedField.text = text;
        }

        public void SelectProfilePicture()
        {
            int i = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            StaticDataHandler.MyProfilePic = i;
            profilePic.sprite = ImportantDataMembers.Instance.ProfilePics[StaticDataHandler.MyProfilePic];
            ProfilePicturePanel.SetActive(false);
        }
    }
}
