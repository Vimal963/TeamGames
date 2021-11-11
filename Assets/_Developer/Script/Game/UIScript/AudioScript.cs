using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamGame
{
    public class AudioScript : MonoBehaviour
    {
        /// <summary>
        /// used to manage sound and music in game
        /// </summary>

        public static AudioScript Instant;
        public const string soundKey = "sound", musicKey = "music";

        public AudioSource musicAS, soundAS, drumAS;
        public AudioClip btnClick, winClip, blinkClip, countDownClip;


        private void Awake()
        {
            if (Instant == null)
            {
                Instant = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        void Start()
        {
            Instant = this;

            if (!PlayerPrefs.HasKey(soundKey))
            {
                PlayerPrefs.SetInt(soundKey, 1);
            }

            CheckMusicAndSound_On_Off();
        }

        private void CheckMusicAndSound_On_Off()
        {
            if (PlayerPrefs.GetInt(soundKey) == 0)
            {
                soundAS.volume = 0;
                musicAS.volume = 0;
                drumAS.volume = 0;
            }
            else
            {
                soundAS.volume = 1;
                musicAS.volume = 1;
                drumAS.volume = 1;
            }

            PlayBGMusic();
        }

        public void PlayBGMusic()
        {
            musicAS.Play();
        }

        public void StopBGMusic()
        {
            musicAS.Stop();
        }

        public void PlayDrumSound()
        {
            StartCoroutine(PlayDrumSound_1());
        }
        IEnumerator PlayDrumSound_1()
        {
            yield return new WaitForSeconds(0.2f);
            drumAS.Play();
        }

        public void PlayBtnClickSound()
        {
            soundAS.PlayOneShot(btnClick);
        }

        public void PlayWinSound()
        {
            soundAS.PlayOneShot(winClip);
        }

        public void PlayBlinkSound()
        {
            soundAS.PlayOneShot(blinkClip);
        }

        public void PlayCountDownSound()
        {
            soundAS.PlayOneShot(countDownClip);
        }
    }
}
