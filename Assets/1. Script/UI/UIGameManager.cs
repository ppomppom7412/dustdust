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
    public UserNameUI userCardMe;//��
    public UserNameUI userCardOther;//���
    public ShadowCountUI shadowTargetUI;//������ �۵��ϴ� UI �籸���ϱ�
    public CountDownUI countDwUI;
    public GameObject skipButton;
    public GameObject myTurnUI;
    public GameObject[] actIcons;

    #region MonoBehaviour func


    private void Start()
    {
        //�� ������� �̺�Ʈ ���
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
    /// �κ� �г� ����
    /// </summary>
    public void OpenLobbyPanel() 
    {
        IngamePanel.SetActive(false);
        lobbyPanel.SetActive(true);

        MapCotroller.Instance.mapPrant.SetActive(false);

        //�ʱ� �����ϱ�
        SetStartButton(true);
    }

    /// <summary>
    /// ��ǲ�ʵ� ���ο� ���� �г����� �����´�.
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
    /// ��Ī ��ư �¿���
    /// </summary>
    /// <param name="isOnOff"></param>
    public void SetStartButton(bool isOnOff)
    {
        if (startButton == null) return;

        //����
        //startButton.gameObject.SetActive(isOnOff);
        //if (connectText != null)
        //{
        //    connectText.gameObject.SetActive(!isOnOff);
        //    connectText.text = "";
        //}

        //����
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
    /// ���� �޼��� ǥ��
    /// </summary>
    /// <param name="message"></param>
    public void SetConnectText(string message)
    {
        if (connectText == null) return;

        connectText.text = message;
    }

    /// <summary>
    /// �г��� ǥ��
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
    /// �ΰ��� �гο���
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
    /// �г��� �����ϱ�
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
    /// �� ����� ǥ�� ������Ʈ
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
    /// ���� �׼ǿ� ���� ǥ��
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
