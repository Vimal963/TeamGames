using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamGame
{
    public class Login_Reg_HomeScren : MonoBehaviour
    {

        /// <summary>
        /// it will show screen as player hit button login or register.
        /// </summary>

        public void OnLClickLogin()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.login);
        }

        public void OnClickReg()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.register);
        }
    }


}