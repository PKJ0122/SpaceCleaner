using UnityEngine;
using UnityEngine.UI;

public class ESCUI : UIBase
{
    Button _esc;
    Button _hide;

    Transform _ui;


    protected override void Awake()
    {
        base.Awake();
        _ui = transform.Find("Panel/Image");
        _esc = transform.Find("Panel/Image/Button - Yes").GetComponent<Button>();
        _hide = transform.Find("Panel/Image/Button - No").GetComponent<Button>();
        _esc.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            Application.Quit();
        });
        _hide.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            HideEffect(_ui);
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Show();
    }

    public override void Show()
    {
        base.Show();
        SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);
        ShowEffect(_ui);
    }
}
