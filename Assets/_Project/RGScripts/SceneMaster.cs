using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour
{
    #region Singleton
    private static SceneMaster _instance = null;
    public static SceneMaster Instance
    {
        get
        {
            if (_instance == null)
                return null;
            return _instance;
        }
    }
    #endregion

    const int SCREEN_WIDTH = 1920;
    const int SCREEN_HEIGHT = 1080;

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

        Screen.SetResolution(SCREEN_WIDTH, SCREEN_HEIGHT, true);
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

    public void SceneChange(string sceneName)
    {
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SoundManager.Instance.UpdateBGMForCurrentScene();
    }
}
