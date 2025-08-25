using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    const float NO_DARK_MODE = 1.0f;
    const float DARK_MODE = 0.18f;

    Button _bgm;
    Button _sfx;
    Button _dark;
    Button _ad;
    Button _gameRestart;
    Button _email;
    Button _close;

    Image _bgmMute;
    Image _sfxMute;
    Image _darkLuminosity;

    Transform _ui;


    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;

        _bgm = transform.Find("Panel/Image/Button - BGM").GetComponent<Button>();
        _sfx = transform.Find("Panel/Image/Button - SFX").GetComponent<Button>();
        _dark = transform.Find("Panel/Image/Button - Dark").GetComponent<Button>();
        _ad = transform.Find("Panel/Image/Button - Ads").GetComponent<Button>();
        _gameRestart = transform.Find("Panel/Image/Button - Restart").GetComponent<Button>();
        _email = transform.Find("Panel/Image/Button - Email").GetComponent<Button>();
        _close = transform.Find("Panel/Image/Button - Close").GetComponent<Button>();

        _bgmMute = _bgm.transform.Find("Image - Mute").GetComponent<Image>();
        _sfxMute = _sfx.transform.Find("Image - Mute").GetComponent<Image>();
        _darkLuminosity = _dark.transform.Find("Image - Dark").GetComponent<Image>();

        _ui = transform.Find("Panel/Image").GetComponent<Transform>();

        Setting setting = Setting.Instance;

        _bgm.onClick.AddListener(() =>
        {
            setting.BgmMute = !setting.BgmMute;
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
        });
        setting.OnBgmMuteChange += v =>
        {
            _bgmMute.enabled = v;
        };
        _sfx.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            setting.SfxMute = !setting.SfxMute;
        });
        setting.OnSfxMuteChange += v =>
        {
            _sfxMute.enabled = v;
        };
        _dark.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            setting.DarkMode = !setting.DarkMode;
        });
        setting.OnDarkModeChange += v =>
        {
            _darkLuminosity.color = v ? new Color(DARK_MODE, DARK_MODE, DARK_MODE) : new Color(NO_DARK_MODE, NO_DARK_MODE, NO_DARK_MODE);
        };
        _ad.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            AdManager.Instance.AdShow();
        });
        _gameRestart.onClick.AddListener(() =>
        {
            Hide();
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            GameManager.Instance.GameEnd();
        });
        _email.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            GUIUtility.systemCopyBuffer = "qlrrudwns@gmail.com";
        });
        _close.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            HideEffect(_ui);
        });
    }

    public override void Show()
    {
        base.Show();
        SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);
        ShowEffect(_ui);
    }
}
