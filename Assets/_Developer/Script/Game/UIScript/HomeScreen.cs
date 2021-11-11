using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamGame
{
    /// <summary>
    /// it's home screen UI all method use to perform some task.
    /// </summary>

    public class HomeScreen : MonoBehaviour
    {
        [SerializeField] private GameObject connectedImg, disConnectedImg;

        private void OnEnable()
        {
            connectedImg.SetActive(ImportantDataMembers.Instance.isLoggedIn);
            disConnectedImg.SetActive(!ImportantDataMembers.Instance.isLoggedIn);
        }

        public void OnClickBack()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            //  UIManager.instance.ShowScreen(ENM_UI_SCREENS.ConnectWallet);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.ConnectWallet);
        }

        public void OnClikPlayNow()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            //    UIManager.instance.ShowScreen(ENM_UI_SCREENS.PickTeam);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickTeam);
        }

        public void OnClickBuyTMGToken()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
        }

        public void OnClickProfile()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PlayerProfile);
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
