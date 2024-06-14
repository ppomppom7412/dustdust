using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;


namespace YellowGreen.BackStep
{
    [System.Serializable]
    public class PopData
    {
        public bool isBackInput; //���ư �Է� ���� ���� (true > onback)
        public PopupManager.PopState setState; //������ ���� / �������� �켱������ ������.
        public System.Action<bool> backAct; //������ �뿡 ���� �ٸ� ���

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
        //����  /None�� �ƹ� �˾��� �������� ��� /Pause�� ������ ������Ѵ�.
        public enum PopState { None, Play, Idle, Stop, Pause }
        PopState currState = PopState.None;

        [SerializeField]
        Stack<PopData> popupStack;
        PopData blockData;

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.Backspace)) 
            {
                //��������â ����
                if ((popupStack ?? new Stack<PopData>()).Count < 1)
                {
                    //"PLAY"���� ����

                }
                //���ư ������
                else
                {
                    //back��ư���� �ִ� �۵�����
                    if (popupStack.Peek().isBackInput)
                        ClosePop();
                }
            }
        }

        #region plus mius

        /// <summary>
        /// �˾��� ���� ���� ��ȭ ����
        /// </summary>
        /// <param name="state"></param>
        void SetState(PopState state)
        {
            if (currState.Equals(state))
                return;

            //���� �ܰ��
            if (currState < state)
            {
                //���߱�
                if (state.Equals(PopState.Pause))
                    Time.timeScale = 0;
            }
            //���� �ܰ��
            else if (currState > state)
            {
                //���߱�
                if (currState.Equals(PopState.Pause))
                    Time.timeScale = 0;
            }
        }

        /// <summary>
        /// �˾��� ���� �� �׾Ƶδ� ���ÿ� �߰�
        /// </summary>
        /// <param name="act">���� �� ��µǴ� �׼� ������ ������ �ִ�.</param>
        /// <param name="state">���¿� ���� ������</param>
        /// <param name="isback">���ư�� ���� ���� ����</param>
        /// <param name="name">������ �ʿ��� �̸�</param>
        public void AddPop(System.Action<bool> act, PopState state, bool isback = true)
        {
            if (popupStack == null)
                popupStack = new Stack<PopData>();

            PopData newPop = new PopData(act, state, isback);

            //���ο� ������ ���� ���ú��� ���°� ������ ����ȭ
            if (currState > state)
                newPop.setState = currState;

            popupStack.Push(newPop);

            if (currState < state) 
                SetState(state);
        }

        /// <summary>
        ///  �ٸ� ������ ���� �뵵 / �������θ� ���� �� ����
        /// </summary>
        public void AddInputBlock() 
        {
            if (blockData == null)
                blockData = new PopData((onoff) => { }, currState, false);

            blockData.setState = currState;

            popupStack.Push(blockData);
        }

        /// <summary>
        /// ��� �˾��� NO�� �ݴ�
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

            //�Է¸������� Ȯ��
            if (blockData != null && popupStack.Peek().Equals(blockData))
                return;

            //�ݱ� �׼� ����
            popupStack.Pop().backAct(false);

            //���� ������ �ְ� ���°� �ٸ��ٸ� �����ϱ�
            if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                SetState(popupStack.Peek().setState);
        }

        /// <summary>
        /// ��� �˾��� Yes�� �ݴ�.
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

            //�Է¸������� Ȯ��
            if (blockData != null && popupStack.Peek().Equals(blockData))
                return;

            //�ݱ� �׼� ����
            popupStack.Pop().backAct(true);

            //���� ������ �ְ� ���°� �ٸ��ٸ� �����ϱ�
            if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                SetState(popupStack.Peek().setState);
        }

        /// <summary>
        /// ��� �˾��� �׼��� �������� �ʰ� �����
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

            //�ݱ� �׼� ����
            popupStack.Pop();

            //���� ������ �ְ� ���°� �ٸ��ٸ� �����ϱ�
            if (popupStack.Count > 0 && currState > popupStack.Peek().setState)
                SetState(popupStack.Peek().setState);
        }

        #endregion

    }


}
