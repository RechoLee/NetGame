using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    protected Transform canvas;

    public Dictionary<string, PanelBase> panelDic = new Dictionary<string, PanelBase>();

    public Dictionary<PanelLayer, Transform> layerDic = new Dictionary<PanelLayer, Transform>();


    private void Awake()
    {
        instance = this;

        InitLayer();
    }


    private void InitLayer()
    {
        canvas = GameObject.Find("Canvas").transform;

        if (canvas == null)
            Debug.LogError("not find canvas");

        foreach (PanelLayer item in Enum.GetValues(typeof(PanelLayer)))
        {
            Transform layer = canvas.Find(item.ToString());

            if (layer != null)
                layerDic.Add(item, layer);
        }
    }

    public void OpenPanel<T>(string path = "", params object[] args)
        where T : PanelBase
    {
        string name = typeof(T).ToString();

        PanelBase panel = canvas.gameObject.AddComponent<T>();

        panelDic.Add(name, panel);

        panel.Init(args);

        path = string.IsNullOrEmpty(path) ? panel.skinPath : path;

        GameObject skinObj = Resources.Load<GameObject>(path);

        if (skinObj == null)
            Debug.LogError("skin Obj is null");
        panel.skinObj = Instantiate<GameObject>(skinObj);

        panel.skinObj.transform.SetParent(layerDic[panel.layer], false);

        panel.OnShowing();
        panel.OnShowed();

    }

    public void ClosePanel(string name)
    {
        PanelBase panel;
        if (!panelDic.TryGetValue(name, out panel))
        {
            return;
        }


        panel.OnClosing();
        panelDic.Remove(name);
        panel.OnClosed();

        GameObject.Destroy(panel.skinObj);
        Component.Destroy(panel);
    }

    // Use this for initialization
    void Start()
    {
        OpenPanel<TitlePanel>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    //如何获取摄像头
    void Test()
    {

        var cam = new WebCamTexture(WebCamTexture.devices[0].name, 400, 300, 30);

        cam.Play();
    }
}

public enum PanelLayer
{
    Tips,
    Panel
}
