using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResultWindow : UIWindow
{
    #region Variables
    public TMP_Text _textWin = null;
    #endregion

    #region Unity Event Functions
    protected override void Awake()
    {
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
        GameManager.Instance.SetResultScreen(false);
        Destroy(this.gameObject);
    }

    public override void OnClickCloseBtn()
    {
        GameManager.Instance.SetResultScreen(false);
        GameManager.Instance.UIPop();
        CloseWindow();
    }

    public void OnClickBtnBackToTitle()
    {
        // back to title scene
    }
}
