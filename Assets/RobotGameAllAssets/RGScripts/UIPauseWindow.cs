using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * 2024. 10. 03 Created by Pumpkin
 */

public class UIPauseWindow : UIWindow
{
    #region Unity Event Functions
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
        GameManager.Instance.SetGamePause(true);
        SoundManager.Instance.SetPauseSEFootstepChannel();
        base.OpenWindow();
    }

    public override void CloseWindow()
    {
        GameManager.Instance.SetGamePause(false);
        base.CloseWindow();
    }

    public override void OnClickCloseBtn()
    {
        GameManager.Instance.UIPop();
        CloseWindow();
    }

    public void OnClickPlayBtn()
    {
        OnClickCloseBtn();
    }

    public void OnClickRestartBtn()
    {
        GameManager.Instance.RestartGame();
        OnClickCloseBtn();
    }

    public void OnClickOptionsBtn()
    {
        GameManager.Instance.OpenSoundWindow();
    }

    public void OnClickQuitBtn()
    {
        Application.Quit();
    }

    public void OnClickCreditBtn()
    {
        GameManager.Instance.OpenCreditWindow();
    }
}
