using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace TeamGame
{
    public class ThousandGamePlay_Practice : MonoBehaviour
    {
        /// <summary>
        /// this the script for thousand game (stop the clock) Practice Only.
        /// here ShowCountDown() used to show count down like your game start in 3,2,1 go,
        /// </summary>
        public WinScreen winScreen;
        [SerializeField] private GameObject msg;
        [SerializeField] private Text txtCounter, numberTxt;

        private bool isGameStart, isGameOver;
        [SerializeField] private Transform screenMiddlePoint;
        [SerializeField] private Sprite[] countDownSprits;
        [SerializeField] private GameObject coundownPrefab;

        private int randomNumber;
        private IEnumerator counterIenumerator;

        private void Start()
        {
            randomNumber = Random.Range(800, 1000);
            AudioScript.Instant.StopBGMusic();
            StartCoroutine("ShowCountDown");
        }

        private IEnumerator ShowCountDown()
        {
            msg.SetActive(true);
            msg.GetComponentInChildren<Text>().text = "Stop the clock at " + randomNumber;
            numberTxt.text = "Stop the clock at " + randomNumber;

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

            OnClickStart();
        }

        private void OnClickStart()
        {
            counterIenumerator = NumberCounter();
            StartCoroutine(counterIenumerator);
        }


        /// <summary>
        /// method will star you clock timer.you have to stop this clock by clicking on the screen.
        /// </summary>
        /// <returns></returns>
        private IEnumerator NumberCounter()
        {
            isGameOver = false;
            isGameStart = true;
            float elapsedTime = 0;
            float start = 0;
            float end = 50000;
            float time = 500;
            float ans;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                if (!isGameOver)
                {
                    ans = Mathf.Lerp(start, end, (elapsedTime / time));
                    string roundUP = ans.ToString().Split('.').GetValue(0).ToString();

                    txtCounter.text = "" + roundUP;
                }
                else
                {
                    StopCoroutine(counterIenumerator);
                }
                yield return null;
            }
        }

        /// <summary>
        /// code use to stop the clock when you will click on screen you clock timer will stop.and winn screen will appear with you stop[ed clock timer.
        /// </summary>
        float timer;
        private void Update()
        {
            if (isGameOver) return;

            if (isGameStart)
            {
                timer += Time.deltaTime;
                if (timer >= 15)
                {
                    isGameOver = true;
                    isGameStart = false;

                    StopCoroutine(counterIenumerator);
                    winScreen.WinScreenShow = true;
                    winScreen.txtScore.text = "0";
                    winScreen.winScreenDialog.GetComponent<Animation>().Play();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.currentSelectedGameObject != null) return;
                    isGameOver = true;
                    isGameStart = false;
                    Stop();
                }
            }
        }

        public void Stop()
        {
            StopCoroutine(counterIenumerator);
            winScreen.WinScreenShow = true;
            winScreen.txtScore.text = "" + txtCounter.text;
            winScreen.winScreenDialog.GetComponent<Animation>().Play();
        }

        public void OnClickHomeButtn()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
            StopCoroutine("ShowCountDown");
            StaticDataHandler.LoadHomeScene();
        }

        public void OnClickPlayAgaing()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
