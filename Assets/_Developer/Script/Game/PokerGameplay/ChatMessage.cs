using System.Collections;
using System.Collections.Generic;
using TeamGame;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour
{
    /// <summary>
    /// here will will manage chat message plane size and test and all of things
    /// </summary>

    public string playerId;
    [SerializeField] private Image pic;
    [SerializeField] private Text name, msg;
    public string chatType;

    private bool added;
    private void Start()
    {
        if (added) return;
        added = true;
        StartCoroutine(SetHeight());
    }

    private IEnumerator SetHeight()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float startHeight = msg.GetComponent<RectTransform>().sizeDelta.y;

        msg.GetComponent<ContentSizeFitter>().enabled = false;

        if (StaticDataHandler.MyPlayerID == playerId)
        {
            if (startHeight <= 55) msg.alignment = TextAnchor.UpperRight;
            else msg.alignment = TextAnchor.UpperLeft;
        }

        Vector2 size = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, startHeight + 145);
        gameObject.GetComponent<RectTransform>().sizeDelta = size;




        ChatController.Instance.CloseAllPanel();


        if (chatType == ChatController.Instance.tableChat)
        {
            transform.SetParent(ChatController.Instance.contentTableChat.transform, false);
            ChatController.Instance.TableChat.SetActive(false);
            ChatController.Instance.contentTableChat.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, size.y);
            ChatController.Instance.TableChat.SetActive(true);
            ChatController.Instance.tableChatBtn.SetActive(true);
        }
        else if (chatType == ChatController.Instance.worldChat)
        {
            transform.SetParent(ChatController.Instance.contentWorldChat.transform, false);
            ChatController.Instance.WorldChat.SetActive(false);
            ChatController.Instance.contentWorldChat.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, size.y);
            ChatController.Instance.WorldChat.SetActive(true);
            ChatController.Instance.worldChatBtn.SetActive(true);
        }
        else
        {
            transform.SetParent(ChatController.Instance.contentTeamChat.transform, false);
            ChatController.Instance.TeamChat.SetActive(false);
            ChatController.Instance.contentTeamChat.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, size.y);
            ChatController.Instance.TeamChat.SetActive(true);
            ChatController.Instance.teamChatBtn.SetActive(true);
        }
    }
}
