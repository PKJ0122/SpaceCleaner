using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : UIBase
{
    Button _setting;


    protected override void Awake()
    {
        base.Awake();
        _setting = transform.Find("Button - Setting").GetComponent<Button>();
        _setting.onClick.AddListener(() =>
        {
            UIManager.Instance.Get<SettingUI>().Show();
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
        });
    }
}
