using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System.Linq;
using DG.Tweening;
using Assets.SimpleLocalization;

namespace TeamGame
{

    /// <summary>
    /// This script used only for ONLINE BLINK GAME(Scrady Game)
    /// all are the things here releted Blink gameplay.
    /// here we registering some socket event like game start count down,winscore,gameplay timer etc,
    /// Method OnReceivedCountDown(object[] args) used to get and show game starts in count down
    /// Method  OnReceivedBlinkGameTimer(object[] args) used to get and show gmae timer
    /// Method OnReceivedBlinkPlayerScore(object[] args) used to get score from server once you complete game.
    /// Method BackCountDown() used to move player to bet screen automaticaly from win screen.
    /// </summary>
    public class BlinkGame_Online : MonoBehaviour
    {

        public WinScreen winScreen;

        [SerializeField] private GameObject statusScreen;
        [SerializeField] private Image cat;
        [SerializeField] private Sprite blink_off, blink_on;
        [SerializeField] private GameObject msg;
        [SerializeField] private Text txtTimer, numberTxt;

        [SerializeField] private Transform screenMiddlePoint;
        [SerializeField] private Sprite[] countDownSprits;
        [SerializeField] private GameObject coundownPrefab;

        private float score, timerInSec;
        private bool isPlayerCanTap;

        private void Start()
        {
            AudioScript.Instant.StopBGMusic();
            ImportantDataMembers.Instance.currentScene = SCENE.Cat;
            ShowStatusScreen(true, "Waiting for other players");
            txtTimer.text = StaticDataHandler.FormatTime(20);

            Network_Emitter.Instance.Emit_StartCountDown();
        }

        private void OnEnable()
        {
            Network_Listener.Instance.onReceinveBlinkCountDown_ACN += OnReceivedCountDown;
            Network_Listener.Instance.onReceivedBlinkTimer_ACN += OnReceivedBlinkGameTimer;
            Network_Listener.Instance.onReceivedBlinkPLayerScore_ACN += OnReceivedBlinkPlayerScore;
        }

        private void OnDisable()
        {
            Network_Listener.Instance.onReceinveBlinkCountDown_ACN -= OnReceivedCountDown;
            Network_Listener.Instance.onReceivedBlinkTimer_ACN -= OnReceivedBlinkGameTimer;
            Network_Listener.Instance.onReceivedBlinkPLayerScore_ACN -= OnReceivedBlinkPlayerScore;
        }

        private void ShowStatusScreen(bool show, string info = "")
        {
            statusScreen.SetActive(show);
            statusScreen.GetComponentInChildren<Text>().text = info;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isPlayerCanTap)
                {
                    if (timerInSec >= 0)
                    {
                        StartCoroutine(BlinkEye());
                        score++;
                        //txtScore.text = "Tap : " + score;

                        if (AudioScript.Instant) AudioScript.Instant.PlayBlinkSound();
                    }
                }
            }
        }


        IEnumerator BlinkEye()
        {
            cat.sprite = blink_off;
            yield return new WaitForSeconds(0.1f);
            cat.sprite = blink_on;
        }

        public void OnClickHomeButton()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            Network_Emitter.Instance.Emit_OnPlayerLeft();
            StaticDataHandler.LoadHomeScene();
        }

        GameObject coundDown;
        int count;
        private void OnReceivedCountDown(object[] args)
        {
            count += 1;
            ShowStatusScreen(false);
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());

            if (count == 1)
            {
                int randomNumber = int.Parse(jsonString["randomNumber"].Value);
                msg.SetActive(true);
                //  msg.GetComponentInChildren<Text>().text = "Scare the cat " + randomNumber + " times";
                //  numberTxt.text = "Scare the cat " + randomNumber + " times";



                Text txt = msg.GetComponentInChildren<Text>();

                string localizationKEY = "DYNAMIC.SCARETHECAT";
                string localizationKEY1 = "DYNAMIC.TIMES";
                if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
                {
                    txt.font = Example.INSTANCE.mChineseSimplyfied;
                }
                else
                {
                    txt.font = Example.INSTANCE.mEnglishTextMultiBold;
                }

                txt.text = LocalizationManager.Localize(localizationKEY) + " " + randomNumber.ToString() + " " + LocalizationManager.Localize(localizationKEY1);



            }

            if (count >= 2)
            {
                if (count == 2)
                {
                    float start = 0.7f, target = 0;
                    msg.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, 850, 0), 1).SetEase(Ease.Linear);
                    msg.transform.GetChild(0).GetComponent<RectTransform>().DOScale(new Vector3(0.7f, 0.7f, 0.7f), 1).SetEase(Ease.Linear);
                    DOTween.To(() => start, x => start = x, target, 3)
                        .OnUpdate(() => { msg.GetComponent<Image>().color = new Color(0, 0, 0, start); })
                        .OnComplete(() => { msg.SetActive(false); numberTxt.gameObject.SetActive(true); });
                }

                int countdown = int.Parse(jsonString["timer"].Value);
                if (coundDown != null) Destroy(coundDown);
                GameObject obj = Instantiate(coundownPrefab);
                coundDown = obj;
                obj.transform.SetParent(screenMiddlePoint, false);
                obj.GetComponent<Image>().sprite = countDownSprits[countdown];
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one * 0.6f;
                obj.transform.DOScale(countdown == 0 ? Vector3.one * 4 : Vector3.one * 1.5f, countdown == 0 ? 0.3f : 0.8f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => Destroy(obj));

                if (AudioScript.Instant) AudioScript.Instant.PlayCountDownSound();
            }
        }

        private void OnReceivedBlinkGameTimer(object[] args)
        {
            ShowStatusScreen(false);
            isPlayerCanTap = true;
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());

            string number = jsonString["timer"].ToString();
            timerInSec = float.Parse(number);

            txtTimer.text = StaticDataHandler.FormatTime(timerInSec);
            if (timerInSec == 0)
            {
                Debug.Log("Submit Score : " + score.ToString());
                score += Random.Range(0, 1); // just for testing, will remove for live
                Network_Emitter.Instance.Emit_BlinkScore(score.ToString());
                isPlayerCanTap = false;
            }
        }

        private void OnReceivedBlinkPlayerScore(object[] args)
        {
            ShowStatusScreen(false);
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());
            Debug.Log("winner player json : " + jsonString.ToString());

            //string playerId = jsonString["winnerPlayerId"]["playerId"];
            //string playerScore = jsonString["winnerPlayerId"]["score"];
            winScreen.txtScore.text = "" + score;
            winScreen.WinScreenShow = true;
            winScreen.winScreenDialog.GetComponent<Animation>().Play();
            StaticDataHandler.inRound = true;
            isPlayerCanTap = false;

            //if (AudioScript._instant) AudioScript._instant.PlayWinSound();
            StartCoroutine(BackCountDown());
        }


        IEnumerator BackCountDown()
        {
            for (int i = 5; i >= 0; i--)
            {
                if (i == 0) StaticDataHandler.LoadBetScene();

                winScreen.txtCountDown.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        public void GoBackToBetScreen()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
            StaticDataHandler.LoadBetScene();
        }
    }
}