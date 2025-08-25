using System;
using UnityEngine;

public class Setting : Singleton<Setting>
{
    public const int TRUE = 1;

    public static Color baseColor = new Color(0.82f, 0.82f, 0.82f);
    public static Color darkColor = new Color(0.18f, 0.18f, 0.18f);

    public bool BgmMute
    {
        get
        {
            if (!PlayerPrefs.HasKey("BGM"))
            {
                PlayerPrefs.SetInt("BGM", 0);
            }
            return PlayerPrefs.GetInt("BGM") == TRUE;
        }
        set
        {
            PlayerPrefs.SetInt("BGM", value ? 1 : 0);
            OnBgmMuteChange?.Invoke(value);
        }
    }

    public bool SfxMute
    {
        get
        {
            if (!PlayerPrefs.HasKey("SFX"))
            {
                PlayerPrefs.SetInt("SFX", 0);
            }
            return PlayerPrefs.GetInt("SFX") == TRUE;
        }
        set
        {
            PlayerPrefs.SetInt("SFX", value ? 1 : 0);
            OnSfxMuteChange?.Invoke(value);
        }
    }

    public bool DarkMode
    {
        get
        {
            if (!PlayerPrefs.HasKey("DRAKMODE"))
            {
                PlayerPrefs.SetInt("DRAKMODE", 0);
            }
            return PlayerPrefs.GetInt("DRAKMODE") == TRUE;
        }
        set
        {
            PlayerPrefs.SetInt("DRAKMODE", value ? 1 : 0);
            OnDarkModeChange?.Invoke(value);
        }
    }

    public event Action<bool> OnBgmMuteChange;
    public event Action<bool> OnSfxMuteChange;
    public event Action<bool> OnDarkModeChange;


    protected override void Awake()
    {
        base.Awake();
        OnDarkModeChange += v =>
        {
            Camera.main.backgroundColor = v ? darkColor : baseColor;
        };
    }

    private void Start()
    {
        OnBgmMuteChange?.Invoke(BgmMute);
        OnSfxMuteChange?.Invoke(SfxMute);
        OnDarkModeChange?.Invoke(DarkMode);
    }
}
