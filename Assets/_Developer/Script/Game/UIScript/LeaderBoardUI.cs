using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace TeamGame
{
    /// <summary>
    /// it used to create leaderboard
    /// OnFetchLeaderBoardData() it will get all players from server
    ///  SetUserData() it will stote player in list and SpawnLeadBoarPanel() it will spone player player on leafetboard
    /// </summary>

    public class LeaderBoardDetail
    {
        public string p_id, p_name, p_score;
        public Sprite profilePic;
    }

    public class LeaderBoardUI : MonoBehaviour
    {
        [SerializeField] private GameObject leaderBoardLoadingScreen;
        [SerializeField] private Transform prefabContainer_Today, prefabContainer_Alltime;
        [SerializeField] private GameObject todayBtn, allTimeBtn, todayLeaderBoard, allTimeLeaderBoard;
        [SerializeField] private GameObject panelPrefab;

        public List<LeaderBoardDetail> LB_FriendsList = new List<LeaderBoardDetail>();

        //private void OnDisable()
        //{
        //    foreach (Transform child in prefabContainer_Today.transform)  Destroy(child.gameObject);
        //    foreach (Transform child in prefabContainer_Alltime.transform)  Destroy(child.gameObject);
        //}

        private void Start()
        {
            todayBtn.SetActive(true);
            todayLeaderBoard.SetActive(true);
            allTimeBtn.SetActive(false);
            allTimeLeaderBoard.SetActive(false);

            StartCoroutine(OnFetchLeaderBoardData(1));
            StartCoroutine(OnFetchLeaderBoardData(2));
        }

        public void OnClickBackBtn()
        {
            if (AudioScript.Instant)
            {
                AudioScript.Instant.PlayBtnClickSound();
            }
            UIManager.instance.ShowScreen(ALL_UI_SCREENS.HomeScreen);
        }

        public void TodayBtnClick()
        {
            todayBtn.SetActive(true);
            todayLeaderBoard.SetActive(true);
            allTimeBtn.SetActive(false);
            allTimeLeaderBoard.SetActive(false);
        }

        public void AllTimeBtnClick()
        {
            todayBtn.SetActive(false);
            todayLeaderBoard.SetActive(false);
            allTimeBtn.SetActive(true);
            allTimeLeaderBoard.SetActive(true);
        }

        private IEnumerator OnFetchLeaderBoardData(int data)
        {
            List<string> lst = new List<string>();
            leaderBoardLoadingScreen.SetActive(true);
            JSONNode jsonData = new JSONObject
            {
                //["player_id"] = StaticDataHandler.PlayerID,
                //["friendList"] = StaticDataHandler.ConvertToJsonArray(lst)
            };

            string url = data == 1 ? StaticDataHandler.URL_TODAY_LEADERBOARD : StaticDataHandler.URL_ALLTIME_LEADERBOARD;

            UnityWebRequest result = UnityWebRequest.Put(url, jsonData.ToString());
            result.method = UnityWebRequest.kHttpVerbPOST;
            result.SetRequestHeader("Content-Type", "application/json");
            result.SetRequestHeader("Accept", "application/json");

            yield return result.SendWebRequest();

            if (result.responseCode == 0)
            {
                leaderBoardLoadingScreen.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = "Server did not respond.";
            }
            else if (result.isNetworkError || result.isHttpError)
            {
                leaderBoardLoadingScreen.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = "Something went to wrong..";
            }
            else
            {
                SetUserData(result.downloadHandler.text, data);
            }
            leaderBoardLoadingScreen.SetActive(false);
        }

        private void SetUserData(string response, int data)
        {
            Debug.Log("LEADERBOARD RESPONSE JSON : " + response);
            JSONNode jsonNode = JSON.Parse(response);

            string status = jsonNode["status"];
            for (int i = 0; i < jsonNode.Count; i++)
            {
                string id = jsonNode[i]["player_id"];
                string name = jsonNode[i]["userName"];
                string score = jsonNode[i]["chips"];
                string profilePic = jsonNode[i]["profilePicture"];

                LeaderBoardDetail leader = new LeaderBoardDetail();
                leader.p_id = id;
                leader.p_name = name;
                leader.p_score = score;
                leader.profilePic = ImportantDataMembers.Instance.ProfilePics[int.Parse(profilePic)];

                LB_FriendsList.Add(leader);
                SpawnLeadBoarPanel(i + 1, name, score, leader.profilePic, data);
            }
            leaderBoardLoadingScreen.SetActive(false);
        }

        private void SpawnLeadBoarPanel(int num, string p_name, string p_Score, Sprite pic, int data)
        {
            GameObject child = Instantiate(panelPrefab);
            child.name = p_name;
            child.transform.SetParent(data == 1 ? prefabContainer_Today : prefabContainer_Alltime, false);
            child.transform.localPosition = Vector2.zero;

            LeaderBoardPanelUI LB_P_UI = child.GetComponent<LeaderBoardPanelUI>();

            LB_P_UI.txtNumber.text = num.ToString();
            LB_P_UI.txtName.text = p_name;
            LB_P_UI.txtScore.text = p_Score;
            LB_P_UI.profilePic.sprite = pic;
        }
    }
}