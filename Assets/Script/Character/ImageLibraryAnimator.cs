using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ImageLibraryAnimator : MonoBehaviour
{
    //캐릭터 모션 리스트
    //public enum MotionList {Attack, Block, Death, Idle, Jab, Jump, Ready, Run, Crawl, Push }
    public enum MotionList { Slash, Shot, Roll, Block, Climb, Death, Idle, Jab, Jump, Ready, Run, Crawl, Push,  Hurt }

    [Header("Components")]
    public UnityEngine.UI.Image image;
    public SpriteLibrary library;
    //public CharacterSpriteBulider builder;

    [Header("SpriteLibrary Data")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<string, int> cateDictionary;

    const string defaultState ="Idle";
    int index = 0;

    IEnumerator playRoutine;

    public void Start()
    {
        library = GetComponent<SpriteLibrary>();
        //builder = GetComponent<CharacterSpriteBulider>();
        image = GetComponent<UnityEngine.UI.Image>();
    }

    public void OnEnable()
    {
        if (library == null)
            library = GetComponent<SpriteLibrary>();
        if (image == null)  
            image = GetComponent<UnityEngine.UI.Image>();

        //PlayAnim(defaultState, true);
    }

    [NaughtyAttributes.Button]
    public void InitSprite() 
    {
        image.sprite = library.GetSprite(defaultState, "0");
    }

    [NaughtyAttributes.Button]
    public void Play()
    {
        PlayAnim(defaultState);
    }

    /// <summary>
    /// 상태를 기반으로 애님 출력
    /// </summary>
    /// <param name="state">플레이할 애님 상태</param>
    /// <param name="loop">루프 여부</param>
    /// <param name="frame">프레임 당 시간</param>
    public void PlayAnim(string state, bool loop = false, float playtime=10f, float frame= 0.2f, bool redefault = true )
    {
        if (!gameObject.activeSelf)
            return;

        //없는 카테고리를 불렀을 경우 제외
        if (!cateDictionary.ContainsKey(state))
            return;

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = PlayLibrary(state, loop, playtime, frame, redefault);
        StartCoroutine(playRoutine); 
    }

    /// <summary>
    /// 코루틴으로 관리
    /// </summary>
    /// <param name="state"></param>
    /// <param name="loop"></param>
    /// <param name="frame"></param>
    /// <returns></returns>
    IEnumerator PlayLibrary(string state, bool loop, float playtime, float frame, bool redefault) 
    {
        WaitForSeconds waitTime = new WaitForSeconds(frame);
        index = 0;

        while (gameObject.activeSelf)
        {
            image.sprite = library.GetSprite(state, index.ToString());

            yield return waitTime;

            playtime -= frame;

            if (playtime <= 0f)
                break;

            if (++index >= cateDictionary[state]) 
            {
                if (loop) 
                    index = 0;
                else
                    break;
            }
        }

        //기본 상태로 돌리기
        if (redefault)
            PlayAnim(defaultState,true);
    }

}
