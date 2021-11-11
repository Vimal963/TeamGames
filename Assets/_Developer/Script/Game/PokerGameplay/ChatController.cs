using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using SimpleJSON;
using TeamGame;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ChatController : MonoBehaviour
{


    /// <summary>
    /// it's use to get and set chat in game
    /// LinkToGateKeeperSocket() it will connct to server and get chat meassage from API
    /// GetChatHistory() used to get previous chat mesages from server
    /// ShowChatHistory() used to show chat maessages
    /// SetMessageInPanel() it will spon and set chat message pane
    /// SendMessage() it will send our mesage
    /// </summary>
    public static ChatController Instance;
    public GameObject ChatPanel, teamChatBtn, tableChatBtn, worldChatBtn, TeamChat, TableChat, WorldChat;
    public GameObject chatChild, chatChildMy;
    public GameObject contentTeamChat, contentTableChat, contentWorldChat;
    public InputField msgInputFiled;

    private static Socket socket;

    [SerializeField] private string currentChatType;
    public readonly string usaChat = "usa";
    public readonly string chinaChat = "china";
    public readonly string worldChat = "world";
    public readonly string tableChat = "table";


    private void OnEnable()
    {
        Instance = this;
        Network_Gatekeeper.OnSuccessfullConnectionToServer += LinkToGateKeeperSocket;
    }

    private void OnDisable()
    {
        Network_Gatekeeper.OnSuccessfullConnectionToServer -= LinkToGateKeeperSocket;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in contentTeamChat.transform) Destroy(child.gameObject);
        foreach (Transform child in contentTableChat.transform) Destroy(child.gameObject);
        foreach (Transform child in contentWorldChat.transform) Destroy(child.gameObject);

        StartCoroutine(GetChatHistory(ImportantDataMembers.Instance.currentPickedTeam == TEAM.Usa ? usaChat : chinaChat));
        StartCoroutine(GetChatHistory(tableChat));
        //StartCoroutine(GetChatHistory(worldChat));
    }

    private void LinkToGateKeeperSocket()
    {
        if (Network_Gatekeeper.gamesocket != null)
        {
            socket = Network_Gatekeeper.gamesocket;
            socket.On("roomChat", GetMessage);
        }
    }

    private IEnumerator GetChatHistory(string type)
    {
        JSONNode jsonData = new JSONObject
        {
            ["team"] = type
        };

        string url = StaticDataHandler.URL_CHAT;
        UnityWebRequest result = UnityWebRequest.Put(url, jsonData.ToString());
        result.method = UnityWebRequest.kHttpVerbPOST;
        result.SetRequestHeader("Content-Type", "application/json");
        result.SetRequestHeader("Accept", "application/json");
        yield return result.SendWebRequest();

        //print("Chat Data ==> " + result.downloadHandler.text);

        if (result.responseCode == 0 || result.isNetworkError || result.isHttpError) print("Error In Chat History");
        else
        {
            ShowChatHistory(result.downloadHandler.text, type);
        }
    }

    private void ShowChatHistory(string chat, string type)
    {
        JSONNode results = JSONNode.Parse(chat);
        for (int i = 0; i < results.Count; i++)
        {
            string pic = results[i]["profilePicture"];
            string name = results[i]["name"];
            string msg = results[i]["message"];
            string playerId = results[i]["playerId"];

            SetMessageInPanel(pic, name, msg, playerId, type);
        }

        if (type == tableChat) contentTableChat.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, results.Count * 200);
        else if (type == worldChat) contentWorldChat.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, results.Count * 200);
        else contentTeamChat.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, results.Count * 200);
    }

    private void SetMessageInPanel(string pic, string name, string msg, string playerId, string type)
    {
        //        print(pic + ", " + name + ", " + msg + ", " + playerId + ", " + type);



        GameObject child = Instantiate(StaticDataHandler.MyPlayerID == playerId ? chatChildMy : chatChild);
        child.GetComponent<ChatMessage>().playerId = playerId;
        child.GetComponent<ChatMessage>().chatType = type;


        child.name = name;

        child.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = ImportantDataMembers.Instance.ProfilePics[int.Parse(pic)];
        child.transform.GetChild(1).GetComponent<Text>().text = name;
        child.transform.GetChild(2).GetComponent<Text>().text = msg;
    }


    public void CloseChatPanel()
    {
        ChatPanel.SetActive(false);
        msgInputFiled.text = "";
    }

    public void CloseAllPanel()
    {
        teamChatBtn.SetActive(false);
        TeamChat.SetActive(false);
        tableChatBtn.SetActive(false);
        TableChat.SetActive(false);
        worldChatBtn.SetActive(false);
        WorldChat.SetActive(false);
    }

    public void ShowTeamChat()
    {
        CloseAllPanel();
        ChatPanel.SetActive(true);
        teamChatBtn.SetActive(true);
        TeamChat.SetActive(true);
        currentChatType = ImportantDataMembers.Instance.currentPickedTeam == TEAM.Usa ? usaChat : chinaChat;
    }

    public void ShowTableChat()
    {
        CloseAllPanel();
        ChatPanel.SetActive(true);
        tableChatBtn.SetActive(true);
        TableChat.SetActive(true);
        currentChatType = tableChat;
    }

    public void ShowWorldChat()
    {
        CloseAllPanel();
        ChatPanel.SetActive(true);
        worldChatBtn.SetActive(true);
        WorldChat.SetActive(true);
        currentChatType = worldChat;
    }

    private void GetMessage(Socket socket, Packet packet, object[] args)
    {
        JSONNode chatData = JSONNode.Parse(args[0].ToString());
        string pic = chatData["profilePicture"];
        string name = chatData["name"];
        string msg = chatData["message"];
        string type = chatData["team"];
        string playerId = chatData["playerId"];
        if (StaticDataHandler.MyPlayerID == playerId) return;

        SetMessageInPanel(pic, name, msg, playerId, type);
    }

    public void SelectInputField()
    {
    }

    public void SetText(string text)
    {
        msgInputFiled.text = text;
    }

    public void SendMessage()
    {
        if (msgInputFiled.text == "") return;
        JSONObject data = new JSONObject
        {
            ["playerId"] = StaticDataHandler.MyPlayerID,
            ["message"] = msgInputFiled.text,
            ["team"] = currentChatType
        };

        Debug.Log("Message : " + data.ToString());
        socket.Emit("chatInRoom", data.ToString());
        SetMessageInPanel(StaticDataHandler.MyProfilePic.ToString(), StaticDataHandler.MyPlayerName, msgInputFiled.text, StaticDataHandler.MyPlayerID, currentChatType);

        msgInputFiled.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatPanel.activeSelf && msgInputFiled.text != "")
            {
                SendMessage();
            }
        }
    }
}
