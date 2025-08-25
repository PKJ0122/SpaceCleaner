using DG.Tweening;
using UnityEngine;

public class BestEffectUI : UIBase
{
    RectTransform _ui;


    protected override void Awake()
    {
        base.Awake();
        _ui = transform.Find("Image").GetComponent<RectTransform>();
        _ui.gameObject.SetActive(false);

        GameManager.Instance.OnRefreshBestScore += v =>
        {
            if (v)
            {
                Show();
            }
        };
    }

    public override void Show()
    {
        Sequence seq = DOTween.Sequence();

        _ui.gameObject.SetActive(true);
        _ui.localScale = new Vector3(0f, 1f, 1f);

        SoundManager.Instance.SFX_Play(SFX_List.NewBestScore);
        seq.Append(_ui.DOScaleX(1f, 0.5f)).SetEase(Ease.OutQuad);
        seq.AppendInterval(1f);
        seq.Append(_ui.DOScaleX(0f, 0.3f)).SetEase(Ease.OutQuad);
        seq.Play();
    }
}
