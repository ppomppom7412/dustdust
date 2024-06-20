using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;


namespace YellowGreen.Popup
{
    [System.Serializable]
    public class PopData
    {
        public bool isBackInput; //백버튼 입력 받음 여부 (true > onback)
        public PopupManager.PopState setState; //변경할 상태 / 높을수록 우선순위를 가진다.
        public System.Action<bool> backAct; //예스와 노에 따른 다른 출력

        public PopData()
        {
            isBackInput = true;
            setState = PopupManager.PopState.Idle; 
            backAct = (backswitch) =>
            {
                if (backswitch)
                    Debug.Log("OnBack true");
                else
                    Debug.Log("OnBack false");
            };
        }

        public PopData(System.Action<bool> act, PopupManager.PopState state, bool isinput = true)
        {
            isBackInput = isinput;
            setState = state;
            backAct = act;
        }

        public PopData(PopData copy)
        {
            isBackInput = copy.isBackInput;
            setState = copy.setState;
            backAct = copy.backAct;
        }
    }

    public class PopupManager : MonoSingleton<PopupManager>
    {
        //상태  /None은 아무 팝업이 없을때만 사용 /Pause시 게임이 멈춰야한다.
        public enum PopState { None, Play, Idle, Stop, Pause }
        PopState currState = PopState.None;

        [SerializeField]
        Stack<PopData> popupStack;
        PopData blockData;

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Backspace)) 
            {
                //게임종료창 띄우기
                if ((popupStack ?? new Stack<PopData>()).Count < 1)
                {
                    //"PLAY"에선 제외

                }
                //백버튼 눌리기
                else
                {
                    //back버튼에만 있는 작동여부
                    if (popupStack.Peek().isBackInput)
                        ClosePop();
                }
            }
        }

        #region plus mius

        /// <summary>
        /// 팝업에 따른 상태 변화 적용
        /// </summary>
        /// <param name="state"></param>
        void SetState(PopState state)
        {
            if (currState.Equals(state))
                return;

            //상위 단계로
            if (currState < state)
            {
                //멈추기
                if (state.Equals(PopState.Pause))
                    Time.timeScale = 0;
            }
            //하위 단계로
            else if (currState > state)
            {
                //멈추기
                if (currState.Equals(PopState.Pause))
                    Time.timeScale = 0;
            }
        }

        /// <summary>
        /// 팝업이 켜질 때 쌓아두는 스택에 추가
        /// </summary>
        /// <param name="act">닫힐 때 출력되는 액션 긍정과 부정이 있다.</param>
        /// <param name="state">상태에 따른 변경점</param>
        /// <param name="isback">백버튼에 대한 대응 여부</param>
        /// <param name="name">삭제에 필요한 이름</param>
        public void AddPop(System.Action<bool> act, PopState state, bool isback = true)
        {
            if (popupStack == null)
                popupStack = new Stack<PopData>();

            PopData newPop = new PopData(act, state, isback);

            //새로운 스택이 기존 스택보다 상태가 낮으면 동기화
            if (currState > state)
                newPop.setState = currState;

            popupStack.Push(newPop);

            if (currState < state) 
                SetState(state);
        }

        /// <summary>
        ///  다른 반응을 막는 용도 / 리무버로만 지울 수 있음
        /// </summary>
        public void AddInputBlock() 
        {
            if (blockData == null)
                blockData = new PopData((onoff) => { }, currState, false);

            blockData.setState = currState;

            popupStack.Push(blockData);
        }

        /// <summary>
        /// 상단 팝업을 NO로 닫다
        /// </summary>
        public void ClosePop() 
        {
            if (popupStack == null) 
            {
                popupStack = new Stack<PopData>();
                return;
            }

            if (popupStack.Count < 1)
                return;

            //입력막기인지 확인
            if (blockData != null && popupStack.Peek().Equals(blockData))
                return;

            //닫기 액션 실행
            popupStack.Pop().backAct(false);

            //남은 스택이 있고 상태가 다르다면 적용하기
            if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                SetState(popupStack.Peek().setState);
        }

        /// <summary>
        /// 상단 팝업을 Yes로 닫다
        /// </summary>
        public void AccessPop()
        {
            if (popupStack == null)
            {
                popupStack = new Stack<PopData>();
                return;
            }

            if (popupStack.Count < 1)
                return;

            //입력막기인지 확인
            if (blockData != null && popupStack.Peek().Equals(blockData))
                return;

            //닫기 액션 실행
            popupStack.Pop().backAct(true);

            //남은 스택이 있고 상태가 다르다면 적용하기
            if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                SetState(popupStack.Peek().setState);
        }

        /// <summary>
        /// 상단 팝업을 액션을 실행하지 않고 지우기
        /// </summary>
        public void RemovePop() 
        {
            if (popupStack == null)
            {
                popupStack = new Stack<PopData>();
                return;
            }

            if (popupStack.Count < 1)
                return;

            //닫기 액션 실행
            popupStack.Pop();

            //남은 스택이 있고 상태가 다르다면 적용하기
            if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                SetState(popupStack.Peek().setState);
        }

        /// <summary>
        /// 닫을 수 있는 상단 팝업 모두 NO로 닫다
        /// </summary>
        public void AllClosePop() 
        {
            if (popupStack == null)
            {
                popupStack = new Stack<PopData>();
                return;
            }

            if (popupStack.Count < 1)
                return;

            for (int i = popupStack.Count; i > 0; --i) 
            {
                //입력막기인지 확인
                if (blockData != null && popupStack.Peek().Equals(blockData))
                    return;

                //닫기 액션 실행
                popupStack.Pop().backAct(false);

                //남은 스택이 있고 상태가 다르다면 적용하기
                if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                    SetState(popupStack.Peek().setState);
            }
        }

        #endregion

    }


}
