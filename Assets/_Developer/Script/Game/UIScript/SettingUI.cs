using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TeamGame
{
    public class SettingUI : MonoBehaviour
    {
        [SerializeField] private Transform music, sound, close;
        private GameObject S_on, S_off, M_on, M_off;

        // Start is called before the first frame update

        private void OnEnable()
        {
            S_on = sound.GetChild(0).gameObject;
            S_off = sound.GetChild(1).gameObject;

            M_on = music.GetChild(0).gameObject;
            M_off = music.GetChild(1).gameObject;


            S_on.GetComponent<Button>().onClick.AddListener(() => { OnClickSound(); });
            S_off.GetComponent<Button>().onClick.AddListener(() => { OnClickSound(); });

            M_on.GetComponent<Button>().onClick.AddListener(() => { OnClickMusic(); });
            M_off.GetComponent<Button>().onClick.AddListener(() => { OnClickMusic(); });

            close.GetComponent<Button>().onClick.AddListener(() => { onClickClose(false); });

            setButtons();
        }

        private void setButtons()
        {
            if (PlayerPrefs.GetInt(AudioScript.soundKey) == 1)
            {
                AudioScript.Instant.soundAS.volume = 1;
                S_on.SetActive(true);
                S_off.SetActive(false);
            }
            else
            {

                AudioScript.Instant.soundAS.volume = 0;
                S_on.SetActive(false);
                S_off.SetActive(true);
            }

            if (PlayerPrefs.GetInt(AudioScript.musicKey) == 1)
            {

                AudioScript.Instant.musicAS.volume = 1;
                M_on.SetActive(true);
                M_off.SetActive(false);
            }
            else
            {
                AudioScript.Instant.musicAS.volume = 0;
                M_on.SetActive(false);
                M_off.SetActive(true);
            }

        }


        public void OnClickSound()
        {
            if (PlayerPrefs.GetInt(AudioScript.soundKey) == 0)
            {
                PlayerPrefs.SetInt(AudioScript.soundKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(AudioScript.soundKey, 0);
            }
            setButtons();
            AudioScript.Instant.PlayBtnClickSound();
        }

        public void OnClickMusic()
        {
            if (PlayerPrefs.GetInt(AudioScript.musicKey) == 0)
            {
                PlayerPrefs.SetInt(AudioScript.musicKey, 1);
            }
            else
            {
                PlayerPrefs.SetInt(AudioScript.musicKey, 0);
            }
            setButtons();
            AudioScript.Instant.PlayBGMusic();
        }

        public void onClickClose(bool wantToDestroyOnClose)
        {
            Time.timeScale = 1;
            AudioScript.Instant.PlayBtnClickSound();
            if (wantToDestroyOnClose)
            {
                Destroy(gameObject);
            }
            else
                gameObject.SetActive(false);
        }
    }
}
