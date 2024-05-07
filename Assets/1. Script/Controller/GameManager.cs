using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Singleton;
using Photon.Pun;

public class GameManager : MonoSingleton<GameManager>
{
    //�÷��̾� ���� ������
    public GameObject playerPrefab;
    public GameObject playerOb;

    public int currTurn = 0;
    public int myTurn = 0;
    public int actionIndex = 0;
    const int maxTurn = 2;
    const float waittime = 15f;
    IEnumerator countDownCrtn;

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

        CreatePlayerPrefab();

        currTurn = -1;
        actionIndex = 0;

        //���� �� ���� ã��
        for (int i = 0; i < PunManager.Instance.curPlayers.Count; ++i)
        {
            if (PunManager.Instance.curPlayers[i].Equals(PunManager.Instance.myPlayer))
                myTurn = i;
        }

        //�г��� �����ϱ�
        UIGameManager.Instance.SetNicknameText();

        //���� ����
        Invoke("NextTurn", 0.5f);
    }

    /// <summary>
    /// �÷��̾� ������ �����
    /// </summary>
    public void CreatePlayerPrefab()
    {
        if (PlayerController.LocalInstance == null)
        {
            // Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            playerOb = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            playerOb.transform.parent = MapCotroller.Instance.transform;
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
        //ī��Ʈ �ٿ� ��Ȱ��ȭ
        if (countDownCrtn != null)
            StopCoroutine(countDownCrtn);

        currTurn += 1;
        actionIndex = 0;

        if (currTurn >= maxTurn)
            currTurn = 0;

        ChangeTurn.Invoke();

        Invoke("NextAction", 0.5f);
    }

    /// <summary>
    /// ���� �׼����� �ѱ��
    /// </summary>
    public void NextAction()
    {
        if (actionIndex.Equals(0))
        {
            //ī��Ʈ �ٿ� Ȱ��ȭ
            countDownCrtn = NextCountDown(waittime);
            StartCoroutine(countDownCrtn);
        }

        // 0�� 1�� �ݺ��ϴ� ������
        //�� �ϰ� ���� ���� ���������� ������ ��� �����ϱ�
        if (currTurn % 2 != myTurn) return;
        
        //�ѱ���ư Ȱ��ȭ
            switch (actionIndex) 
            {
                case 0://�̵�
                //�� ��ġ �������� ��ư ����
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.Around1);
                    break;

                case 1://����
                //�� ��ġ ���� ��ü ��ư ����
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.NotTarget);
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
        // 0�� 1�� �ݺ��ϴ� ������
        //�� �ϰ� ���� ���� ���������� ������ ��� �����ϱ�
        if (currTurn % 2 != myTurn) return;

        if (actionIndex.Equals(2))
        {
            //��ų ����
        }
        else 
        {
            PlayerController.LocalInstance.SendChange(actionIndex, value);
        }
    }

    /// <summary>
    /// ���� �ð� ī��Ʈ �ٿ�
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator NextCountDown(float time) 
    {
        WaitForSeconds wait1f = new WaitForSeconds(1f);

        while (time > 0) 
        {
            //UI�� �ð��� ������ �����ֱ�
            yield return wait1f;
        }

        //�ð��� ������ �� �ѱ��
        NextTurn();
    }

}
