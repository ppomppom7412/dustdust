using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YellowGreen.ObjectPool {
    public class OBPSound :OBPObject<OBPSound>
    {
        public AudioSource source;
        public bool isBGM = false;
        public bool isPlay = false;
        public float changetime = 1.5f;
        IEnumerator timer;
        bool isInit;

        #region action func

        public void PlaySound(AudioClip clip, Vector3 pos, float volume, bool ismute, bool isRandomPitch) 
        {
            if (!isInit)
                Initialize();

            if (isPlay)
                Stop();

            source.volume = volume;
            source.mute = ismute;
            source.loop = false;
            source.clip = clip;

            if (isRandomPitch)
                source.pitch = Random.Range(0.7f, 1.3f);
            else
                source.pitch = 1f;

            Play(pos);
        }

        public void PlayBGM(AudioClip clip, float volume, bool ismute)
        {
            if (!isInit)
            {
                Initialize();
                EffectController.Instance.ChangeBGM.AddListener(ChangeBGM);
            }

            if (isPlay)
                Stop();

            source.volume = volume;
            source.mute = ismute;
            source.loop = true;
            source.clip = clip;
            source.pitch = 1f;
            //DOTween.To(() => source.pitch, value => source.pitch = value, 1f, changetime);

            Play(Vector3.zero);
        }

        public void Stop()
        {
            if (!isPlay) return;

            source.Stop();
            isPlay = false;

            if (timer != null)
                StopCoroutine(timer);

            if (!isBGM)
                EffectController.Instance.ChangeSound.RemoveListener(ChangeSound);
        }

        private void Initialize()
        {
            if (source == null)
            {
                source = gameObject.GetComponent<AudioSource>();

                if (source == null)
                    source = gameObject.AddComponent<AudioSource>();
            }

            isInit = true;
        }

        void Play(Vector3 pos)
        {
            isPlay = true;
            Active(pos);

            source.Play();

            if (!isBGM)
            {
                EffectController.Instance.ChangeSound.AddListener(ChangeSound);

                timer = StopTimer(source.clip.length);
                StartCoroutine(timer);
            }
        }

        IEnumerator StopTimer(float time) 
        {
            yield return new WaitForSeconds(time+0.1f);

            gameObject.SetActive(false);
            isPlay = false;
        }

        #endregion

        #region event

        public void ChangeSound()
        {
            source.volume = EffectController.Instance.sfxVolume;
            source.mute = EffectController.Instance.sfxMute;
        }

        public void ChangeBGM()
        {
            source.volume = EffectController.Instance.bgmVolume;
            source.mute = EffectController.Instance.bgmMute;
        }

        public void Pause(bool pause)
        {
            if (!isPlay) return;

            if (pause)
            {
                source.Pause();
                StopCoroutine(timer);
            }
            else
            {
                source.UnPause();

                timer = StopTimer(source.clip.length - source.time);
                StopCoroutine(timer);
            }
        }

        #endregion
    }
}
