using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamGame
{
    public class PickTeamErrorUI : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnClickBuy()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            Debug.Log("OnClickBUY");
            // UIManager.instance.ShowScreen(ENM_UI_SCREENS.ConnectWallet);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.ConnectWallet);
        }


        public void OnClickClose(int val = 0)
        {
            if (val == 0)
            {
                if (AudioScript.Instant)
                {
                    AudioScript.Instant.PlayBtnClickSound();
                }
            }
            Debug.Log("OnClickClose");
            //UIManager.instance.ShowScreen(ENM_UI_SCREENS.PickTeam);
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickTeam);
        }
    }
}
