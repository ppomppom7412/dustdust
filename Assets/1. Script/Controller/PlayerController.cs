using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;

public class PlayerController : MonoBehaviourPunCallbacks//, IPunObservable
{
    //���� ������Ʈ Ȯ�ο�
    public static PlayerController LocalInstance;
    public PhotonView PV; //�̺�Ʈ �ۼ��� //�̰� ���� ������ �ȵ� ��

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
            //���� ��ġ ����
            SendChange(1, Random.Range(0, MapCotroller.mapSizeX * MapCotroller.mapSizeY-1));

            //������ ���� �ƹ��ų� �����ϱ�
            //���߿� �κ񿡼� ������ ������ ������ ������ �ϱ�
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
    ///// �ްų� ������
    ///// </summary>
    ///// <param name="stream"></param>
    ///// <param name="info"></param>
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        DebugLogger.SendDebug("PlayerController : Send Next()");

    //        //�츮�� �� �÷��̾ �����ϰ� �ֽ��ϴ�:
    //        //�ٸ� �÷��̾�� �����͸� �����ϴ�.
    //        stream.SendNext(curpos);
    //        stream.SendNext(myCharID);
    //    }
    //    else
    //    {
    //        DebugLogger.SendDebug("PlayerController : Receive Next()");

    //        // ��Ʈ��ũ �÷��̾�, ������ ����
    //        this.curpos = (Vector2)stream.ReceiveNext();
    //        this.myCharID = (int)stream.ReceiveNext();

    //        UpdatePlayer();
    //    }
    //}

    #endregion

    #region main func

    /// <summary>
    /// �������� ���� ���� ������� �����Ѵ�.
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
    /// ������ ������
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendCustom(string value = "")
    {
        DebugLogger.SendDebug("PlayerController : SendCharacter");
        PV.RPC("ReceivedCharacter", RpcTarget.All, value);
    }

    /// <summary>
    /// ������ ������
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

        //�� ���� �ƴ϶�� �������� �ʴ´�. (�غ�ܰ��� -1�϶� ���� ����)
        if (GameManager.Instance.currTurn.Equals(-1) && !GameManager.Instance.currTurn.Equals(GameManager.Instance.myTurn))
            return;

        if (id.Equals(-1))//�ϳѱ�
        {
            //DebugLogger.SendDebug("PlayerController : SendPosition");
            //PV.RPC("ReceivedPosition", RpcTarget.All, value);
        }
        else if (id.Equals(0))//�̵�
        {
            DebugLogger.SendDebug("PlayerController : SendPosition");
            PV.RPC("ReceivedPosition", RpcTarget.All, value);
        }
        else if (id.Equals(1))//����
        {
            DebugLogger.SendDebug("PlayerController : SendAttack");
            PV.RPC("ReceivedAttack", RpcTarget.All, value);
        }
        else if (id.Equals(2))//��ų
        {
            DebugLogger.SendDebug("PlayerController : SendSkill");
            PV.RPC("ReceivedSkill", RpcTarget.All, value);
        }
    }

    [PunRPC]
    /// <summary>
    /// ĳ���� ���� input
    /// </summary>
    public void ReceivedCharacter(string value)
    {
        //if (photonView.IsMine) return;

        DebugLogger.SendDebug("PlayerController : ReceivedCharacter");

        //ĳ���� ������ ���� �����ϱ�
        charBuilder.RebuildToString(value);

        UpdatePlayer();
    }

    [PunRPC]
    /// <summary>
    /// ��ġ ���� (input)
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
    /// ���� (input)
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
    /// ��ų ��� (input)
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
