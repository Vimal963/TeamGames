using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamGame
{
    public class PickBetingStyleUI : MonoBehaviour
    {

        /// <summary>
        /// this is used to Pick Betting style like Singlebet,POker rule or Tournament
        /// </summary>

        [SerializeField] private GameObject connectedImg, disConnectedImg;

        private void OnEnable()
        {
            connectedImg.SetActive(ImportantDataMembers.Instance.isLoggedIn);
            disConnectedImg.SetActive(!ImportantDataMembers.Instance.isLoggedIn);
        }

        public void OnClickSingleBet()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            ImportantDataMembers.Instance.currentBettingStyle = BETTING_STYLE.SingleBet;
            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickBettingStyle;
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickGame);
        }

        public void OnClickePokerRules()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            ImportantDataMembers.Instance.currentBettingStyle = BETTING_STYLE.PokerRule;

            GAMES game = (GAMES)Random.Range(1, 3);
            ImportantDataMembers.Instance.currentSelectedGame = game;

            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickBettingStyle;
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickStack);
        }

        public void OnClickTournament()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            ImportantDataMembers.Instance.currentBettingStyle = BETTING_STYLE.Tournament;
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.Tournament);
        }

        public void OnClickBack(int val = 0)
        {
            if (val == 0)
            {
                if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
            }

            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickTeam);

        }

    }
}