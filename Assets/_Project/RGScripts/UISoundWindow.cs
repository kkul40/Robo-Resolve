using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/* 
 * 2024. 09. 21 Created by Pumpkin
 */

public class UISoundWindow : UIWindow
{
    #region Variables
    public Toggle _toggleMaster = null;
    public Toggle _toggleBGM = null;
    public Toggle _toggleSE = null;

    public Slider _sliderMaster = null;
    public Slider _sliderBGM = null;
    public Slider _sliderSE = null;

    public Text _textMaster = null;
    public Text _textBGM = null;
    public Text _textSE = null;

    private bool _fromChild = false;

    [Header("Debug")]
    public Button _btnBGMPlay = null;
    public Button _btnBGMStop = null;
    public Button _btnSEPlay = null;
    public Button _btnSEStop = null;
    public TMPro.TMP_Dropdown _dropdownChannelList = null;
    public TMPro.TMP_Dropdown _dropdownAudioList = null;
    public Toggle _toggleLoop = null;
    #endregion

    #region Unity Event Functions
    protected override void Awake()
    {
        if (_btnBGMPlay)
            _btnBGMPlay.onClick.AddListener(OnBGMPlayButton);
        if (_btnBGMStop)
            _btnBGMStop.onClick.AddListener(BGMStopButton);
        if (_btnSEPlay)
            _btnSEPlay.onClick.AddListener(SEPlayButton);
        if (_btnSEStop)
            _btnSEStop.onClick.AddListener(SEStopButton);

        if (_toggleMaster)
            _toggleMaster.onValueChanged.AddListener(delegate { OnMasterToggleChanged(_toggleMaster); });
        if (_toggleBGM)
            _toggleBGM.onValueChanged.AddListener(delegate { OnBGMToggleChanged(_toggleBGM); });
        if (_toggleSE)
            _toggleSE.onValueChanged.AddListener(delegate { OnSEToggleChanged(_toggleSE); });

        float initMasterVolume = SoundManager.Instance.GetVolume(SoundManager.eSoundGroup.Master);
        float initBGMVolume = SoundManager.Instance.GetVolume(SoundManager.eSoundGroup.BGM);
        float initSEVolume = SoundManager.Instance.GetVolume(SoundManager.eSoundGroup.SE);

        if (_sliderMaster)
        {
            _sliderMaster.value = initMasterVolume;
            _sliderMaster.onValueChanged.AddListener(OnMasterSliderChanged);
        }
        if (_sliderBGM)
        {
            _sliderBGM.value = initBGMVolume;
            _sliderBGM.onValueChanged.AddListener(OnBGMSliderChanged);
        }
        if (_sliderSE)
        {
            _sliderSE.value = initSEVolume;
            _sliderSE.onValueChanged.AddListener(OnSESliderChanged);
        }

        if (_textMaster)
            _textMaster.text = SetVolumeText(initMasterVolume);
        if (_textBGM)
            _textBGM.text = SetVolumeText(initBGMVolume);
        if (_textSE)
            _textSE.text = SetVolumeText(initSEVolume);

        SetDropDownMenu();

        this.gameObject.SetActive(false);

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    public override void OpenWindow()
    {
        base.OpenWindow();
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
    }

    public override void OnClickCloseBtn()
    {
        if (GameManager.Instance)
            GameManager.Instance.UIPop();
        CloseWindow();
    }

    void OnMasterSliderChanged(float value)
    {
        SoundManager.Instance.SetVolume(SoundManager.eSoundGroup.Master, value);
        if (_textMaster)
            _textMaster.text = SetVolumeText(value);
    }

    void OnBGMSliderChanged(float value)
    {
        SoundManager.Instance.SetVolume(SoundManager.eSoundGroup.BGM, value);
        if (_textBGM)
            _textBGM.text = SetVolumeText(value);
    }

    void OnSESliderChanged(float value)
    {
        SoundManager.Instance.SetVolume(SoundManager.eSoundGroup.SE, value);
        SoundManager.Instance.SetVolume(SoundManager.eSoundGroup.SE_Footstep, value);
        if (_textSE)
            _textSE.text = SetVolumeText(value);
    }

    void OnMasterToggleChanged(Toggle change)
    {
        if (_fromChild)
        {
            _fromChild = false;
            return;
        }

        if (change.isOn)
        {
            _toggleBGM.isOn = true;
            _toggleSE.isOn = true;
        }
        else
        {
            _toggleBGM.isOn = false;
            _toggleSE.isOn = false;
        }
    }

    void OnBGMToggleChanged(Toggle change)
    {
        if (change.isOn)
        {
            SoundManager.Instance.SetUnPause(SoundManager.eSoundGroup.BGM);
            _fromChild = true;
            _toggleMaster.isOn = true;
        }
        else
        {
            SoundManager.Instance.SetPause(SoundManager.eSoundGroup.BGM);
            if (!_toggleSE.isOn)
            {
                _fromChild = true;
                _toggleMaster.isOn = false;
            }
        }
    }

    void OnSEToggleChanged(Toggle change)
    {
        if (change.isOn)
        {
            SoundManager.Instance.SetUnPause(SoundManager.eSoundGroup.SE);
            SoundManager.Instance.SetUnPause(SoundManager.eSoundGroup.SE_Footstep);
            _fromChild = true;
            _toggleMaster.isOn = true;
        }
        else
        {
            SoundManager.Instance.SetPause(SoundManager.eSoundGroup.SE);
            SoundManager.Instance.SetPause(SoundManager.eSoundGroup.SE_Footstep);
            if (!_toggleBGM.isOn)
            {
                _fromChild = true;
                _toggleMaster.isOn = false;
            }
        }
    }

    private string SetVolumeText(float value)
    {
        return ((int)(value * 100f)).ToString() + "%";
    }


    /// <summary>
    /// Debug Functions
    /// </summary>
    void OnBGMPlayButton()
    {
        SoundManager.Instance.UpdateBGMForCurrentScene();
    }
    void BGMStopButton()
    {
        SoundManager.Instance.StopBGMSound();
    }
    void SEPlayButton()
    {
        if (!_dropdownAudioList || !_toggleLoop)
            return;

        SoundManager.Instance.PlaySESoundForDebug(
            (SoundManager.eSoundChannel)_dropdownChannelList.value,
            _dropdownAudioList.options[_dropdownAudioList.value].text,
            _toggleLoop.isOn);
    }
    void SEStopButton()
    {
        SoundManager.Instance.StopSESound((SoundManager.eSoundChannel)_dropdownChannelList.value);
    }

    void SetDropDownMenu()
    {
        _dropdownAudioList.ClearOptions();
        List<string> options = new List<string>();

        Dictionary<string, AudioClip> dic = SoundManager.Instance.GetAudioDic();
        foreach (KeyValuePair<string, AudioClip> pair in dic)
        {
            options.Add(pair.Key);
        }
        _dropdownAudioList.AddOptions(options);
        _dropdownAudioList.SetValueWithoutNotify(0);

        _dropdownChannelList.SetValueWithoutNotify(0);
    }
}
