using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : PanelBase
{
    private Button closeBtn;

    public override void Init(params object[] _args)
    {
        base.Init(_args);
        skinPath = "InfoPanel";
        layer = PanelLayer.Tips;
    }

    public override void OnShowing()
    {
        base.OnShowing();

        closeBtn = skinObj.transform.Find("Close").GetComponent<Button>();

        closeBtn.onClick.AddListener(() => {
            UIManager.instance.OpenPanel<TitlePanel>();
            Close();
        });

    }
}
