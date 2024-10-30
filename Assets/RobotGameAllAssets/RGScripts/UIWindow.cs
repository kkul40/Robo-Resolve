using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    protected virtual void Awake()
    {

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnClickCloseBtn()
    {

    }
}
