using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks //, IPunObservable
{
    //본인 오브젝트 확인용
    public static PlayerController LocalInstance;
    public static PlayerController[] players;
    public PhotonView PV; //이벤트 송수신 //이거 없인 생성이 안됨 ㅠ

    [Header("data")]
    public bool isMe = false;
    public int curpos;
    bool isOpen = false;
    public bool IsOpen 
    {
        set {
            isOpen = value;

            if (isOpen || isMe)
                charBuilder.SetAlphaValue(1);
            else
                charBuilder.SetAlphaValue(0);
        }

        get { return isOpen; }
    }
    //public bool isInit = false;
    public PlayerState state;

    Vector3 rightScale = new Vector3(0.5f, 0.5f, 1f);
    Vector3 leftScale = new Vector3(-0.5f, 0.5f, 1f);
    IEnumerator moveRoutine;

    [Header("Object")]
    public CharacterAnimation playerAnim;
    public CharacterBuilder charBuilder;
    public GameObject meMarker;
    public UserNameUI userUi;

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
        //셋업 전까진 안보이게 하자
        charBuilder.SetAlphaValue(0);
        meMarker.SetActive(false);
        state.Init();

        //isInit = false;

        if (LocalInstance == this)
        {
            isMe = true;
            gameObject.name = PunManager.Instance.myPlayer.NickName;
            userUi = UIGameManager.Instance.userCardMe;

            //Send Ready GameScene
            PV.RPC("ReceivedReady", RpcTarget.All);

        }
        else
        {
            isMe = false;
            userUi = UIGameManager.Instance.userCardOther;

            for (int i = 0; i < PunManager.Instance.curPlayers.Count; ++i)
            {
                if (PunManager.Instance.curPlayers[i] != PunManager.Instance.myPlayer)
                    gameObject.name = PunManager.Instance.curPlayers[i].NickName;
            }
        }

        //StartCoroutine(StartSetup());
    }

    #endregion

    #region main func

    /// <summary>
    /// 초기 위치 및 플레이어 세팅
    /// </summary>
    void InitPlayer() 
    {
        //isInit = true;

        meMarker.SetActive(isMe);

        //프리팹 외형 아무거나 세팅하기
        //나중엔 로비에서 세팅한 값으로 보내는 것으로 하기
        PV.RPC("ReceivedCharacter", RpcTarget.All, ((PresetList)(Random.Range(1, 17))).ToString());

        //랜덤 위치 지정
        PV.RPC("ReceivedPosition", RpcTarget.All, MapCotroller.mapSizeX * MapCotroller.mapSizeY - 1);
    }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public void Damaged() 
    {
        //해당 위치에 있는 플레이어는 공격당하고 공개한다.
        IsOpen = true;

        if (isMe)
            UIGameManager.Instance.HurtEffect();

        //애님 효과주기
        playerAnim.Hit();
        EffectController.Instance.ShowSfx(EffectController.SoundEnum.Hit, transform.position);
        userUi.PlayAnim(ImageLibraryAnimator.MotionList.Hurt);
        userUi.ShakeHeart(state.curHp);

        //데미지 처리 및 싱크
        state.GetDamage();
        userUi.SyncState(state);
    }

    IEnumerator WalkPlayer(MapSlot prev_slot, MapSlot dest_slot, float movetime) 
    {   
        float time = 0f;
        int passagestep = 0;

        //방향 지정
        if (prev_slot.mapPoint.x < dest_slot.mapPoint.x)
            transform.localScale = rightScale;
        else if (prev_slot.mapPoint.x > dest_slot.mapPoint.x)
            transform.localScale = leftScale;

        //상하 좌우 모션 결정
        if ((prev_slot.mapPoint - dest_slot.mapPoint).x == 0)
        {
            playerAnim.Climb();
            userUi.PlayAnim(ImageLibraryAnimator.MotionList.Climb);
        }
        else
        {
            playerAnim.Run();
            userUi.PlayAnim(ImageLibraryAnimator.MotionList.Run);
        }

        while (time < movetime) 
        {
            transform.position = Vector3.Lerp(prev_slot.transform.position, dest_slot.transform.position, time / movetime);
            time += Time.deltaTime;

            if (passagestep.Equals(0) && time / movetime >= 0.1f)
            {
                //이동 통로 열기
                if (isOpen)
                    prev_slot.OpenPassage(dest_slot.mapPoint - prev_slot.mapPoint);
                passagestep += 1;
            }
            else if (passagestep.Equals(1) && time / movetime >= 0.3f)
            {
                //도착 통로 열기
                dest_slot.OpenPassage(prev_slot.mapPoint - dest_slot.mapPoint);
                passagestep += 1;
            }
            else if (passagestep.Equals(2) && time / movetime >= 0.5f)
            {
                //비공개 > 공개시 
                if (prev_slot.damge <= 0 && dest_slot.damge > 0)
                    IsOpen = true;
            }
            else if (passagestep.Equals(3) && time / movetime >= 0.7f) 
            {
                //이전 통로 닫기
                prev_slot.AllClosePassage();
                passagestep += 1;
            }
            else if (passagestep.Equals(4) && time / movetime >= 0.9f)
            {
                //도착 통로 닫기
                dest_slot.AllClosePassage();
                passagestep += 1;
            }

            yield return null;
        }

        playerAnim.Idle();
        userUi.PlayAnim(ImageLibraryAnimator.MotionList.Idle);
    }

    #endregion

    #region send & receive

    /// <summary>
    /// 변경점 보내기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SendAction(int id, int value = 0)
    {
        //DebugLogger.SendDebug("SendChange ("+id+"):"+value + " / turn: "+ GameManager.Instance.currTurn);

        //내 턴이 아니라면 진행하지 않는다. (준비단계인 -1일땐 뭐든 받음)
        if (GameManager.Instance.currTurn > -1 
            && GameManager.Instance.currTurn != GameManager.Instance.myTurn) 
        {
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
            PV.RPC("ReceivedMove", RpcTarget.All, value);
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
    /// 플레이어 마다 생성이 완료될 때 보낸다.
    /// </summary>
    public void ReceivedReady()
    {
        //플레이어를 모두 담아준다.
        players = FindObjectsOfType<PlayerController>();

        //모든 플레이어가 생성 되었다면 자신의 세팅을 해준다.
        if (players != null && players.Length > 1)
        {
            //if (isInit) return;

            DebugLogger.SendDebug("ReceivedReady : All ");

            //각 플레이어는 2개의 플레이어컨트롤러를 가지고 있기 때문에 모두 실행해버리면 4번 진행된다.
            //반드시 본인의 것만 실행하기!
            LocalInstance.InitPlayer();
        }
    }

    [PunRPC]
    /// <summary>
    /// 캐릭터 변경 (init)
    /// </summary>
    public void ReceivedCharacter(string value)
    {
        DebugLogger.SendDebug("PlayerController : ReceivedCharacter" + value);
        //if (photonView.IsMine) return;

        //캐릭터 빌더에 의해 변경하기
        charBuilder.RebuildToString(value);
        userUi.SetProfileCharacter(value);
        userUi.PlayAnim(ImageLibraryAnimator.MotionList.Idle);

        GameManager.Instance.AddReadyCount();
    }

    [PunRPC]
    /// <summary>
    /// 위치 변경 (init)
    /// </summary>
    public void ReceivedPosition(int index)
    {
        GameManager.Instance.AddReadyCount();
        curpos = index;
        transform.position = MapCotroller.Instance.GetSlotPosition(index);
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
    /// 캐릭터 이동 (input)
    /// </summary>
    public void ReceivedMove(int index)
    {
        //if (photonView.IsMine) return;

        MapSlot prev_slot = MapCotroller.Instance.ExistTargetSlot(MapCotroller.GetMapPoint(0, curpos));
        MapSlot dest_slot = MapCotroller.Instance.ExistTargetSlot(MapCotroller.GetMapPoint(0, index));

        //이동 연출 멈추기
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        transform.position = MapCotroller.Instance.GetSlotPosition(curpos);

        curpos = index;

        //'나'이거나 / 비공개 > 공개 / 공개 > 공개
        if (isMe 
            || (prev_slot.damge <= 0 && dest_slot.damge > 0) 
            || (prev_slot.damge > 0 && dest_slot.damge > 0))
        {
            // 이동 연출하기
            moveRoutine = WalkPlayer(prev_slot, dest_slot, 1f);
            StartCoroutine(moveRoutine);
        }
        //공개 > 비공개
        else if ((prev_slot.damge > 0 && dest_slot.damge <= 0)) 
        {
            IsOpen = false;

            playerAnim.Jump();
            userUi.PlayAnim(ImageLibraryAnimator.MotionList.Jump);
            EffectController.Instance.ShowSfx(EffectController.SoundEnum.Smoke, transform.position);

            //이동 전 퐁
            EffectController.Instance.ShowEfx(EffectController.EffectEnum.Jump, transform.position);

            transform.position = MapCotroller.Instance.GetSlotPosition(index);

            //이동 후 퐁
            if (isMe)
                EffectController.Instance.ShowEfx(EffectController.EffectEnum.Smoke, transform.position);
        }
        //비공개 > 비공개
        else if ((prev_slot.damge <= 0 && dest_slot.damge <= 0))
        {
            playerAnim.Jump();
            userUi.PlayAnim(ImageLibraryAnimator.MotionList.Jump);
            EffectController.Instance.ShowSfx(EffectController.SoundEnum.Smoke, transform.position);

            transform.position = MapCotroller.Instance.GetSlotPosition(index);
        }
    }

    [PunRPC]
    /// <summary>
    /// 공격 (input)
    /// </summary>
    public void ReceivedAttack(int index)
    {
        //if (photonView.IsMine) return;

        //무기에 따라 변경하는 것 추가해야함.... @@@@
        userUi.PlayAnim(ImageLibraryAnimator.MotionList.Slash);
        playerAnim.Slash();//검 및 스태프 형태
        //playerAnim.Jab();//주먹 및 단검 형태
        //playerAnim.Shot();//석궁 및 총 형태

        //공격 이펙트는 보여주기
        EffectController.Instance.ShowEfx(EffectController.EffectEnum.Hit, MapCotroller.Instance.GetSlotPosition(index));
        CameraController.Instance.VerticalVibration(3,0.1f,0.2f);
        MapCotroller.Instance.SetSlotDamage(index);
    }

    [PunRPC]
    /// <summary>
    /// 스킬 사용 (input)
    /// </summary>
    public void ReceivedSkill(int index)
    {
        //if (photonView.IsMine) return;

        //데미지를 줬으면 싱크를 해줘야함!!
        //players[i].state.GetDamage();
        //players[i].userUi.SyncState(players[i].state);

        //스킬 미구현이라서 턴 넘기는 것으로 고정
        GameManager.Instance.NextTurn();
    }

    #endregion
}
