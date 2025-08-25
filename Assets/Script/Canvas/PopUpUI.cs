using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : UIBase
{
    TMP_Text _popUp;
    Button _yes;

    Transform _ui;


    protected override void Awake()
    {
        base.Awake();
        _popUp = transform.Find("Panel/Image/Text (TMP) - PopUp").GetComponent<TMP_Text>();
        _yes = transform.Find("Panel/Image/Button - Yes").GetComponent<Button>();
        _ui = transform.Find("Panel/Image");

        _yes.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            HideEffect(_ui);
        });
    }

    public void Show(string detail)
    {
        base.Show();
        ShowEffect(_ui);
        SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
        _popUp.text = detail;
    }
}
