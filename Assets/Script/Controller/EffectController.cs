using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton; //MonoSingleton
using AYellowpaper.SerializedCollections; //SerializedDictionary
using YellowGreen.ObjectPool; //OBPObject, ObjectPool, OBPPart
using UnityEngine.Events;

public class EffectController : MonoSingleton<EffectController>
{
    #region monobehaviour

    private void Start()
    {
        ShowBGM(SoundEnum.BGM_Main);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
            ShowBGM((SoundEnum)Random.Range(0, 4));

        if (Input.GetKeyUp(KeyCode.Space))
            ShowSfx((SoundEnum)Random.Range(4, 16),Vector3.zero); 
    }

    #endregion

    #region sound

    public enum SoundEnum
    {
        BGM_Main,
        BGM_Lobby,
        BGM_Play1,
        BGM_Play2,
        UI_Click,
        UI_Close,
        UI_Open,
        Win,
        Lose,
        Death,//Á×À½
        Smoke,
        Jump,
        Slash,//ÈÖµÎ¸£±â
        Shot,//½î±â
        Hit, //¸Â±â
        Eat,
    }

    [NaughtyAttributes.Foldout("Sound Elements")]
    public float bgmVolume =1f;
    [NaughtyAttributes.Foldout("Sound Elements")]
    public bool bgmMute = false;
    [NaughtyAttributes.Foldout("Sound Elements")]
    public float sfxVolume =1f;
    [NaughtyAttributes.Foldout("Sound Elements")]
    public bool sfxMute = false;
    [NaughtyAttributes.Foldout("Sound Elements")]
    public OBPSound bgmObject;

    [NaughtyAttributes.Foldout("Sound Elements")]
    public UnityEvent ChangeSound;
    [NaughtyAttributes.Foldout("Sound Elements")]
    public UnityEvent ChangeBGM;

    [SerializedDictionary("Tag", "SoundData")]
    public SerializedDictionary<SoundEnum, ClipData> sfxDict;
    [SerializedDictionary("Channel", "Pool")]
    public SerializedDictionary<int, ObjectPool<OBPSound>> sfxPool;

    public void ShowSfx(SoundEnum id, Vector3 pos)
    {
        if (!(sfxDict ?? new SerializedDictionary<SoundEnum, ClipData>()).ContainsKey(id))
            return;

        if (!(sfxPool ?? new SerializedDictionary<int, ObjectPool<OBPSound>>()).ContainsKey(sfxDict[id].channel))
            return;

        OBPSound getefx = sfxPool[sfxDict[id].channel].GetPoolObject() as OBPSound;

        if (getefx!= null)
            getefx.PlaySound(sfxDict[id].clip, pos, sfxVolume, sfxMute, true);
    }

    public void ShowBGM(SoundEnum id)
    {
        if (!(sfxDict ?? new SerializedDictionary<SoundEnum, ClipData>()).ContainsKey(id))
            return;

        if (bgmObject != null)
            bgmObject.PlayBGM(sfxDict[id].clip, bgmVolume, bgmMute);
    }

    [NaughtyAttributes.Button]
    public void ChangeCallSfx() 
    {
        if (ChangeSound != null)
            ChangeSound.Invoke();
    }

    [NaughtyAttributes.Button]
    public void ChangeCallBGM()
    {
        if (ChangeBGM != null)
            ChangeBGM.Invoke();
    }

    #endregion

    #region Efx 

    public enum EffectEnum
    {
        Smoke,
        Jump,
        Hit
    }

    [SerializedDictionary("Tag", "Pool")]
    public SerializedDictionary<EffectEnum, ObjectPool<OBPPart>> efxPool;

    public void ShowEfx(EffectEnum id, Vector3 pos)
    {
        if (!(efxPool ?? new SerializedDictionary<EffectEnum, ObjectPool<OBPPart>>()).ContainsKey(id))
            return;

        OBPObject<OBPPart> getefx = efxPool[id].GetPoolObject();

        if (getefx != null)
            getefx.Active(pos);
    }

    #endregion

}

[System.Serializable]
public class ClipData 
{
    public int channel;
    public AudioClip clip;
}

public class BoolUnityEvent : UnityEvent<bool>
{

}