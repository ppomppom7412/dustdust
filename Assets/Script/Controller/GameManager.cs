using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Singleton;
using Photon.Pun;

public class GameManager : MonoSingleton<GameManager>
{
    public enum TurnIndex {Skip = -1, Move, Attack, Skill }

    //플레이어 전용 프리팹
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
        //게임 씬을 불러오는 방식
        //StartGame();
    //}

    #endregion

    /// <summary>
    /// 게임 시작에 필요한 준비 및 설정
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

        //나의 턴 순서 찾기
        for (int i = 0; i < PunManager.Instance.curPlayers.Count; ++i)
        {
            if (PunManager.Instance.curPlayers[i].Equals(PunManager.Instance.myPlayer))
                myTurn = i;
        }

        //닉네임 설정하기
        UIGameManager.Instance.SetNicknameText();

        readycount = 0;
    }

    /// <summary>
    /// 모든 플레이어 세팅이 끝날 때
    /// </summary>
    public void AddReadyCount() 
    {
        if (currTurn < 0 && ++readycount >= 4)
        {
            //턴 넘기기
            if (myTurn.Equals(0))
                PlayerController.LocalInstance.SendChange(-1);

            //가려놨던 캐릭터 표기하기
            PlayerController.LocalInstance.charBuilder.SetAlphaValue(1f);
        }
    }

    /// <summary>
    /// 닉네임 얻기
    /// </summary>
    /// <returns></returns>
    public string GetNickname() 
    {
        //서버에 등록된 닉네임 반환
        return PhotonNetwork.NickName;
    }

    /// <summary>
    ///  다음 턴으로 넘기기
    /// </summary>
    public void NextTurn()
    {
        //공격시 들어났던 캐릭터를 숨겨주고
        //그림자를 남겨 1부터 시작하게 해준다.

        //모든 버튼 비활성화
        MapCotroller.Instance.OffAllSlot();

        currTurn += 1;
        actionIndex = -1;

        if (currTurn >= maxTurn)
            currTurn = 0;

        ChangeTurn.Invoke();

        //내 턴과 현재 턴 동일할 경우 진행하기
        if (currTurn.Equals(myTurn))
        {
            UIGameManager.Instance.skipButton.SetActive(true);
            NextAction();
        }
    }

    /// <summary>
    /// 다음 액션으로 넘기기
    /// </summary>
    public void NextAction()
    {
        //내 턴과 현재 턴 동일할 경우 진행하기
        if (!currTurn.Equals(myTurn)) return;

        UIGameManager.Instance.ShowActionIcon(++actionIndex);

        Debug.Log("[On Action] "+actionIndex);

        //넘기기버튼 활성화
        switch (actionIndex) 
            {
                case 0://이동
                //내 위치 기준으로 버튼 열기
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.Around1);
                    break;

                case 1://공격
                //내 위치 빼고 전체 버튼 열기
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.All);
                    break;

                case 2://스킬                
                //가진 스킬을 기반으로 사용여부 체크
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.Random);
                    break;
            }
        
    }

    /// <summary>
    /// 받아온 값 적용하기
    /// </summary>
    /// <param name="value"></param>
    public void Action(int value) 
    {
        //내 턴과 현재 턴 동일할 경우 진행하기
        if (!currTurn.Equals(myTurn)) return;

        if (value.Equals(-1))
        {
            //턴 넘기기
            PlayerController.LocalInstance.SendChange(-1);
        }
        else if (actionIndex.Equals(2))
        {
            //스킬 구현
        }
        else 
        {
            PlayerController.LocalInstance.SendChange(actionIndex, value);
            NextAction();
        }
    }

}
