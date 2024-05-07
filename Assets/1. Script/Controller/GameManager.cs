using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Singleton;
using Photon.Pun;

public class GameManager : MonoSingleton<GameManager>
{
    //플레이어 전용 프리팹
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

        CreatePlayerPrefab();

        currTurn = -1;
        actionIndex = 0;

        //나의 턴 순서 찾기
        for (int i = 0; i < PunManager.Instance.curPlayers.Count; ++i)
        {
            if (PunManager.Instance.curPlayers[i].Equals(PunManager.Instance.myPlayer))
                myTurn = i;
        }

        //닉네임 설정하기
        UIGameManager.Instance.SetNicknameText();

        //게임 시작
        Invoke("NextTurn", 0.5f);
    }

    /// <summary>
    /// 플레이어 프리팹 만들기
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
        //카운트 다운 비활성화
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
    /// 다음 액션으로 넘기기
    /// </summary>
    public void NextAction()
    {
        if (actionIndex.Equals(0))
        {
            //카운트 다운 활성화
            countDownCrtn = NextCountDown(waittime);
            StartCoroutine(countDownCrtn);
        }

        // 0과 1만 반복하는 나머지
        //내 턴과 현재 턴의 나머지값이 동일할 경우 진행하기
        if (currTurn % 2 != myTurn) return;
        
        //넘기기버튼 활성화
            switch (actionIndex) 
            {
                case 0://이동
                //내 위치 기준으로 버튼 열기
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.Around1);
                    break;

                case 1://공격
                //내 위치 빼고 전체 버튼 열기
                MapCotroller.Instance.OnSlots(PlayerController.LocalInstance.curpos, MapCotroller.SlotShape.NotTarget);
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
        // 0과 1만 반복하는 나머지
        //내 턴과 현재 턴의 나머지값이 동일할 경우 진행하기
        if (currTurn % 2 != myTurn) return;

        if (actionIndex.Equals(2))
        {
            //스킬 구현
        }
        else 
        {
            PlayerController.LocalInstance.SendChange(actionIndex, value);
        }
    }

    /// <summary>
    /// 남은 시간 카운트 다운
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator NextCountDown(float time) 
    {
        WaitForSeconds wait1f = new WaitForSeconds(1f);

        while (time > 0) 
        {
            //UI로 시간초 지남을 보여주기
            yield return wait1f;
        }

        //시간초 지나면 턴 넘기기
        NextTurn();
    }

}
