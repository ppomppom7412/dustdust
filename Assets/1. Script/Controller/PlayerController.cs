using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;

public class PlayerController : MonoBehaviourPunCallbacks //, IPunObservable
{
    //���� ������Ʈ Ȯ�ο�
    public static PlayerController LocalInstance;
    public PhotonView PV; //�̺�Ʈ �ۼ��� //�̰� ���� ������ �ȵ� ��
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

        //�¾� ������ �Ⱥ��̰� ����
        charBuilder.SetAlphaValue(0);

        state.Init();

        StartCoroutine(StartSetup());
    }

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
    /// ������ ���� �غ�
    /// </summary>
    /// <returns></returns>
    IEnumerator StartSetup()
    {
        yield return new WaitForSeconds(0.8f);

        //������ ���� �ƹ��ų� �����ϱ�
        //���߿� �κ񿡼� ������ ������ ������ ������ �ϱ�
        SendCustom(((CharacterBuilder.PresetList)(Random.Range(1, 17))).ToString());

        //���� ��ġ ����
        SendChange(0, Random.Range(0, MapCotroller.mapSizeX * MapCotroller.mapSizeY - 1));

        //�÷��̾ ��� ����ش�.
        players = FindObjectsOfType<PlayerController>();

    }

    /// <summary>
    /// �������� ���� ���� ������� �����Ѵ�.
    /// </summary>
    void UpdatePlayer()
    {
        //UI ����ȭ
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
    /// ������ ������
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendCustom(string value = "")
    {
        DebugLogger.SendDebug("PlayerController : SendCharacter "+value);
        PV.RPC("ReceivedCharacter", RpcTarget.All, value);
    }

    /// <summary>
    /// ������ ������
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendChange(int id, int value = 0)
    {
        DebugLogger.SendDebug("SendChange ("+id+"):"+value + " / turn: "+ GameManager.Instance.currTurn);

        //�� ���� �ƴ϶�� �������� �ʴ´�. (�غ�ܰ��� -1�϶� ���� ����)
        if (GameManager.Instance.currTurn != -1) 
        {
            if (GameManager.Instance.currTurn != GameManager.Instance.myTurn) 
                return;
        }

        if (id.Equals(-1))//�ϳѱ�
        {
            DebugLogger.SendDebug("PlayerController : NextTurn" + GameManager.Instance.currTurn+1);
            PV.RPC("ReceivedNextTurn", RpcTarget.All);
        }
        else if (id.Equals(0))//�̵�
        {
            DebugLogger.SendDebug("PlayerController : SendPosition "+value);
            PV.RPC("ReceivedPosition", RpcTarget.All, value);
        }
        else if (id.Equals(1))//����
        {
            DebugLogger.SendDebug("PlayerController : SendAttack "+value);
            PV.RPC("ReceivedAttack", RpcTarget.All, value);
        }
        else if (id.Equals(2))//��ų
        {
            DebugLogger.SendDebug("PlayerController : SendSkill " + value);
            PV.RPC("ReceivedSkill", RpcTarget.All, value);
        }
    }

    [PunRPC]
    /// <summary>
    /// ���� ������ �ѱ��
    /// </summary>
    public void ReceivedNextTurn()
    {
        //if (photonView.IsMine) return;

        GameManager.Instance.NextTurn();
    }

    [PunRPC]
    /// <summary>
    /// ĳ���� ���� input
    /// </summary>
    public void ReceivedCharacter(string value)
    {
        //if (photonView.IsMine) return;

        //ĳ���� ������ ���� �����ϱ�
        charBuilder.RebuildToString(value);

        UpdatePlayer();

        GameManager.Instance.AddReadyCount();
    }

    [PunRPC]
    /// <summary>
    /// ��ġ ���� (input)
    /// </summary>
    public void ReceivedPosition(int index)
    {
        //if (photonView.IsMine) return;

        //������ ���¿��� �̵����� �� ���� ����Ʈ �� �̵� ����� �����.
        if (!isMe && isOpen)
        {
            UIGameManager.Instance.shadowTargetUI.ShowCount(curpos);
            EffectController.Instance.ShowEfx(1, transform.position);
            isOpen = false;
        }

        curpos = index;

        transform.position = MapCotroller.Instance.GetSlotPosition(index);

        //���� ���� �̵� �� ȿ��
        if (isMe || isOpen)
        {
            playerAnim.Land(); //��� ��������
            EffectController.Instance.ShowEfx(0, transform.position);
        }

        //UpdatePlayer();

        GameManager.Instance.AddReadyCount();
    }

    [PunRPC]
    /// <summary>
    /// ���� (input)
    /// </summary>
    public void ReceivedAttack(int index)
    {
        //if (photonView.IsMine) return;

        if (isMe || isOpen)
        {
            playerAnim.Slash();//�� �� ������ ����
            //playerAnim.Jab();//�ָ� �� �ܰ� ����
            //playerAnim.Shot();//���� �� �� ����
            //���� ȿ����
        }

        //���� ����Ʈ�� �����ֱ�
        EffectController.Instance.ShowEfx(2, MapCotroller.Instance.GetSlotPosition(index));

        //�ش� ��ġ�� �ִ� �÷��̾�� ���ݴ��ϰ� �����Ѵ�.
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i].curpos == index) 
            {
                players[i].isOpen = true;
                players[i].playerAnim.Hit();
                players[i].state.GetDamage();

                //���� �ƴ� ���°� ���ݴ��ϸ� ���� ����ī��Ʈ�� ���ش�.
                if (!players[i].isMe)
                    UIGameManager.Instance.shadowTargetUI.OffCount();
            }
        }

        UpdatePlayer();
    }

    [PunRPC]
    /// <summary>
    /// ��ų ��� (input)
    /// </summary>
    public void ReceivedSkill(int index)
    {
        //if (photonView.IsMine) return;

        //SetPosition(index);
        UpdatePlayer();
    }

    #endregion
}
