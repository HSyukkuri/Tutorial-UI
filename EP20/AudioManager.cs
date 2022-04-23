using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public float masterVolumePercent { get; private set; } = 1f;
    public float sfxVolumePercent { get; private set; } = 1f;
    public float systemVolumePercent { get; private set; } = 1f;
    public float musicVolumePercent { get; private set; } = 1f;

    AudioSource[] musicSource;
    int index_ActiveSource = 0;

    public static AudioManager instance;

    public AudioClip bgm_Normal;
    public AudioClip bgm_Battle;
    public AudioClip sfx_Attack;
    public AudioClip sfx_Move;
    public AudioClip sys_Click;
    public AudioClip sys_Success;
    public AudioClip sys_Fail;
    public AudioClip sys_GameOver;

    private void Awake() {

        if(instance != null) {
            Destroy(this);
        }
        else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            musicSource = new AudioSource[2];

            for (int i = 0; i < 2; i++) {
                GameObject newMusicSource = new GameObject("Music source" + i);
                musicSource[i] = newMusicSource.AddComponent<AudioSource>();
                musicSource[i].loop = true;
                newMusicSource.transform.parent = transform;
            }




        }

        
    }

    public void SetVol_SFX(float percent) {
        sfxVolumePercent = percent;
    }

    public void SetVol_System(float percent) {
        systemVolumePercent = percent;
    }

    public void SetVol_Music(float percent) {
        musicVolumePercent = percent;
        musicSource[0].volume = musicVolumePercent * masterVolumePercent;
        musicSource[1].volume = musicVolumePercent * masterVolumePercent;
    }

    public void SetVol_Master(float percent) {
        masterVolumePercent = percent;
        musicSource[0].volume = musicVolumePercent * masterVolumePercent;
        musicSource[1].volume = musicVolumePercent * masterVolumePercent;
    }

    public void Play_System(AudioClip clip) {
        AudioSource.PlayClipAtPoint(clip,transform.position,systemVolumePercent * masterVolumePercent);
    }

    public void Play_SFX(AudioClip clip) {
        AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolumePercent * masterVolumePercent);
    }

    public void Play_Music(AudioClip clip,float fadeTime = 1) {

        if(musicSource[index_ActiveSource].clip == clip) {
            return;
        }

        StartCoroutine(ChangeMusic(clip,fadeTime));
    }

    IEnumerator ChangeMusic(AudioClip clip, float fadeTime) {
        index_ActiveSource = 1 - index_ActiveSource;
        musicSource[index_ActiveSource].clip = clip;
        musicSource[index_ActiveSource].Play();

        float percent = 0;
        float currentVolume = musicVolumePercent * masterVolumePercent;

        while (percent < 1) {
            percent += Time.deltaTime / fadeTime;
            musicSource[index_ActiveSource].volume = Mathf.Lerp(0, currentVolume, percent);
            musicSource[1 - index_ActiveSource].volume = Mathf.Lerp(currentVolume, 0, percent);
            yield return null;
        }
        musicSource[1 - index_ActiveSource].clip = null;

    }
}
