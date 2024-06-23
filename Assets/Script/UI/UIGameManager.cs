using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Singleton;
using DG.Tweening;
using NaughtyAttributes;

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
    public GameObject hurtAnimUI;
    public GameObject[] actIcons;

    [Header("Loading UI")]
    public GameObject LoadingPanel;
    public Slider loadSlider;
    public Text loadText;

    #region MonoBehaviour func

    private void Start()
    {
        //턴 변경시의 이벤트 등록
        GameManager.Instance.ChangeTurn.AddListener(UpdateTurnImage);

        OpenLobbyPanel();
        LoadingPanel.SetActive(false);
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

        userCardMe.InitState();
        userCardOther.InitState();
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

        //행동 제어 시간 + 시간초과시 행동 넘김
        countDwUI.StartTimer(GameManager.waittime);
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

    public void HurtEffect() 
    {
        hurtAnimUI.SetActive(false);
        hurtAnimUI.SetActive(true);
    }

    #endregion

    #region Loading UI func

    public void OpenLoadingPanel() 
    {
        //켜둔 팝업 지우기
        YellowGreen.Popup.PopupManager.Instance.AllClosePop();

        //못 닫고 못 열게 만들기
        YellowGreen.Popup.PopupManager.Instance.AddInputBlock();

        loadSlider.value = 0f;
        loadText.text = "0%";

        //로딩 팝업을 켜고 뒤에는 인게임을 열어준다,
        LoadingPanel.SetActive(true);
        OpenIngamePanel();

        StartCoroutine(WaitToPlayer(5.0f));
    }

    IEnumerator WaitToPlayer(float wait_time) 
    {
        float progress = 0f;
        WaitForSeconds wait02f = new WaitForSeconds(0.2f);
        WaitForSeconds waittarget = new WaitForSeconds(wait_time);

        //인게임 초기화
        GameManager.Instance.InitGame();

        //90%까지 자동이동
        DOTween.To(() => progress, per => progress = per, 0.9f, wait_time)
            .OnUpdate(() => { SetLoadSlider(progress); });

        yield return wait_time;

        //1명 완료시 95%
        while (GameManager.Instance.GetReadyCount() < 2)
            yield return wait02f;

        //95%까지 자동이동
        DOTween.To(() => progress, per => progress = per, 0.95f, 0.15f)
            .OnUpdate(() => { SetLoadSlider(progress); });

        yield return wait02f;

        while (GameManager.Instance.GetReadyCount() < 4)
            yield return wait02f;

        //100%까지 자동이동
        DOTween.To(() => progress, per => progress = per, 1f, 0.15f)
            .OnUpdate(() => { SetLoadSlider(progress); });

        yield return wait02f;

        YellowGreen.Popup.PopupManager.Instance.RemovePop();
        LoadingPanel.SetActive(false);

        GameManager.Instance.AddReadyCount();
    }

    void SetLoadSlider(float value) 
    {
        loadSlider.value = value;
        loadText.text = (loadSlider.value * 100f).ToString("0") + "%";
    }

    #endregion
}
