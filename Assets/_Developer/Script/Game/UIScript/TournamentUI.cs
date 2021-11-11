using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TeamGame
{
    public class TournamentUI : MonoBehaviour
    {
        //public TMP_Text txtRemainingtime, NextTournamentTime;
        //[SerializeField] private Text remainTimeTxt, nextGameTimeTxt, upcomingTimeTxt;

        // Start is called before the first frame update
        void Start()
        {

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
            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.PickBettingStyle);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickBettingStyle);

        }
    }
}