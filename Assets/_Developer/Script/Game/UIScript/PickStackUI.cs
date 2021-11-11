using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace TeamGame
{
    public class PickStackUI : MonoBehaviour
    {
        /// <summary>
        /// it will used to set and pick stack for playing game
        /// stack will fetched from backand
        /// </summary>
        [SerializeField] private GameObject panelPrefab;
        [SerializeField] private Transform prefabContainer;
        [SerializeField] private GameObject connectedImg, disConnectedImg;

        private void OnEnable()
        {
            connectedImg.SetActive(ImportantDataMembers.Instance.isLoggedIn);
            disConnectedImg.SetActive(!ImportantDataMembers.Instance.isLoggedIn);

            SetStackPanel();

        }

        private void SetStackPanel()
        {
            foreach (Transform child in prefabContainer.transform)
            {
                Destroy(child.gameObject);
            }

            if (ImportantDataMembers.Instance.OnlineStackList.Count <= 0)
            {
                ImportantDataMembers.Instance.OnlineStackList.Add("5");
                ImportantDataMembers.Instance.OnlineStackList.Add("10");
                ImportantDataMembers.Instance.OnlineStackList.Add("15");
                ImportantDataMembers.Instance.OnlineStackList.Add("20");
            }

            for (int i = 0; i < ImportantDataMembers.Instance.OnlineStackList.Count; i++)
            {
                GameObject pnl = Instantiate(panelPrefab);
                pnl.transform.SetParent(prefabContainer, false);
                pnl.transform.localPosition = Vector3.zero;
                pnl.GetComponentInChildren<Text>().text = ImportantDataMembers.Instance.OnlineStackList[i];
                pnl.GetComponent<Button>().onClick.AddListener(() => OnClickStock(pnl.GetComponentInChildren<Text>().text));
            }
        }


        public void OnSelectItem()
        {
            //    StaticDataHandler.loadBetScene();
        }


        public void OnClickStock(string id)
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            ImportantDataMembers.Instance.currentSelectedStack = StaticDataHandler.GetStackval(id);
            Debug.Log("On Stack select : " + StaticDataHandler.GetStackval(id));
            Invoke(nameof(SetSoundInterval), 0.2f);
        }

        private void SetSoundInterval()
        {
            ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickStack;
            StaticDataHandler.LoadBetScene();
        }

        public void OnClickBack(int val = 0)
        {
            if (val == 0)
            {
                if (AudioScript.Instant)
                {
                    AudioScript.Instant.PlayBtnClickSound();
                }
            }

            if (ImportantDataMembers.Instance.currentBettingStyle == BETTING_STYLE.PokerRule)
            //if (ImportantDataMembers.Instance.PreviousScreen == ALL_UI_SCREENS.PickBettingStyle)
            {
                ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickTeam;
                UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickBettingStyle);
            }
            else
            {
                ImportantDataMembers.Instance.PreviousScreen = ALL_UI_SCREENS.PickBettingStyle;
                UIManager.instance.ShowScreen(ALL_UI_SCREENS.PickGame);
            }

        }
    }
}
