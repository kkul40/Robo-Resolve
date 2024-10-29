using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditWindow : UIWindow
{
    const float START_POS_Y = -600f;
    const float FINISH_POS_Y = 2340f;

    public GameObject _goBtnClose = null;
    public GameObject _goAfterScroll = null;
    public GameObject _objContent = null;
    public float _scrollSpeed = 0f;

    #region Unity Event Functions
    protected override void Awake()
    {
        StartCoroutine(ScrollContent());

        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_objContent.transform.position.y < FINISH_POS_Y)
                _objContent.transform.Translate(Vector3.up * _scrollSpeed * 3);
            else
            {
                _goAfterScroll.SetActive(true);
                _goBtnClose.SetActive(true);
            }
        }

        base.Update();
    }
    #endregion

    public override void OpenWindow()
    {
        base.OpenWindow();
    }

    public override void CloseWindow()
    {
        GameManager.Instance.SetCreditScreen(false);
        Destroy(this.gameObject);
    }

    public override void OnClickCloseBtn()
    {
        GameManager.Instance.SetCreditScreen(false);
        GameManager.Instance.UIPop();
        CloseWindow();
    }

    IEnumerator ScrollContent()
    {
        if (_objContent == null)
            yield break;

        while (_objContent.transform.position.y < FINISH_POS_Y)
        {
            _objContent.transform.Translate(Vector3.up * Time.deltaTime * _scrollSpeed);

            yield return null;
        }

        if (_goAfterScroll)
            _goAfterScroll.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (_goBtnClose)
            _goBtnClose.SetActive(true);
    }
}
