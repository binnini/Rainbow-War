using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Manager : MonoBehaviour
{
    public enum BGMName
    {
        Lobby, Ingame, Stop
    }

    public enum SFXName
    {
        Walk, Dash, Shot, Victory, Defeat, StopMove, StopGun, Button, CountDown, Reload, Teleport, Hit, Painting, StopPainting, Respawning, Respawn, GetPaint, Dead, Success, Fail
    }

    private Setting_Data set_data;
    private AudioSource[] audio_sources;

    public Scrollbar master_scroll;
    public Scrollbar bgm_scroll;
    public Scrollbar sfx_scroll;

    public AudioClip lobby_clip;
    public AudioClip ingame_clip;
    public AudioClip dash_clip;
    public AudioClip walk_clip;
    public AudioClip teleport_clip;
    public AudioClip shot_clip;
    public AudioClip victory_clip;
    public AudioClip defeat_clip;
    public AudioClip button_clip;
    public AudioClip countDown_clip;
    public AudioClip reload_clip;
    public AudioClip painting_clip;
    public AudioClip hit_clip;
    public AudioClip respawning_clip;
    public AudioClip respawn_clip;
    public AudioClip getPaint_clip;
    public AudioClip dead_clip;
    public AudioClip success_clip;
    public AudioClip fail_clip;

    public void SetMasterVolume(float value)
    {
        set_data.master_volume = value;

        audio_sources[0].volume = set_data.bgm_volume * set_data.master_volume;
        audio_sources[1].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[2].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[3].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[4].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[5].volume = set_data.sfx_volume * set_data.master_volume;
    }
    public void SetBGMVolume(float value)
    {
        set_data.bgm_volume = value;

        audio_sources[0].volume = set_data.bgm_volume * set_data.master_volume;
    }
    public void SetSFXVolume(float value)
    {
        set_data.sfx_volume = value;

        audio_sources[1].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[2].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[3].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[4].volume = set_data.sfx_volume * set_data.master_volume;
        audio_sources[5].volume = set_data.sfx_volume * set_data.master_volume;
    }

    public void BGMPlay(BGMName name)
    {
        if(name == BGMName.Lobby)
        {
            audio_sources[0].Stop();

            //audio_sources[0].volume = set_data.bgm_volume;
            audio_sources[0].clip = lobby_clip;

            audio_sources[0].Play();
        } 
        else if(name == BGMName.Ingame)
        {
            audio_sources[0].Stop();

            audio_sources[0].loop = true;
            //audio_sources[0].volume = set_data.bgm_volume;
            audio_sources[0].clip = ingame_clip;

            audio_sources[0].Play();
        }
        else if(name == BGMName.Stop)
        {
            audio_sources[0].Stop();
        }
    }

    public void SFXPlay(SFXName name)
    {
        if (name == SFXName.Dash)
        {
            audio_sources[1].Stop();

            //audio_sources[1].volume = set_data.sfx_volume;
            audio_sources[1].clip = dash_clip;

            audio_sources[1].Play();
        }
        else if (name == SFXName.Walk)
        {
            audio_sources[1].Stop();

            //audio_sources[1].volume = set_data.sfx_volume;
            audio_sources[1].clip = walk_clip;

            audio_sources[1].Play();
        }
        else if (name == SFXName.Dead)
        {
            audio_sources[1].Stop();

            //audio_sources[1].volume = set_data.sfx_volume;
            audio_sources[1].clip = dead_clip;

            audio_sources[1].Play();
        }
        else if (name == SFXName.Teleport)
        {
            audio_sources[5].Stop();

            //audio_sources[3].volume = set_data.sfx_volume;
            audio_sources[5].clip = teleport_clip;

            audio_sources[5].Play();
        }
        else if (name == SFXName.StopMove)
        {
            audio_sources[1].Stop();
        }
        else if(name == SFXName.Shot)
        {
            audio_sources[2].Stop();

            //audio_sources[2].volume = set_data.sfx_volume;
            audio_sources[2].clip = shot_clip;

            audio_sources[2].Play();
        }
        else if (name == SFXName.Reload)
        {
            audio_sources[2].Stop();

            //audio_sources[2].volume = set_data.sfx_volume;
            audio_sources[2].clip = reload_clip;

            audio_sources[2].Play();
        }
        else if (name == SFXName.StopGun)
        {
            audio_sources[2].Stop();
        }
        else if (name == SFXName.Victory)
        {
            audio_sources[3].Stop();

            //audio_sources[3].volume = set_data.sfx_volume;
            audio_sources[3].clip = victory_clip;

            audio_sources[3].Play();
        }
        else if (name == SFXName.Defeat)
        {
            audio_sources[3].Stop();

            //audio_sources[3].volume = set_data.sfx_volume;
            audio_sources[3].clip = defeat_clip;

            audio_sources[3].Play();
        }
        else if (name == SFXName.Button)
        {
            audio_sources[3].Stop();

            //audio_sources[3].volume = set_data.sfx_volume;
            audio_sources[3].clip = button_clip;

            audio_sources[3].Play();
        }
        else if (name == SFXName.CountDown)
        {
            audio_sources[3].Stop();

            //audio_sources[3].volume = set_data.sfx_volume;
            audio_sources[3].clip = countDown_clip;

            audio_sources[3].Play();
        }
        else if (name == SFXName.GetPaint)
        {
            audio_sources[3].Stop();

            //audio_sources[3].volume = set_data.sfx_volume;
            audio_sources[3].clip = getPaint_clip;

            audio_sources[3].Play();
        }
        else if (name == SFXName.Painting)
        {
            audio_sources[4].Stop();

            //audio_sources[4].volume = set_data.sfx_volume;
            audio_sources[4].clip = painting_clip;
            audio_sources[4].loop = true;
            audio_sources[4].Play();
        }
        else if (name == SFXName.Success)
        {
            audio_sources[4].Stop();

            //audio_sources[4].volume = set_data.sfx_volume;
            audio_sources[4].clip = success_clip;
            audio_sources[4].loop = false;
            audio_sources[4].Play();
        }
        else if (name == SFXName.Fail)
        {
            audio_sources[4].Stop();

            //audio_sources[4].volume = set_data.sfx_volume;
            audio_sources[4].clip = fail_clip;
            audio_sources[4].loop = false;
            audio_sources[4].Play();
        }
        else if (name == SFXName.StopPainting)
        {
            audio_sources[4].Stop();
        }
        else if (name == SFXName.Hit)
        {
            audio_sources[5].Stop();

            //audio_sources[5].volume = set_data.sfx_volume;
            audio_sources[5].clip = hit_clip;

            audio_sources[5].Play();
        }
        else if (name == SFXName.Respawning)
        {
            audio_sources[5].Stop();

            //audio_sources[5].volume = set_data.sfx_volume;
            audio_sources[5].clip = respawning_clip;

            audio_sources[5].Play();
        }
        else if (name == SFXName.Respawn)
        {
            audio_sources[5].Stop();

            //audio_sources[5].volume = set_data.sfx_volume;
            audio_sources[5].clip = respawn_clip;

            audio_sources[5].Play();
        }

    }

    public void ButtonSFX()
    {
        SFXPlay(SFXName.Button);
    }

    private void Awake()
    {
        audio_sources = GetComponents<AudioSource>();        
        set_data = Setting_Data.instance.GetComponent<Setting_Data>();
        Debug.Log(set_data != null);
    }
    void Start()
    {
        SetMasterVolume(set_data.master_volume);
        SetBGMVolume(set_data.bgm_volume);
        SetSFXVolume(set_data.sfx_volume);

        if (master_scroll != null)
        {
            master_scroll.value = set_data.master_volume;
        }
        if (bgm_scroll != null)
        {
            bgm_scroll.value = set_data.bgm_volume;
        }
        if (sfx_scroll != null)
        {
            sfx_scroll.value = set_data.sfx_volume;
        }

        audio_sources[0].loop = true;
        audio_sources[1].loop = true;
        audio_sources[2].loop = false;
        audio_sources[3].loop = false;
        audio_sources[4].loop = true;
        audio_sources[5].loop = false;
    }

    void Update()
    {

        if (master_scroll != null && master_scroll.IsActive())
        {
            set_data.master_volume = master_scroll.value;
            SetMasterVolume(master_scroll.value);
        }
        if (bgm_scroll != null && bgm_scroll.IsActive())
        {
            set_data.bgm_volume = bgm_scroll.value;
            SetBGMVolume(bgm_scroll.value);
        }
        if (sfx_scroll != null && sfx_scroll.IsActive())
        {
            set_data.sfx_volume = sfx_scroll.value;
            SetSFXVolume(sfx_scroll.value);
        }
    }
}
