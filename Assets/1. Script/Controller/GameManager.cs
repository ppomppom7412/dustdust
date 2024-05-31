using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Singleton;
using Photon.Pun;

public class GameManager : MonoSingleton<GameManager>
{
    public enum TurnIndex {Skip = -1, Move, Attack, Skill }

    //�÷��̾� ���� ������
    public GameObject playerPrefab;
    public GameObject playerOb;

    public int currTurn = 0;
    public int myTurn = 0;
    public int actionIndex = 0;
    const int maxTurn = 2;
    int readycount = 0;
    public const int waittime = 20;

    public UnityEngine.Events.UnityEvent ChangeTurn;

    #region MonoBehaviour func

    //private void Start()
    //{
        //���� ���� �ҷ����� ���
        //StartGame();
    //}

    #endregion

    /// <summary>
    /// ���� ���ۿ� �ʿ��� �غ� �� ����
    /// </summary>
    public void StartGame()
    {
        UIGameManager.Instance.OpenIngamePanel();

        if (PlayerController.LocalInstance == null)
        {
            // Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            playerOb = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        }

        currTurn = -1;
        actionIndex = -1;

        //���� �� ���� ã��
        for (int i = 0; i < PunManager.Instance.curPlayers.Count; ++i)
        {
            if (PunManager.Instance.curPlayers[i].Equals(PunManager.Instance.myPlayer))
                myTurn = i;
        }

        //�г��� �����ϱ�
        UIGameManager.Instance.SetNicknameText();

        readycount = 0;
    }

    /// <summary>
    /// ��� �÷��̾� ������ ���� ��
    /// </summary>
    public void AddReadyCount() 
    {
        if (currTurn < 0 && ++readycount >= 4)
        {
            //�� �ѱ��
            if (myTurn.Equals(0))
                PlayerController.LocalInstance.SendChange(-1);

            //�������� ĳ���� ǥ���ϱ�
            PlayerController.LocalInstance.charBuilder.SetAlphaValue(1f);
        }
    }

    /// <summary>
    /// �г��� ���
    /// </summary>
    /// <returns></returns>
    public string GetNickname() 
    {
        //������ ��ϵ� �г��� ��ȯ
        return PhotonNetwork.NickName;
    }

    /// <summary>
    ///  ���� ������ �ѱ��
    /// </summary>
    public void NextTurn()
    {
        //���ݽ� ���� ĳ���͸� �����ְ�
        //�׸��ڸ� ���� 1���� �����ϰ� ���ش�.

        //��� ��ư ��Ȱ��ȭ
        MapCotroller.Instance.OffAllSlot();

        currTurn += 1;
        actionIndex = -1;

        if (currTurn >= maxTurn)
            currTurn = 0;

        ChangeTurn.Invoke();

        //�� �ϰ� ���� �� ������ ��� �����ϱ�
        if (currTurn.Equals(myTurn))
        {
            UIGameManager.Instance.skipButton.SetActive(true);
            NextAction();
        }
    }

    /// <summary>
    /// ���� �׼����� �ѱ��
    /// </summary>
    public void NextAction()
    {
        //�� �ϰ� ���� �� ������ ��� �����ϱ�
        if (!currTurn.Equals(myTurn)) return;

        UIGameManager.Instance.ShowActionIcon(++actionIndex);

        Debug.Log("[On Action] "+actionIndex);

        //�ѱ���ư Ȱ��ȭ
        switch (actionIndex) 
            {
                case 0://�̵�
                //�� ��ġ �������� ��ư ����
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.Around1);
                    break;

                case 1://����
                //�� ��ġ ���� ��ü ��ư ����
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.All);
                    break;

                case 2://��ų                
                //���� ��ų�� ������� ��뿩�� üũ
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.Random);
                    break;
            }
        
    }

    /// <summary>
    /// �޾ƿ� �� �����ϱ�
    /// </summary>
    /// <param name="value"></param>
    public void Action(int value) 
    {
        //�� �ϰ� ���� �� ������ ��� �����ϱ�
        if (!currTurn.Equals(myTurn)) return;

        if (value.Equals(-1))
        {
            //�� �ѱ��
            PlayerController.LocalInstance.SendChange(-1);
        }
        else if (actionIndex.Equals(2))
        {
            //��ų ����
        }
        else 
        {
            PlayerController.LocalInstance.SendChange(actionIndex, value);
            NextAction();
        }
    }

}
