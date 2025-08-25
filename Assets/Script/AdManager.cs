using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

public class AdManager : Singleton<AdManager>
{
    BannerView _bannerView;
    public BannerView BannerView => _bannerView;

    RewardedAd _rewardedAd;
    public RewardedAd RewardedAd => _rewardedAd;

    string _bannerViewId;
    string _rewardAdId;


    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        _rewardAdId = "adUnitId";
        _bannerViewId = "adUnitId";
#else
        _rewardAdId = "ca-app-pub-5639813524802030/9769699335";
        _bannerViewId = "ca-app-pub-5639813524802030/2555460272";
#endif

        MobileAds.Initialize(initStatus => { });

        DontDestroyOnLoad(this);
        RequestBanner();
    }

    public void AdShow()
    {
        StartCoroutine(C_AdShow());
    }

    void RequestBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }
        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(_bannerViewId, adSize, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();
        BannerView.LoadAd(request);
    }

    const float DELAY = 1f;

    YieldInstruction _delay = new WaitForSeconds(DELAY);

    IEnumerator C_AdShow()
    {
        PopUpUI PopUpUi = UIManager.Instance.Get<PopUpUI>();
        UIBase LodingUi = UIManager.Instance.Get<LodingUI>();
        LodingUi.Show();

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
        }
        _rewardedAd = new RewardedAd(_rewardAdId);
        _rewardedAd.OnAdClosed += (object sender, EventArgs args) =>
        {
            PopUpUi.Show("Thank you.\r\nI Love You <sprite=2>.");
        };
        AdRequest request = new AdRequest.Builder().Build();
        RewardedAd.LoadAd(request);

        float timeout = 10f;
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            if (_rewardedAd.IsLoaded())
            {
                LodingUi.Hide();
                _rewardedAd.Show();
                yield break;
            }

            yield return _delay;
            elapsedTime += DELAY;
        }

        LodingUi.Hide();
        PopUpUi.Show("Fail.");
    }
}
