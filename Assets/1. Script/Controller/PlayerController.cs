using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;

public class PlayerController : MonoBehaviourPunCallbacks //, IPunObservable
{
    //본인 오브젝트 확인용
    public static PlayerController LocalInstance;
    public PhotonView PV; //이벤트 송수신 //이거 없인 생성이 안됨 ㅠ
    public PlayerController[] players;

    [Header("data")]
    public bool isMe = false;
    public int curpos;
    public int myCharID = 0;
    public bool isOpen = false;
    public PlayerState state;

    [Header("Object")]
    public CharacterAnimation playerAnim;
    public CharacterBuilder charBuilder;
    public GameObject meMarker;

    #region MonoBehaviour

    void Awake() 
    {
        if (photonView.IsMine)
            LocalInstance = this;

        if (PV == null)
            PV = gameObject.GetComponent<PhotonView>();

        if (PV == null)
            PV = gameObject.AddComponent<PhotonView>();

        state = new PlayerState();
    }

    public override void OnEnable()
    {
        if (LocalInstance == this)
            isMe = true;
        else
            isMe = false;

        meMarker.SetActive(isMe);

        //셋업 전까진 안보이게 하자
        charBuilder.SetAlphaValue(0);

        state.Init();

        StartCoroutine(StartSetup());
    }

    #endregion

    #region IPunObservable implementation

    ///// <summary>
    ///// 받거나 보내기
    ///// </summary>
    ///// <param name="stream"></param>
    ///// <param name="info"></param>
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        DebugLogger.SendDebug("PlayerController : Send Next()");

    //        //우리는 이 플레이어를 소유하고 있습니다:
    //        //다른 플레이어에게 데이터를 보냅니다.
    //        stream.SendNext(curpos);
    //        stream.SendNext(myCharID);
    //    }
    //    else
    //    {
    //        DebugLogger.SendDebug("PlayerController : Receive Next()");

    //        // 네트워크 플레이어, 데이터 수신
    //        this.curpos = (Vector2)stream.ReceiveNext();
    //        this.myCharID = (int)stream.ReceiveNext();

    //        UpdatePlayer();
    //    }
    //}

    #endregion

    #region main func

    /// <summary>
    /// 시작을 위한 준비
    /// </summary>
    /// <returns></returns>
    IEnumerator StartSetup()
    {
        yield return new WaitForSeconds(0.8f);

        //프리팹 외형 아무거나 세팅하기
        //나중엔 로비에서 세팅한 값으로 보내는 것으로 하기
        SendCustom(((CharacterBuilder.PresetList)(Random.Range(1, 17))).ToString());

        //랜덤 위치 지정
        SendChange(0, Random.Range(0, MapCotroller.mapSizeX * MapCotroller.mapSizeY - 1));

        //플레이어를 모두 담아준다.
        players = FindObjectsOfType<PlayerController>();

    }

    /// <summary>
    /// 서버에서 보낸 것을 기반으로 변경한다.
    /// </summary>
    void UpdatePlayer()
    {
        //UI 동기화
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i].isMe)
                UIGameManager.Instance.userCardMe.SyncState(players[i].state);
            else
                UIGameManager.Instance.userCardOther.SyncState(players[i].state);
        }
    }

    #endregion

    #region send & receive

    /// <summary>
    /// 변경점 보내기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendCustom(string value = "")
    {
        DebugLogger.SendDebug("PlayerController : SendCharacter "+value);
        PV.RPC("ReceivedCharacter", RpcTarget.All, value);
    }

    /// <summary>
    /// 변경점 보내기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendChange(int id, int value = 0)
    {
        DebugLogger.SendDebug("SendChange ("+id+"):"+value + " / turn: "+ GameManager.Instance.currTurn);

        //내 턴이 아니라면 진행하지 않는다. (준비단계인 -1일땐 뭐든 받음)
        if (GameManager.Instance.currTurn != -1) 
        {
            if (GameManager.Instance.currTurn != GameManager.Instance.myTurn) 
                return;
        }

        if (id.Equals(-1))//턴넘김
        {
            DebugLogger.SendDebug("PlayerController : NextTurn" + GameManager.Instance.currTurn+1);
            PV.RPC("ReceivedNextTurn", RpcTarget.All);
        }
        else if (id.Equals(0))//이동
        {
            DebugLogger.SendDebug("PlayerController : SendPosition "+value);
            PV.RPC("ReceivedPosition", RpcTarget.All, value);
        }
        else if (id.Equals(1))//공격
        {
            DebugLogger.SendDebug("PlayerController : SendAttack "+value);
            PV.RPC("ReceivedAttack", RpcTarget.All, value);
        }
        else if (id.Equals(2))//스킬
        {
            DebugLogger.SendDebug("PlayerController : SendSkill " + value);
            PV.RPC("ReceivedSkill", RpcTarget.All, value);
        }
    }

    [PunRPC]
    /// <summary>
    /// 다음 턴으로 넘기기
    /// </summary>
    public void ReceivedNextTurn()
    {
        //if (photonView.IsMine) return;

        GameManager.Instance.NextTurn();
    }

    [PunRPC]
    /// <summary>
    /// 캐릭터 변경 input
    /// </summary>
    public void ReceivedCharacter(string value)
    {
        //if (photonView.IsMine) return;

        //캐릭터 빌더에 의해 변경하기
        charBuilder.RebuildToString(value);

        UpdatePlayer();

        GameManager.Instance.AddReadyCount();
    }

    [PunRPC]
    /// <summary>
    /// 위치 변경 (input)
    /// </summary>
    public void ReceivedPosition(int index)
    {
        //if (photonView.IsMine) return;

        //공개된 상태에서 이동했을 때 점프 이펙트 및 이동 기록을 남긴다.
        if (!isMe && isOpen)
        {
            UIGameManager.Instance.shadowTargetUI.ShowCount(curpos);
            EffectController.Instance.ShowEfx(1, transform.position);
            isOpen = false;
        }

        curpos = index;

        transform.position = MapCotroller.Instance.GetSlotPosition(index);

        //나만 보는 이동 후 효과
        if (isMe || isOpen)
        {
            playerAnim.Land(); //잠깐 움츠리기
            EffectController.Instance.ShowEfx(0, transform.position);
        }

        //UpdatePlayer();

        GameManager.Instance.AddReadyCount();
    }

    [PunRPC]
    /// <summary>
    /// 공격 (input)
    /// </summary>
    public void ReceivedAttack(int index)
    {
        //if (photonView.IsMine) return;

        if (isMe || isOpen)
        {
            playerAnim.Slash();//검 및 스태프 형태
            //playerAnim.Jab();//주먹 및 단검 형태
            //playerAnim.Shot();//석궁 및 총 형태
            //공격 효과음
        }

        //공격 이펙트는 보여주기
        EffectController.Instance.ShowEfx(2, MapCotroller.Instance.GetSlotPosition(index));

        //해당 위치에 있는 플레이어는 공격당하고 공개한다.
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i].curpos == index) 
            {
                players[i].isOpen = true;
                players[i].playerAnim.Hit();
                players[i].state.GetDamage();

                //내가 아닌 상태가 공격당하면 이전 공격카운트를 꺼준다.
                if (!players[i].isMe)
                    UIGameManager.Instance.shadowTargetUI.OffCount();
            }
        }

        UpdatePlayer();
    }

    [PunRPC]
    /// <summary>
    /// 스킬 사용 (input)
    /// </summary>
    public void ReceivedSkill(int index)
    {
        //if (photonView.IsMine) return;

        //SetPosition(index);
        UpdatePlayer();
    }

    #endregion
}
