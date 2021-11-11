using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamGame
{
    public class PickTeamUI : MonoBehaviour
    {

        /// <summary>
        /// her will pich a team like USA,China 
        /// </summary>

        [SerializeField] private GameObject connectedImg, disConnectedImg;

        private void OnEnable()
        {
            connectedImg.SetActive(ImportantDataMembers.Instance.isLoggedIn);
            disConnectedImg.SetActive(!ImportantDataMembers.Instance.isLoggedIn);
        }

        void Start()
        {

        }

        public void OnClickUsaTeam()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            ImportantDataMembers.Instance.currentPickedTeam = TEAM.Usa;
            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.PickBettingStyle);
            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickTeam;// UIManager.instance.currentScreen;
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickBettingStyle);
        }

        public void OnClikChinaTeam()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            ImportantDataMembers.Instance.currentPickedTeam = TEAM.China;
            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.PickBettingStyle);
            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickTeam;// UIManager.instance.currentScreen;
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickBettingStyle);
        }

        public void OnClickPractice()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            ImportantDataMembers.Instance.currentPickedTeam = TEAM.Practice;
            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.PickGame);
            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickTeam;// UIManager.instance.currentScreen;
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickGame);
        }

        public void OnClickBack()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void OnClickMenu()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            Debug.Log("On Click Menu");
        }
    }
}
