using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamGame
{
    public class WelComeUI : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnClickConnectWallet()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            Debug.Log("Connnect Wallet");
            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.ConnectWallet);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.ConnectWallet);
        }
    }
}
