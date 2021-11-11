using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TeamGame;

public class WinChipsAnimController : MonoBehaviour
{

    /// <summary>
    /// here we will set some winning animation in bet screen that which player win tha game.
    /// </summary>
    [SerializeField] private GameObject playerChipWin, systemChipWin;
    int winPlayerIndex;


    public void WinChipsAnimation(float playerWin, float systemWin, int winIndex)
    {
        winPlayerIndex = winIndex;
        PlayerWinAnim(playerWin);
        SystemWinAnim(systemWin);
    }

    private void PlayerWinAnim(float win)
    {
        Text comp = playerChipWin.GetComponent<Text>();
        float start = 0.0f, target = win;
        DOTween.To(() => start, x => start = x, target, 0.5f)
                        .OnUpdate(() => { comp.text = "$" + StaticDataHandler.GetTwoDecimalString(start); });

        float y = 0;
        if (winPlayerIndex < 2 || winPlayerIndex > 6) y = -90f;
        else y = 90f;

        playerChipWin.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, y, 0), 0.5f)
                       .OnComplete(() => { Invoke("PlayerWinAnim1", 0.5f); })
                       .SetEase(Ease.Linear);
    }

    private void PlayerWinAnim1()
    {
        playerChipWin.transform.DOMove(BetScreenUI.Instance.playerScripts[winPlayerIndex].gameObject.transform.position, 1.5f)
        //playerChipWin.transform.DOMove(asd.transform.GetChild(winPlayerIndex).transform.position, 2)
        .OnComplete(() => { StartCoroutine(Complete(playerChipWin)); });
    }

    private void SystemWinAnim(float win)
    {
        Text comp = systemChipWin.GetComponent<Text>();
        float start = 0.0f, target = win;
        DOTween.To(() => start, x => start = x, target, 0.5f)
                        .OnUpdate(() => { comp.text = "$" + StaticDataHandler.GetTwoDecimalString(start); });

        float y = 0;
        if (winPlayerIndex < 2 || winPlayerIndex > 6) y = 90f;
        else y = -90f;


        systemChipWin.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, y, 0), 0.5f)
                       .OnComplete(() => { Invoke("SystemWinAnim1", 0.5f); })
                       .SetEase(Ease.Linear);
    }

    private void SystemWinAnim1()
    {
        float start = 1, target = 0;
        DOTween.To(() => start, x => start = x, target, 1)
                          .OnUpdate(() => { systemChipWin.GetComponent<CanvasGroup>().alpha = start; })
                          .OnComplete(() => { StartCoroutine(Complete(systemChipWin)); });
    }

    IEnumerator Complete(GameObject obj)
    {
        yield return new WaitForSeconds(0.1f);
        obj.SetActive(false);
        if (!playerChipWin.activeSelf && !systemChipWin.activeSelf) Destroy(gameObject);
    }

}
