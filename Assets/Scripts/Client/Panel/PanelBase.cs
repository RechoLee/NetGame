using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{

    public object[] args;

    public string skinPath;

    public GameObject skinObj;

    public PanelLayer layer;

    // Use this for initialization
    void Start()
    {

    }


    #region My Define

    public virtual void Init(params object[] _args)
    {
        args = _args;
    }

    public virtual void OnShowing()
    {

    }

    public virtual void OnShowed()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void OnClosing()
    {


    }

    public virtual void OnClosed() { }

    public virtual void Close()
    {
        string name = this.GetType().ToString();
        UIManager.instance.ClosePanel(name);
    }

    #endregion
}
