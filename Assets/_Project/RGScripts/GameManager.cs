using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/* 
 * 2024. 10. 02 Created by Pumpkin
 */

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                return null;
            return _instance;
        }
    }
    #endregion

    #region Variables
    public GameObject _prefabPauseWindow = null;
    public GameObject _prefabSoundWindow = null;
    public GameObject _prefabCreditWindow = null;
    public GameObject _prefabResultWindow = null;

    private UIWindow _PauseWindow = null;
    private UIWindow _SoundWindow = null;

    private bool _pause = false;
    private bool _isInCredit = false;
    private bool _isInResult = false;

    private Stack<UIWindow> _stackUI = null;

    private PlayerMovement _player = null;
    private List<BatteryCollectibleScript> _listBatteries = null;
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

        _stackUI = new Stack<UIWindow>();
        _listBatteries = new List<BatteryCollectibleScript>();

        StartCoroutine(DrawUIWindows());

        GameObject obj = GameObject.Find("NewPlayer");
        if (obj)
            _player = obj.GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!_isInCredit)
                ControlUIWindows();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isInResult)
                OpenResultWindow();
        }
    }
    #endregion

    public bool IsGamePause()
    {
        return _pause;
    }

    public void SetGamePause(bool pause)
    {
        _pause = pause;
    }

    public void SetCreditScreen(bool inCredit)
    {
        _isInCredit = inCredit;
    }

    public void SetResultScreen(bool inResult)
    {
        _isInResult = inResult;
    }

    void ControlUIWindows()
    {
        if (_stackUI.Count == 0)
        {
            OpenPauseWindow();
        }
        else
        {
            UIPop();
        }
    }

    public void OpenPauseWindow()
    {
        if (_PauseWindow)
        {
            _PauseWindow.OpenWindow();
            UIPush(_PauseWindow);
        }
    }

    public void OpenSoundWindow()
    {
        if (_SoundWindow)
        {
            _SoundWindow.OpenWindow();
            UIPush(_SoundWindow);
        }
    }

    public void OpenCreditWindow()
    {
        GameObject goCanvas = GameObject.Find("Canvas");
        if (goCanvas)
        {
            GameObject go = (GameObject)Instantiate(_prefabCreditWindow, goCanvas.transform);
            if (go)
            {
                UIWindow cs = go.GetComponent<UIWindow>();
                UIPush(cs);
                _isInCredit = true;
            }
        }
    }

    public void OpenResultWindow()
    {
        GameObject goCanvas = GameObject.Find("Canvas");
        if (goCanvas)
        {
            GameObject go = (GameObject)Instantiate(_prefabResultWindow, goCanvas.transform);
            if (go)
            {
                UIWindow cs = go.GetComponent<UIWindow>();
                UIPush(cs);
                _isInResult = true;
            }
        }
    }

    public void UIPush(UIWindow obj)
    {
        _stackUI.Push(obj);
    }

    public void UIPop()
    {
        if (_stackUI.Count <= 0)
            return;

        UIWindow go = _stackUI.Pop();
        if (go)
            go.CloseWindow();
    }

    public void RestartGame()
    {
        // Player Reset
        if (_player)
            _player.ResetPlayer();

        // Battery Items Reset
        for (int i = 0; i < _listBatteries.Count; ++i)
            _listBatteries[i].Reset();

        // Battery Info Reset
        // TO_DO : ????
    }

    public void AddBatteriesToResetPool(BatteryCollectibleScript cs)
    {
        if (cs == null)
            return;

        _listBatteries.Add(cs);
    }

    IEnumerator DrawUIWindows()
    {
        while (true)
        {
            if (SoundManager.Instance != null && SoundManager.Instance.GetSoundLoadingFinish())
                break;

            yield return null;
        }

        GameObject goCanvas = GameObject.Find("Canvas");
        if (goCanvas)
        {
            GameObject go = (GameObject)Instantiate(_prefabPauseWindow, goCanvas.transform);
            if (go)
                _PauseWindow = go.GetComponent<UIWindow>();
            go = (GameObject)Instantiate(_prefabSoundWindow, goCanvas.transform);
            if (go)
                _SoundWindow = go.GetComponent<UIWindow>();
            if (_PauseWindow)
                _PauseWindow.gameObject.SetActive(false);
            if (_SoundWindow)
                _SoundWindow.gameObject.SetActive(false);
        }
    }
}
