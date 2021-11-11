using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace TeamGame
{


    /// <summary>
    /// this the script for blink game (scrady game) Practice Only.
    /// here ShowCountDown() used to show count down like your game start in 3,2,1 go,
    /// ShowTimer() use to show Timer of you game. once your timer is zero you game will over.and win screen will open.
    /// in game you have to jus mouse down to blink car method used to do this that is BlinkEye()
    /// </summary>
    public class BlinkGame_Practice : MonoBehaviour
    {
        [SerializeField] private WinScreen winScreen;
        [SerializeField] private Image cat;
        [SerializeField] private Sprite blink_off, blink_on;
        [SerializeField] private GameObject msg;
        [SerializeField] private Text txtTimer, numberTxt;

        [SerializeField] private Transform screenMiddlePoint;
        [SerializeField] private Sprite[] countDownSprits;
        [SerializeField] private GameObject coundownPrefab;

        private int randomNumber;
        private float score, timerInSec;

        private void Start()
        {
            randomNumber = Random.Range(90, 120);
            AudioScript.Instant.StopBGMusic();
            StartCoroutine(ShowCountDown());
        }

        private IEnumerator ShowCountDown()
        {
            msg.SetActive(true);
            msg.GetComponentInChildren<Text>().text = "Scare the cat " + randomNumber + " times";
            numberTxt.text = "Scare the cat " + randomNumber + " times";

            yield return new WaitForSeconds(2);
            float start = 0.7f, target = 0;
            msg.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, 850, 0), 1).SetEase(Ease.Linear);
            msg.transform.GetChild(0).GetComponent<RectTransform>().DOScale(new Vector3(0.7f, 0.7f, 0.7f), 1).SetEase(Ease.Linear);
            DOTween.To(() => start, x => start = x, target, 3)
                .OnUpdate(() => { msg.GetComponent<Image>().color = new Color(0, 0, 0, start); })
                .OnComplete(() => { msg.SetActive(false); numberTxt.gameObject.SetActive(true); });

            for (int countdown = 3; countdown >= 0; countdown--)
            {
                GameObject obj = Instantiate(coundownPrefab);
                obj.transform.SetParent(screenMiddlePoint, false);
                obj.GetComponent<Image>().sprite = countDownSprits[countdown];
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one * 0.6f;
                obj.transform.DOScale(countdown == 0 ? Vector3.one * 4 : Vector3.one * 1.5f, countdown == 0 ? 0.3f : 0.8f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(obj));
                yield return new WaitForEndOfFrame();
                if (AudioScript.Instant) AudioScript.Instant.PlayCountDownSound();
                yield return new WaitForSeconds(countdown == 0 ? 0.3f : 0.8f);
            }

            ResetGame();
        }

        private void ResetGame()
        {
            timerInSec = 20f;
            txtTimer.text = StaticDataHandler.FormatTime(timerInSec);
            score = 0;
            winScreen.txtScore.text = "" + score.ToString();
            InvokeRepeating(nameof(ShowTimer), 1, 1);
        }

        private void ShowTimer()
        {
            if (timerInSec > 0)
            {
                timerInSec--;
                txtTimer.text = StaticDataHandler.FormatTime(timerInSec);
            }
            else
            {
                OnGameOVer();
                CancelInvoke(nameof(ShowTimer));
            }

        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    OnClickBlinkButton();
                }
            }
        }


        public void OnClickBlinkButton()
        {
            if (timerInSec > 0)
            {
                StartCoroutine(BlinkEye());
                score++;

                if (AudioScript.Instant) AudioScript.Instant.PlayBlinkSound();

            }
        }

        private IEnumerator BlinkEye()
        {
            cat.sprite = blink_off;
            yield return new WaitForSeconds(0.1f);
            cat.sprite = blink_on;
        }

        private void OnGameOVer()
        {
            winScreen.WinScreenShow = true;
            winScreen.txtScore.text = "" + score.ToString();
            winScreen.winScreenDialog.GetComponent<Animation>().Play();
        }

        public void OnClickHome()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
            StopCoroutine("ShowCountDown");
            StaticDataHandler.LoadHomeScene();
        }

        public void OnClickPlayAgain()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}