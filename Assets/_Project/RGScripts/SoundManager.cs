using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* 
 * 2024. 09. 21 Created by Pumpkin
 */

public class SoundManager : MonoBehaviour
{
    #region Singleton
    private static SoundManager _instance = null;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                return null;
            return _instance;
        }
    }
    #endregion

    public enum eSoundGroup
    {
        Master = 0,
        BGM,
        SE,
        SE_Footstep,
    }

    public enum eSoundChannel
    {
        SE01 = 0,
        SE02,
        SE_Charge,
        End
    }

    public enum eSoundEffect
    {
        Footstep1 = 0,
        Footstep1_Reverb,
        Footstep1_Underwater,
        Footstep1_Underwater_Reverb,
        Footstep2,
        Footstep2_Reverb,
        Footstep2_Underwater,
        Footstep2_Underwater_Reverb,
        Footstep_Gravel_Heavy1,
        Footstep_Gravel_Heavy1_Reverb,
        Footstep_Gravel_Heavy2,
        Footstep_Gravel_Heavy2_Reverb,
        Footstep_Gravel_Light1,
        Footstep_Gravel_Light1_Reverb,
        Footstep_Gravel_Light2,
        Footstep_Gravel_Light2_Reverb,
        Hurt,
        Hurt_Reverb,
        Jump,
        Jump_Reverb,
        Land,
        Land_Reverb,
        SolarCharge,
        SolarCharge_Reverb,
        Talk_Placeholder,
        Timer,
    }

    #region Variables
    const float MIN_VOLUME = -30f;
    const float MAX_VOLUME = 0f;

    public float _footstepPitch = 0f;

    public AudioMixer _audioMixer = null;
    public AudioMixerGroup _groupBGM = null;
    public AudioMixerGroup _groupSE = null;
    public AudioMixerGroup _groupSEFootstep = null;
    private AudioSource _bgmSource = null;
    private AudioSource _footstepSource = null;
    private List<AudioSource> _listSESource = null;

    public List<AudioClip> _listAudioClip = null;
    private Dictionary<string, AudioClip> _dicAudioClip = null;

    string TitleBGM = "Music - Title (WAV)";
    string TutorialBGM = "Music - Tutorial (WAV)";
    string LevelOneBGM = string.Empty;
    string LevelTwoBGM = string.Empty;
    string LevelThreeBGM = string.Empty;
    string EndingBGM = string.Empty;

    private string _currentSceneBGM = null;
    private bool _isSoundLoading = false;
    #endregion


    #region Unity Event Functions
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _bgmSource = gameObject.AddComponent<AudioSource>();
        if (_bgmSource)
        {
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = true;
            _bgmSource.outputAudioMixerGroup = _groupBGM;
        }

        _footstepSource = gameObject.AddComponent<AudioSource>();
        if (_footstepSource)
        {
            _footstepSource.loop = true;
            _footstepSource.playOnAwake = false;
            _footstepSource.outputAudioMixerGroup = _groupSEFootstep;
        }

        _listSESource = new List<AudioSource>();
        for (int i = 0; i < (int)eSoundChannel.End; ++i)
        {
            _listSESource.Add(gameObject.AddComponent<AudioSource>());
            if (_listSESource[i])
            {
                _listSESource[i].playOnAwake = false;
                _listSESource[i].outputAudioMixerGroup = _groupSE;
            }
        }

        // TO_DO : This should be executed on the title screen or loading screen.
        _dicAudioClip = new Dictionary<string, AudioClip>();
        for (int i = 0; i < _listAudioClip.Count; ++i)
        {
            if (_listAudioClip[i])
                _dicAudioClip.Add(_listAudioClip[i].name, _listAudioClip[i]);
        }
        _isSoundLoading = true;
        UpdateBGMForCurrentScene();

        // TO_DO : Temporary code (Resource loading issues)
        GameObject obj = GameObject.Find("NewPlayer");
        if (obj)
        {
            PlayerMovement cs = obj.GetComponent<PlayerMovement>();
            if (cs)
                cs.SetFootstep();
        }

        SoundManager.Instance.PlaySESound(SoundManager.eSoundChannel.SE_Charge, SoundManager.eSoundEffect.SolarCharge);
        SoundManager.Instance.StopSESound(SoundManager.eSoundChannel.SE_Charge);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion


    public float GetVolume(eSoundGroup type)
    {
        float volume = 0f;
        _audioMixer.GetFloat(type.ToString(), out volume);
        return VolumeToFloat(volume);
    }

    public void SetVolume(eSoundGroup type, float value)
    {
        if (_audioMixer == null)
            return;

        _audioMixer.SetFloat(type.ToString(), FloatToVolume(value));
    }

    public bool GetSoundLoadingFinish()
    {
        return _isSoundLoading;
    }

    public Dictionary<string, AudioClip> GetAudioDic()
    {
        return _dicAudioClip;
    }

    public void SetPause(eSoundGroup type)
    {
        switch (type)
        {
            case eSoundGroup.Master:
                break;

            case eSoundGroup.BGM:
                _bgmSource.Pause();
                break;

            case eSoundGroup.SE:
                for (int i = 0; i < (int)eSoundChannel.End; ++i)
                    _listSESource[i].Pause();
                break;

            case eSoundGroup.SE_Footstep:
                _footstepSource.Pause();
                break;
        }
    }

    public void SetUnPause(eSoundGroup type)
    {
        switch (type)
        {
            case eSoundGroup.Master:
                break;

            case eSoundGroup.BGM:
                _bgmSource.UnPause();
                break;

            case eSoundGroup.SE:
                for (int i = 0; i < (int)eSoundChannel.End; ++i)
                    _listSESource[i].UnPause();
                break;

            case eSoundGroup.SE_Footstep:
                _footstepSource.UnPause();
                break;
        }
    }

    public void SetPauseSEFootstepChannel()
    {
        if (_footstepSource)
            _footstepSource.Pause();
    }

    public void SetUnPauseSEFootstepChannel()
    {
        if (_footstepSource)
        {
            _footstepSource.pitch = _footstepPitch;
            _footstepSource.UnPause();
        }
    }

    public void SetPauseBySEChannel(eSoundChannel type)
    {
        if (_listSESource[(int)type])
            _listSESource[(int)type].Pause();
    }

    public void SetUnPauseBySEChannel(eSoundChannel type)
    {
        if (_listSESource[(int)type])
            _listSESource[(int)type].UnPause();
    }

    private float FloatToVolume(float value)
    {
        if (value <= 0f)
            return -80f;
        return MIN_VOLUME + ((MAX_VOLUME - MIN_VOLUME) * value);
    }

    private float VolumeToFloat(float volume)
    {
        return (volume - MIN_VOLUME) / (MAX_VOLUME - MIN_VOLUME);
    }

    private string GetSEKeyFromEnum(eSoundEffect value)
    {
        switch (value)
        {
            case eSoundEffect.Footstep1:
                return "Footstep 1";
            case eSoundEffect.Footstep1_Reverb:
                return "Footstep 1 (Reverb)";
            case eSoundEffect.Footstep1_Underwater:
                return "Footstep 1 Underwater";
            case eSoundEffect.Footstep1_Underwater_Reverb:
                return "Footstep 1 Underwater (Reverb)";
            case eSoundEffect.Footstep2:
                return "Footstep 2";
            case eSoundEffect.Footstep2_Reverb:
                return "Footstep 2 (Reverb)";
            case eSoundEffect.Footstep2_Underwater:
                return "Footstep 2 Underwater";
            case eSoundEffect.Footstep2_Underwater_Reverb:
                return "Footstep 2 Underwater (Reverb)";
            case eSoundEffect.Footstep_Gravel_Heavy1:
                return "Footstep Gravel Heavy 1";
            case eSoundEffect.Footstep_Gravel_Heavy1_Reverb:
                return "Footstep Gravel Heavy 1 (Reverb)";
            case eSoundEffect.Footstep_Gravel_Heavy2:
                return "Footstep Gravel Heavy 2";
            case eSoundEffect.Footstep_Gravel_Heavy2_Reverb:
                return "Footstep Gravel Heavy 2 (Reverb)";
            case eSoundEffect.Footstep_Gravel_Light1:
                return "Footstep Gravel Light 1";
            case eSoundEffect.Footstep_Gravel_Light1_Reverb:
                return "Footstep Gravel Light 1 (Reverb)";
            case eSoundEffect.Footstep_Gravel_Light2:
                return "Footstep Gravel Light 2";
            case eSoundEffect.Footstep_Gravel_Light2_Reverb:
                return "Footstep Gravel Light 2 (Reverb)";
            case eSoundEffect.Hurt:
                return "Hurt";
            case eSoundEffect.Hurt_Reverb:
                return "Hurt (Reverb)";
            case eSoundEffect.Jump:
                return "Jump";
            case eSoundEffect.Jump_Reverb:
                return "Jump (Reverb)";
            case eSoundEffect.Land:
                return "Land";
            case eSoundEffect.Land_Reverb:
                return "Land (Reverb)";
            case eSoundEffect.SolarCharge:
                return "Solar Charge";
            case eSoundEffect.SolarCharge_Reverb:
                return "Solar Charge (Reverb)";
            case eSoundEffect.Talk_Placeholder:
                return "Talk (Placeholder)";
            case eSoundEffect.Timer:
                return "Timer";
        }

        return "";
    }

    /// <summary>
    /// BGM Change : Execute when moving the scene
    /// </summary>
    public void UpdateBGMForCurrentScene()
    {
        StopBGMSound();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("TitleScreen"))
            _currentSceneBGM = TitleBGM;
        else if (sceneName.Equals("RobotGameTutorial"))
            _currentSceneBGM = TutorialBGM;
        else if (sceneName.Equals("RobotGameLevel1"))
            _currentSceneBGM = LevelOneBGM;
        else if (sceneName.Equals("RobotGameLevel2"))
            _currentSceneBGM = LevelTwoBGM;
        else if (sceneName.Equals("RobotGameLevel3"))
            _currentSceneBGM = LevelThreeBGM;
        else if (sceneName.Equals("EndingScreen"))
            _currentSceneBGM = EndingBGM;

        PlayBGMSound(_currentSceneBGM);
    }

    public void PlayBGMSound(string key)
    {
        if (_bgmSource == null)
            return;

        if (_dicAudioClip.ContainsKey(key))
        {
            _bgmSource.clip = _dicAudioClip[key];
            _bgmSource.Play();
        }
    }

    public void StopBGMSound()
    {
        if (_bgmSource == null)
            return;

        _bgmSource.Stop();
    }

    public void PlaySEFootstepSound()
    {
        if (_footstepSource == null)
            return;

        string key = GetSEKeyFromEnum(eSoundEffect.Footstep1);
        if (_dicAudioClip.ContainsKey(key))
        {
            _footstepSource.clip = _dicAudioClip[key];
            _footstepSource.pitch = _footstepPitch;
            _footstepSource.loop = true;
            _footstepSource.Play();
        }
    }

    public void StopSEFootstepSound()
    {
        if (_footstepSource == null)
            return;

        _footstepSource.Stop();
    }

    public void PlaySESound(eSoundChannel channel, eSoundEffect value, bool loop = false)
    {
        if (_listSESource[(int)channel] == null)
            return;

        string key = GetSEKeyFromEnum(value);
        if (_dicAudioClip.ContainsKey(key))
        {
            _listSESource[(int)channel].clip = _dicAudioClip[key];

            _listSESource[(int)channel].loop = loop;
            _listSESource[(int)channel].Play();
        }
    }

    public void PlaySESoundForDebug(eSoundChannel channel, string key, bool loop = false)
    {
        if (_listSESource[(int)channel] == null)
            return;

        if (_dicAudioClip.ContainsKey(key))
        {
            _listSESource[(int)channel].clip = _dicAudioClip[key];
            _listSESource[(int)channel].loop = loop;
            _listSESource[(int)channel].Play();
        }
    }

    public void StopSESound(eSoundChannel channel)
    {
        if (_listSESource[(int)channel] == null)
            return;

        _listSESource[(int)channel].Stop();
    }

    public void EnableSEChannel(eSoundChannel channel, bool enable)
    {
        if (_listSESource[(int)channel] == null)
            return;

        _listSESource[(int)channel].enabled = enable;
    }
}