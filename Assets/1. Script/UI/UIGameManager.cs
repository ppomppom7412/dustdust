using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Singleton;

public class UIGameManager : MonoSingleton<UIGameManager>
{
    [Header("Lobby UI")]
    public GameObject lobbyPanel;
    public GameObject idlePanel;
    public GameObject roomPanel;

    public Text nicknameText;
    public Text connectText;
    public InputField nickField;
    //public GameObject[] presetButtons;
    public GameObject startButton;
    public GameObject exitButton;

    [Header("Ingame UI")]
    public GameObject IngamePanel;
    public UserNameUI userCardMe;//나
    public UserNameUI userCardOther;//상대
    public ShadowCountUI shadowTargetUI;//별도로 작동하는 UI 재구성하기
    public CountDownUI countDwUI;
    public GameObject skipButton;
    public GameObject myTurnUI;
    public GameObject[] actIcons;

    #region MonoBehaviour func


    private void Start()
    {
        //턴 변경시의 이벤트 등록
        GameManager.Instance.ChangeTurn.AddListener(UpdateTurnImage);

        OpenLobbyPanel();
    }

    //private void OnDestroy()
    //{
    //    GameManager.Instance.ChangeTurn.RemoveListener(UpdateTurnImage);
    //}

    #endregion

    #region Lobby UI func

    /// <summary>
    /// 로비 패널 열기
    /// </summary>
    public void OpenLobbyPanel() 
    {
        IngamePanel.SetActive(false);
        lobbyPanel.SetActive(true);

        MapCotroller.Instance.mapPrant.SetActive(false);

        //초기 세팅하기
        SetStartButton(true);
    }

    /// <summary>
    /// 인풋필드 내부에 적힌 닉네임을 가져온다.
    /// </summary>
    /// <returns></returns>
    public string GetNicknameFieldValue()
    {
        if (nickField == null) return "not find field";

        if (nicknameText != null)
            nicknameText.text = nickField.text;

        return nickField.text;
    }

    /// <summary>
    /// 매칭 버튼 온오프
    /// </summary>
    /// <param name="isOnOff"></param>
    public void SetStartButton(bool isOnOff)
    {
        if (startButton == null) return;

        //이전
        //startButton.gameObject.SetActive(isOnOff);
        //if (connectText != null)
        //{
        //    connectText.gameObject.SetActive(!isOnOff);
        //    connectText.text = "";
        //}

        //이후
        if (isOnOff)
        {
            nickField.enabled = true;
            //for (int i = 0; i < presetButtons.Length; ++i)
            //    presetButtons[i].SetActive(true);
            startButton.SetActive(true);
            connectText.text = "";

            idlePanel.SetActive(true);
            roomPanel.SetActive(false);
        }
        else
        {
            nickField.enabled = false;
            //for (int i = 0; i < presetButtons.Length; ++i)
            //    presetButtons[i].SetActive(false);
            startButton.SetActive(false);

            idlePanel.SetActive(false);
            roomPanel.SetActive(true);
            exitButton.SetActive(true);
        }
    }

    /// <summary>
    /// 연결 메세지 표기
    /// </summary>
    /// <param name="message"></param>
    public void SetConnectText(string message)
    {
        if (connectText == null) return;

        connectText.text = message;
    }

    /// <summary>
    /// 닉네임 표기
    /// </summary>
    /// <param name="nickstring"></param>
    public void SetNicknameText(string nickstring)
    {
        if (nicknameText == null) return;
        nicknameText.text = nickstring;
    }

    #endregion

    #region Ingame UI func

    /// <summary>
    /// 인게임 패널열기
    /// </summary>
    public void OpenIngamePanel() 
    {
        lobbyPanel.SetActive(false);
        IngamePanel.SetActive(true);

        MapCotroller.Instance.mapPrant.SetActive(true);
        skipButton.SetActive(false);
        myTurnUI.SetActive(false);
        countDwUI.gameObject.SetActive(false);
        ShowActionIcon(-1);
    }

    /// <summary>
    /// 닉네임 설정하기
    /// </summary>
    public void SetNicknameText()
    {
        for (int i = 0; i < PunManager.Instance.curPlayers.Count; ++i)
        {
            if (PunManager.Instance.curPlayers[i].Equals(PunManager.Instance.myPlayer))
                userCardMe.SetUserNameUI(PunManager.Instance.curPlayers[i].NickName, true);
            else
                userCardOther.SetUserNameUI(PunManager.Instance.curPlayers[i].NickName, false);
        }

        UpdateTurnImage();
    }

    /// <summary>
    /// 턴 변경시 표기 업데이트
    /// </summary>
    public void UpdateTurnImage() 
    {
        skipButton.SetActive(false);
        ShowActionIcon(-1);

        if (GameManager.Instance.currTurn.Equals(GameManager.Instance.myTurn))
        {
            myTurnUI.SetActive(true);
            userCardMe.SetTurn(true);
            userCardOther.SetTurn(false);
        }
        else 
        {
            userCardMe.SetTurn(false);
            userCardOther.SetTurn(true);
        }
    }

    /// <summary>
    /// 현재 액션에 대한 표기
    /// </summary>
    /// <param name="act"></param>
    public void ShowActionIcon(int act)
    {
        for (int i = 0; i < actIcons.Length; ++i) 
        {
            if (act.Equals(i))
                actIcons[i].SetActive(true);
            else
                actIcons[i].SetActive(false);
        }
    }

    #endregion
}
