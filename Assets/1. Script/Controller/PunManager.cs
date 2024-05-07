using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class PunManager : MonoBehaviourPunCallbacks
{
    static public PunManager Instance;

    public string gameVersion = "1"; //게임 버전 : 매칭 및 동기화 전용
    public const byte MaxPlayerPerRoom = 2; //최대 인원
    public const float StartWaitTime = 2.5f;

    //플레이어 데이터
    public Player myPlayer = null;
    public List<Player> curPlayers = null;
    int connectCount = 0; //연결횟수

    //0기본 -1끊김 10연결 20방만듬 21방들어옴 30방나감 31누가나감 40게임시작
    int state = 0;

    #region MonoBehaviour 

    private void Awake()
    {
        //이를 통해 마스터 클라이언트에서 PhotonNetwork.LoadLevel()을 사용할 수 있고
        //같은 방에 있는 모든 클라이언트가 레벨을 자동으로 동기화할 수 있습니다.
        // >즉 씬 자동 동기화(loadlevel을 통한) 여부 
        PhotonNetwork.AutomaticallySyncScene = true;

        if (Instance == null)
        {
            Instance = this; 

            //절대 삭제 금지
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
    /// 포톤 연결 및 방입장
    /// </summary>
    public void Connect()
    {
        PhotonNetwork.NickName = UIGameManager.Instance.GetNicknameFieldValue();
        UIGameManager.Instance.SetStartButton(false);

        //서버 연결 성공
        if (PhotonNetwork.IsConnected)
        {
            // 이 시점에서 Random Room에 참여하려면 중요합니다.
            // 실패하면 OnJoinRandomFailed()에서 알림을 받고 하나를 생성합니다.
            //PhotonNetwork.JoinRandomRoom();
        }
        //서버 연결 실패
        else
        {
            if (++connectCount < 10)
            {
                // 가장 먼저 Photon 온라인 서버에 연결해야 합니다.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                //10번이상 시도했을 경우 종료
                Application.Quit();
            }
        }
    }

    /// <summary>
    /// 연결 끊기
    /// </summary>
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// 방 나가기
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// 플레이어 정보 담기
    /// </summary>
    void SetRoomPlayer()
    {
        myPlayer = PhotonNetwork.LocalPlayer;
        curPlayers = new List<Player>();

        //prev : 자신을 기준으로 담는 문제가 있음
        //foreach (int key in PhotonNetwork.CurrentRoom.Players.Keys) 
        //    curPlayers.Add(PhotonNetwork.CurrentRoom.Players[key]);

        //new : 방에 들어온 순서로 담는다.
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i)
            curPlayers.Add(PhotonNetwork.PlayerList[i]);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    /// <summary>
    /// 연결 성공 콜백
    /// </summary>
    public override void OnConnectedToMaster()
    {
        state = 10;
        DebugLogger.SendDebug("Launcher : OnConnectedToMaster()");

        //성공했으면 룸에 들어가기
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 연결 끊김 콜백
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        UIGameManager.Instance.SetStartButton(true);

        DebugLogger.SendDebug("Launcher : OnDisconnected() >> " +  cause);
        state = -1;
    }

    /// <summary>
    /// 랜덤 룸 진입 실패
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        DebugLogger.SendDebug("Launcher : OnJoinRandomFailed()");

        // 방이 없거나 모두 꽉 찼을 수 있습니다.
        if (state.Equals(10))
        {
            DebugLogger.SendDebug("Launcher : CreateRoom()");
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerPerRoom });
        }
    }

    /// <summary>
    /// 랜덤 룸 진입 성공
    /// </summary>
    public override void OnJoinedRoom()
    {
        state = 20;
        DebugLogger.SendDebug("Launcher: OnJoinedRoom() PlayerCount " + PhotonNetwork.CurrentRoom.PlayerCount);

        UIGameManager.Instance.SetConnectText("Wait to other player [1/2]");

        ////[대기 관리] 혼자면 기다린다.
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    ////대기 방으로
        //    //PhotonNetwork.LoadLevel("ReadyScene");
        //}

        //인원수가 찼으면 시작할겁니다.
        if (PhotonNetwork.CurrentRoom.PlayerCount >= MaxPlayerPerRoom)
        {
            StopAllCoroutines();
            StartCoroutine(WaitToStartRoutine(StartWaitTime));
        }
    }

    /// <summary>
    /// (내가) 방에서 떠날 때
    /// </summary>
    public override void OnLeftRoom()
    {
        state = 30;

        ////로비로 돌아오기 (씬에서 로비로)
        //SceneManager.LoadScene(0);
        ////초기세팅만들기
        //UIGameManager.Instance.SetStartButton(true);

        //로비로 돌아오기 (같은 씬에서 로비로)
        UIGameManager.Instance.OpenLobbyPanel();
    }

    /// <summary>
    /// 누군가 방에 들어올 때
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        state = 21;
        DebugLogger.SendDebug("GameManager : EnteredRoom()" + other.NickName);

        //실질적으로 불리는 
        StopAllCoroutines();
        StartCoroutine(WaitToStartRoutine(StartWaitTime));

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    PhotonNetwork.LoadLevel("GameScene");
        //}
    }

    /// <summary>
    /// 누군가 방에서 나갈 때
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
        //추가++ 누군가 나가면 나도 새롭게 방을 잡아야한다.
        else
        {
            state = 31;
            PhotonNetwork.LeaveRoom();
        }
    }

    #endregion

    #region Routine func

    /// <summary>
    /// 대기 시간 후 게임씬으로 이동
    /// </summary>
    /// <param name="wait_time"></param>
    /// <returns></returns>
    IEnumerator WaitToStartRoutine(float wait_time) 
    {
        WaitForSecondsRealtime wait01f = new WaitForSecondsRealtime(0.1f);

        //나갈 수 없게 막아두기
        UIGameManager.Instance.exitButton.SetActive(false);

        while (wait_time > 0) 
        {
            UIGameManager.Instance.SetConnectText("Starting... "+ wait_time.ToString("F1") );
            yield return wait01f;
            wait_time -= 0.1f;
        }


        state = 40;
        SetRoomPlayer();

        //게임 씬을 부르는 방식
        //if (PhotonNetwork.IsMasterClient)
        //    PhotonNetwork.LoadLevel("GameScene");

        //같은 씬에서 부르는 방식
        GameManager.Instance.StartGame();
    }

    #endregion
}
