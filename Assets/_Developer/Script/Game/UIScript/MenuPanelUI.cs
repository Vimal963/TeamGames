using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamGame
{
    public class MenuPanelUI : MonoBehaviour
    {
        /// <summary>
        /// it's Menu panel screen UI all method use to perform some task.
        /// </summary>
        void Start()
        {

        }

        public void onClickCloseButton()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            gameObject.SetActive(false);
        }

        public void OnClickBuyTMGToken()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
        }

        public void OnClickLeaderBoard()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.LeaderBoard);
        }

        public void OnClickTopUpGameWallet()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
        }

        public void OnClickDisconnectmainWallet()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
        }
    }
}
