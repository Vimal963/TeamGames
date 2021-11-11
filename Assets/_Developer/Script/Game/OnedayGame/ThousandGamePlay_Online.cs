using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Linq;
using DG.Tweening;
using System.Collections;
using Assets.SimpleLocalization;

namespace TeamGame
{
    public class ThousandGamePlay_Online : MonoBehaviour
    {
        /// <summary>
        /// This script used only for ONLINE THOUSAND GAME(Stop The Clock)
        /// all are the things here releted THOUSAND gameplay.
        /// here we registering some socket event like game start count down,winscore,gameplay timer etc,
        /// Method OnReceivedCountDown(object[] args) used to get and show game starts in count down
        /// Method  OnReceivedThousandGameTimer(object[] args) used to get and show gmae timer
        /// Method OnReceivedThousandPlayerScore(object[] args) used to get score from server once you complete game.
        /// Method BackCountDown() used to move player to bet screen automaticaly from win screen.
        /// </summary>



        public WinScreen winScreen;

        [SerializeField] private GameObject statusScreen;
        [SerializeField] private GameObject msg;
        [SerializeField] private Text txtCounter, numberTxt;
        [SerializeField] private Transform screenMiddlePoint;
        [SerializeField] private Sprite[] countDownSprits;
        [SerializeField] private GameObject coundownPrefab;

        private string score;

        private bool isGameStart;
        private bool isGameOver;


        private void Start()
        {
            AudioScript.Instant.StopBGMusic();
            ImportantDataMembers.Instance.currentScene = SCENE.Clock;
            isGameStart = false;
            isGameOver = false;
            ShowStatusScreen(true, "Waiting for other players");

            Network_Emitter.Instance.Emit_StartCountDown();
        }

        private void OnEnable()
        {
            Network_Listener.Instance.onReceiveThousandCountDown_ACN += OnReceivedCountDown;
            Network_Listener.Instance.onReceivedThousandTimer_ACN += OnReceivedThousandGameTimer;
            Network_Listener.Instance.onReceivedThousandPlayerScore_ACN += OnReceivedThousandPlayerScore;
        }

        private void OnDisable()
        {
            Network_Listener.Instance.onReceiveThousandCountDown_ACN -= OnReceivedCountDown;
            Network_Listener.Instance.onReceivedThousandTimer_ACN -= OnReceivedThousandGameTimer;
            Network_Listener.Instance.onReceivedThousandPlayerScore_ACN -= OnReceivedThousandPlayerScore;
        }

        private void ShowStatusScreen(bool show, string info = "")
        {
            statusScreen.SetActive(show);
            statusScreen.GetComponentInChildren<Text>().text = info;

            Text txt = statusScreen.GetComponentInChildren<Text>();

            if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
            {
                txt.font = Example.INSTANCE.mChineseSimplyfied;
            }
            else
            {
                txt.font = Example.INSTANCE.mEnglishTextMultiBold;
            }

            string localizationKEY = "DYNAMIC.WAITINGFORPLAYER";
            switch (info)
            {
                case "Waiting for other players":
                    localizationKEY = "DYNAMIC.WAITINGFORPLAYER";
                    break;

                default:
                    break;
            }
            txt.text = LocalizationManager.Localize(localizationKEY);


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
                msg.GetComponentInChildren<Text>().text = "Stop the clock at " + randomNumber;
                //  numberTxt.text = "Stop the clock at " + randomNumber;






                Text txt = numberTxt;

                string localizationKEY = "DYNAMIC.STOPCLOCKAT";

                if (ImportantDataMembers.Instance.currentLanguage == LANGUAGE.Chinese)
                {
                    txt.font = Example.INSTANCE.mChineseSimplyfied;
                }
                else
                {
                    txt.font = Example.INSTANCE.mEnglishTextMultiBold;
                }

                txt.text = LocalizationManager.Localize(localizationKEY) + " " + randomNumber.ToString();




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

        private void OnReceivedThousandGameTimer(object[] args)
        {
            ShowStatusScreen(false);
            JSONNode jsonString = JSONNode.Parse(args[0].ToString());

            string number = jsonString["timer"];
            txtCounter.text = number;
            score = number;
            isGameStart = true;
        }

        private void OnReceivedThousandPlayerScore(object[] args)
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

            //if (AudioScript._instant) AudioScript._instant.PlayWinSound();
            StartCoroutine(BackCountDown());
        }

        IEnumerator BackCountDown()
        {
            for (int i = 5; i >= 0; i--)
            {
                if (i == 0)
                {
                    StaticDataHandler.LoadBetScene();
                    yield break;
                }
                winScreen.txtCountDown.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        public void GoBackToBetScreen()
        {
            if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();

            StaticDataHandler.LoadBetScene();
        }


        /// <summary>
        /// below code is for after game stater when you click on screen you timer will stop and gameover and winscreen appear with you stop s\clock time.
        /// </summary>

        float timer;
        private void Update()
        {
            if (isGameOver) return;

            if (isGameStart)
            {

                //this 4 line used to over the game automatically in 15 seconds.
                timer += Time.deltaTime;
                if (timer >= 15)
                {
                    OnClickStopButton();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
                    {
                        if (AudioScript.Instant) AudioScript.Instant.PlayBtnClickSound();
                        OnClickStopButton();
                    }
                }
            }
        }

        private void OnClickStopButton()
        {
            isGameOver = true;
            isGameStart = false;
            Debug.Log("Score On Stop : " + score);
            Network_Emitter.Instance.Emit_ThousandScore(score);
        }
    }
}
