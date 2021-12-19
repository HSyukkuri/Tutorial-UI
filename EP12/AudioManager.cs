using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public float masterVolumePercent { get; private set; } = 1f;
    public float sfxVolumePercent { get; private set; } = 1f;
    public float systemVolumePercent { get; private set; } = 1f;
    public float musicVolumePercent { get; private set; } = 1f;

    AudioSource musicSource;

    public static AudioManager instance;

    public AudioClip bgm_Normal;
    public AudioClip bgm_Battle;
    public AudioClip sfx_Attack;
    public AudioClip sys_Click;
    public AudioClip sys_Success;
    public AudioClip sys_Fail;

    private void Awake() {

        if(instance != null) {
            Destroy(this);
        }
        else {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            GameObject newMusicSource = new GameObject("Music source");
            musicSource = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
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
        musicSource.volume = musicVolumePercent * masterVolumePercent;
    }

    public void SetVol_Master(float percent) {
        masterVolumePercent = percent;
        musicSource.volume = musicVolumePercent * masterVolumePercent;
    }

    public void Play_System(AudioClip clip) {
        AudioSource.PlayClipAtPoint(clip,transform.position,systemVolumePercent * masterVolumePercent);
    }

    public void Play_SFX(AudioClip clip) {
        AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolumePercent * masterVolumePercent);
    }

    public void Play_Music(AudioClip clip) {

        if(musicSource.clip == clip) {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }
}
