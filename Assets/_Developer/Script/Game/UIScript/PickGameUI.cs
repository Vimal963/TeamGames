using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace TeamGame
{
    public class PickGameUI : MonoBehaviour
    {
        /// <summary>
        /// it will use to pick a game 
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

        public void OnClickGame(int gameIndex)
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            if (gameIndex == 0)
            {
                GAMES game = (GAMES)Random.Range(1, 3);
                ImportantDataMembers.Instance.currentSelectedGame = game;
            }
            else
            {
                ImportantDataMembers.Instance.currentSelectedGame = (GAMES)gameIndex;
            }

            Debug.Log("Your Selcted gasme is :" + ImportantDataMembers.Instance.currentSelectedGame.ToString());

            if (ImportantDataMembers.Instance.currentPickedTeam == TEAM.Practice)
            {
                switch (ImportantDataMembers.Instance.currentSelectedGame)
                {
                    case GAMES.Thousand:
                        SceneManager.LoadScene("Prac_StopTheClock");
                        break;
                    case GAMES.Blink:
                        SceneManager.LoadScene("Prac_Cat Blink");
                        break;
                    default:
                        SceneManager.LoadScene("Prac_StopTheClock");
                        break;
                }
            }
            else
            {
                UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickStack);
            }

            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickGame;// UIManager.instance.currentScreen;
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

            if (ImportantDataMembers.Instance.PreviousScreen == ALL_UI_SCREENS.PickTeam)
            {
                UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickTeam);
            }
            else
            {
                UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickBettingStyle);
            }

        }
    }
}
