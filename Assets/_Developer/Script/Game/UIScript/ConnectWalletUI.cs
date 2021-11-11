using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamGame
{
    public class ConnectWalletUI : MonoBehaviour
    {

        /// <summary>
        /// used to connect wallet
        /// </summary>

        public void OnClickMetaMask()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void OnClickTrustWallet()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            // UIManager.instance.ShowScreen(ENM_UI_SCREENS.HomeScreen);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void OnClickWalletConnect()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.HomeScreen);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void OnClickMathWalllet()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.HomeScreen);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void OnClickBinanceChina()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.HomeScreen);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void onClickCloseButton()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }

            UIManager.instance.ShowScreen(ALL_UI_SCREENS.Welcome);
        }

    }
}