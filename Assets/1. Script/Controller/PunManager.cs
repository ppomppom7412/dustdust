using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class PunManager : MonoBehaviourPunCallbacks
{
    static public PunManager Instance;

    public string gameVersion = "1"; //���� ���� : ��Ī �� ����ȭ ����
    public const byte MaxPlayerPerRoom = 2; //�ִ� �ο�
    public const float StartWaitTime = 2.5f;

    //�÷��̾� ������
    public Player myPlayer = null;
    public List<Player> curPlayers = null;
    int connectCount = 0; //����Ƚ��

    //0�⺻ -1���� 10���� 20�游�� 21����� 30�泪�� 31�������� 40���ӽ���
    int state = 0;

    #region MonoBehaviour 

    private void Awake()
    {
        //�̸� ���� ������ Ŭ���̾�Ʈ���� PhotonNetwork.LoadLevel()�� ����� �� �ְ�
        //���� �濡 �ִ� ��� Ŭ���̾�Ʈ�� ������ �ڵ����� ����ȭ�� �� �ֽ��ϴ�.
        // >�� �� �ڵ� ����ȭ(loadlevel�� ����) ���� 
        PhotonNetwork.AutomaticallySyncScene = true;

        if (Instance == null)
        {
            Instance = this; 

            //���� ���� ����
            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (state.Equals(0))
            UIGameManager.Instance.SetStartButton(true);
    }
    #endregion

    #region net func

    /// <summary>
    /// ���� ���� �� ������
    /// </summary>
    public void Connect()
    {
        PhotonNetwork.NickName = UIGameManager.Instance.GetNicknameFieldValue();
        UIGameManager.Instance.SetStartButton(false);

        //���� ���� ����
        if (PhotonNetwork.IsConnected)
        {
            // �� �������� Random Room�� �����Ϸ��� �߿��մϴ�.
            // �����ϸ� OnJoinRandomFailed()���� �˸��� �ް� �ϳ��� �����մϴ�.
            //PhotonNetwork.JoinRandomRoom();
        }
        //���� ���� ����
        else
        {
            if (++connectCount < 10)
            {
                // ���� ���� Photon �¶��� ������ �����ؾ� �մϴ�.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                //10���̻� �õ����� ��� ����
                Application.Quit();
            }
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// �� ������
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// �÷��̾� ���� ���
    /// </summary>
    void SetRoomPlayer()
    {
        myPlayer = PhotonNetwork.LocalPlayer;
        curPlayers = new List<Player>();

        //prev : �ڽ��� �������� ��� ������ ����
        //foreach (int key in PhotonNetwork.CurrentRoom.Players.Keys) 
        //    curPlayers.Add(PhotonNetwork.CurrentRoom.Players[key]);

        //new : �濡 ���� ������ ��´�.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
            curPlayers.Add(PhotonNetwork.PlayerList[i]);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    /// <summary>
    /// ���� ���� �ݹ�
    /// </summary>
    public override void OnConnectedToMaster()
    {
        state = 10;
        DebugLogger.SendDebug("Launcher : OnConnectedToMaster()");

        //���������� �뿡 ����
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// ���� ���� �ݹ�
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        UIGameManager.Instance.SetStartButton(true);

        DebugLogger.SendDebug("Launcher : OnDisconnected() >> " +  cause);
        state = -1;
    }

    /// <summary>
    /// ���� �� ���� ����
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        DebugLogger.SendDebug("Launcher : OnJoinRandomFailed()");

        // ���� ���ų� ��� �� á�� �� �ֽ��ϴ�.
        if (state.Equals(10))
        {
            DebugLogger.SendDebug("Launcher : CreateRoom()");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
        }
    }

    /// <summary>
    /// ���� �� ���� ����
    /// </summary>
    public override void OnJoinedRoom()
    {
        state = 20;
        DebugLogger.SendDebug("Launcher: OnJoinedRoom() PlayerCount " + PhotonNetwork.CurrentRoom.PlayerCount);

        UIGameManager.Instance.SetConnectText("Wait to other player [1/2]");

        ////[��� ����] ȥ�ڸ� ��ٸ���.
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    ////��� ������
        //    //PhotonNetwork.LoadLevel("ReadyScene");
        //}

        //�ο����� á���� �����Ұ̴ϴ�.
        if (PhotonNetwork.CurrentRoom.PlayerCount >= MaxPlayerPerRoom)
        {
            StopAllCoroutines();
            StartCoroutine(WaitToStartRoutine(StartWaitTime));
        }
    }

    /// <summary>
    /// (����) �濡�� ���� ��
    /// </summary>
    public override void OnLeftRoom()
    {
        state = 30;

        ////�κ�� ���ƿ��� (������ �κ��)
        //SceneManager.LoadScene(0);
        ////�ʱ⼼�ø����
        //UIGameManager.Instance.SetStartButton(true);

        //�κ�� ���ƿ��� (���� ������ �κ��)
        UIGameManager.Instance.OpenLobbyPanel();
    }

    /// <summary>
    /// ������ �濡 ���� ��
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        state = 21;
        DebugLogger.SendDebug("GameManager : EnteredRoom()" + other.NickName);

        //���������� �Ҹ��� 
        StopAllCoroutines();
        StartCoroutine(WaitToStartRoutine(StartWaitTime));

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    PhotonNetwork.LoadLevel("GameScene");
        //}
    }

    /// <summary>
    /// ������ �濡�� ���� ��
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerLeftRoom(Player other)
    {
        DebugLogger.SendDebug("GameManager : OnPlayerLeftRoom()" + other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            state = 31;
            PhotonNetwork.LeaveRoom();
        }
        //�߰�++ ������ ������ ���� ���Ӱ� ���� ��ƾ��Ѵ�.
        else
        {
            state = 31;
            PhotonNetwork.LeaveRoom();
        }
    }

    #endregion

    #region Routine func

    /// <summary>
    /// ��� �ð� �� ���Ӿ����� �̵�
    /// </summary>
    /// <param name="wait_time"></param>
    /// <returns></returns>
    IEnumerator WaitToStartRoutine(float wait_time) 
    {
        WaitForSecondsRealtime wait01f = new WaitForSecondsRealtime(0.1f);

        //���� �� ���� ���Ƶα�
        UIGameManager.Instance.exitButton.SetActive(false);

        while (wait_time > 0) 
        {
            UIGameManager.Instance.SetConnectText("Starting... "+ wait_time.ToString("F1") );
            yield return wait01f;
            wait_time -= 0.1f;
        }


        state = 40;
        SetRoomPlayer();

        //���� ���� �θ��� ���
        //if (PhotonNetwork.IsMasterClient)
        //    PhotonNetwork.LoadLevel("GameScene");

        //���� ������ �θ��� ���
        GameManager.Instance.StartGame();
    }

    #endregion
}
