using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;

public class PlayerController : MonoBehaviourPunCallbacks//, IPunObservable
{
    //본인 오브젝트 확인용
    public static PlayerController LocalInstance;
    public PhotonView PV; //이벤트 송수신 //이거 없인 생성이 안됨 ㅠ

    [Header("data")]
    public bool isMe = false;
    public Vector2 curpos;
    public int myCharID = 0;

    [Header("Object")]
    public Character playerchar;
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
    }

    void Start()
    {
        if (LocalInstance == this)
            isMe = true;
        else
            isMe = false;

        meMarker.SetActive(isMe);
        curpos = Vector2.zero;

        if (isMe)
        {
            //랜덤 위치 지정
            SendChange(1, Random.Range(0, MapCotroller.mapSizeX * MapCotroller.mapSizeY-1));

            //프리팹 외형 아무거나 세팅하기
            //나중엔 로비에서 세팅한 값으로 보내는 것으로 하기
            SendCustom(((CharacterBuilder.PresetList)(Random.Range(1, 17))).ToString());
        }
    }

    //private void Update()
    //{
    //    if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
    //    {
    //        return;
    //    }
    //}

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
    /// 서버에서 보낸 것을 기반으로 변경한다.
    /// </summary>
    void UpdatePlayer()
    {
        gameObject.transform.position = curpos * 2f;
    }

    public void SetPosition(int index)
    {
        curpos = MapCotroller.GetMapPoint(0, index);

        MapSlot target = MapCotroller.Instance.ExistTargetSlot(curpos);

        if (target != null)
            transform.position = target.transform.position;
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
        DebugLogger.SendDebug("PlayerController : SendCharacter");
        PV.RPC("ReceivedCharacter", RpcTarget.All, value);
    }

    /// <summary>
    /// 변경점 보내기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendChange(int id, int value = 0)
    {
        //if (id.Equals(0))
        //{
        //    DebugLogger.SendDebug("PlayerController : SendCharacter");
        //    PV.RPC("ReceivedCharacter", RpcTarget.All, value);
        //}

        //내 턴이 아니라면 진행하지 않는다. (준비단계인 -1일땐 뭐든 받음)
        if (GameManager.Instance.currTurn.Equals(-1) && !GameManager.Instance.currTurn.Equals(GameManager.Instance.myTurn))
            return;

        if (id.Equals(-1))//턴넘김
        {
            //DebugLogger.SendDebug("PlayerController : SendPosition");
            //PV.RPC("ReceivedPosition", RpcTarget.All, value);
        }
        else if (id.Equals(0))//이동
        {
            DebugLogger.SendDebug("PlayerController : SendPosition");
            PV.RPC("ReceivedPosition", RpcTarget.All, value);
        }
        else if (id.Equals(1))//공격
        {
            DebugLogger.SendDebug("PlayerController : SendAttack");
            PV.RPC("ReceivedAttack", RpcTarget.All, value);
        }
        else if (id.Equals(2))//스킬
        {
            DebugLogger.SendDebug("PlayerController : SendSkill");
            PV.RPC("ReceivedSkill", RpcTarget.All, value);
        }
    }

    [PunRPC]
    /// <summary>
    /// 캐릭터 변경 input
    /// </summary>
    public void ReceivedCharacter(string value)
    {
        //if (photonView.IsMine) return;

        DebugLogger.SendDebug("PlayerController : ReceivedCharacter");

        //캐릭터 빌더에 의해 변경하기
        charBuilder.RebuildToString(value);

        UpdatePlayer();
    }

    [PunRPC]
    /// <summary>
    /// 위치 변경 (input)
    /// </summary>
    public void ReceivedPosition(int index)
    {
        //if (photonView.IsMine) return;

        DebugLogger.SendDebug("PlayerController : RecivedPosition");
        SetPosition(index);
        UpdatePlayer();
    }

    [PunRPC]
    /// <summary>
    /// 공격 (input)
    /// </summary>
    public void ReceivedAttack(int index)
    {
        //if (photonView.IsMine) return;

        DebugLogger.SendDebug("PlayerController : RecivedAttack");
        //SetPosition(index);
        UpdatePlayer();
    }

    [PunRPC]
    /// <summary>
    /// 스킬 사용 (input)
    /// </summary>
    public void ReceivedSkill(int index)
    {
        //if (photonView.IsMine) return;

        DebugLogger.SendDebug("PlayerController : RecivedSkill");
        //SetPosition(index);
        UpdatePlayer();
    }

    #endregion
}
