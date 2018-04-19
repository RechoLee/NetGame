using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : PanelBase
{
    private Button startBtn;
    private Button infoBtn;

    public override void Init(params object[] _args)
    {
        base.Init(_args);

        skinPath = "TitlePanel";
        layer = PanelLayer.Panel;

    }

    public override void OnShowing()
    {
        base.OnShowing();

        Transform skinTran = skinObj.transform;

        startBtn = skinTran.Find("Begin").GetComponent<Button>();
        infoBtn = skinTran.Find("Info").GetComponent<Button>();

        startBtn.onClick.AddListener(() => {
            Debug.Log("**************进入新的场景************");

            Close();
        });

        infoBtn.onClick.AddListener(() => {
            UIManager.instance.OpenPanel<InfoPanel>();

            Close();
        });

    }

}
