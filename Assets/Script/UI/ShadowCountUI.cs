using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowCountUI : MonoBehaviour
{
    public Text countTxt;
    public Image mainImg;
    public Image frameImg;

    const int maxCount = 5;
    int currCount = 0;

    //alpha 값 제어용
    Color curColor;
    float alphatime = 1.5f;
    IEnumerator alphaRtn;
    MapSlot curSlot;

    private void Start()
    {
        GameManager.Instance.ChangeTurn.AddListener(UpCount);
    }

    //private void OnDestroy()
    //{
    //    GameManager.Instance.ChangeTurn.RemoveListener(UpCount);
    //}

    /// <summary>
    /// 해당 UI 이동 호출
    /// </summary>
    /// <param name="targetslot"></param>
    public void ShowCount(int index) 
    {
        currCount = 0;

        if (countTxt != null)
            countTxt.text = currCount.ToString();

        transform.position = MapCotroller.Instance.GetSlotPosition(index);

        TransAlphaValue(1);
    }

    public void OffCount() 
    {
        if (alphaRtn != null)
            StopCoroutine(alphaRtn);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 턴이 넘어감에 따른 호출
    /// </summary>
    /// <returns></returns>
    public void UpCount() 
    {
        currCount += 1;

        if (currCount < maxCount)
        {
            if (countTxt != null)
                countTxt.text = currCount.ToString();
        }
        else 
        {
            TransAlphaValue(0);
        }
    }

    //서서히 드러나기/사라지기
    void TransAlphaValue(float to) 
    {
        if (mainImg == null || frameImg == null) return;

        if (alphaRtn != null)
            StopCoroutine(alphaRtn);

        //메인 이미지의 알파값 조정
        curColor = mainImg.color;

        if (to > 0)
            curColor.a = 0f;
        else
            curColor.a = 1f;

        mainImg.color = curColor;

        //프레임도
        curColor = frameImg.color;
        curColor.a = mainImg.color.a;
        frameImg.color = curColor;

        //글자도
        curColor = countTxt.color;
        curColor.a = mainImg.color.a;
        countTxt.color = curColor;

        gameObject.SetActive(true);

        alphaRtn = TransAlphaRoutine(to, alphatime);
        StartCoroutine(alphaRtn);
    }

    //TransAlphaValue를 보조하는 코루틴
    IEnumerator TransAlphaRoutine(float to, float time) 
    {
        WaitForSeconds waittime = new WaitForSeconds(0.2f);
        float timealpha_value = 1f / time * 0.2f;

        while (curColor.a != to) 
        {
            yield return waittime;

            if (to > 0)
                curColor.a -= timealpha_value;
            else
                curColor.a += timealpha_value;

            if (curColor.a < 0)
                curColor.a = 0;
            else if (curColor.a > 1f)
                curColor.a = 1f;

            mainImg.color = curColor;

            curColor = frameImg.color;
            curColor.a = mainImg.color.a;
            frameImg.color = curColor;

            curColor = countTxt.color;
            curColor.a = mainImg.color.a;
            countTxt.color = curColor;
        }

        if (to <= 0)
            gameObject.SetActive(false);
    }

}
