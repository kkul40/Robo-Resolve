using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public GameObject _prefabSoundWindow = null;
    private GameObject _goSoundWindow = null;

    public void NextScene()
    {
        SceneMaster.Instance.SceneChange("RobotGameTutorial");
    }

    public void OpenOption()
    {
        if (_goSoundWindow == null)
        {
            GameObject goCanvas = GameObject.Find("Canvas");
            if (goCanvas)
            {
                GameObject go = (GameObject)Instantiate(_prefabSoundWindow, goCanvas.transform);
                if (go)
                {
                    _goSoundWindow = go;
                    _goSoundWindow.SetActive(true);
                }
            }
        }
        else
        {
            _goSoundWindow.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
