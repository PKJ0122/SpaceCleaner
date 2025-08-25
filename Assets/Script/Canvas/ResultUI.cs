using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : UIBase
{
    TMP_Text _comment;
    TMP_Text _scoreText;
    TMP_Text _score;
    Button _restart;

    public Action OnRestart;

    RectTransform _ui;


    protected override void Awake()
    {
        base.Awake();
        _comment = transform.Find("Panel/Image/Text (TMP) - GameResult").GetComponent<TMP_Text>();
        _scoreText = transform.Find("Panel/Image/Text (TMP) - ScoreText").GetComponent<TMP_Text>();
        _score = transform.Find("Panel/Image/Text (TMP) - Score").GetComponent<TMP_Text>();
        _restart = transform.Find("Panel/Image/Button - Restart").GetComponent<Button>();
        _ui = transform.Find("Panel/Image").GetComponent<RectTransform>();

        _restart.onClick.AddListener(() =>
        {
            HideEffect(_ui);
            SoundManager.Instance.SFX_Play(SFX_List.ButtonClick);
            OnRestart?.Invoke();
            GameManager.Instance.GameStart();
        });
    }

    public override void Show()
    {
        base.Show();

        SoundManager.Instance.SFX_Play(SFX_List.GameEnd);

        Sequence seq = DOTween.Sequence();

        _comment.enabled = false;
        _scoreText.enabled = false;
        _score.enabled = false;
        _restart.gameObject.SetActive(false);

        SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);
        seq.Append(_ui.DOScale(1.1f, 0.2f));
        seq.Append(_ui.DOScale(1f, 0.1f));
        seq.AppendCallback(() =>
        {
            _comment.enabled = true;
            SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);
        });
        seq.Append(_comment.rectTransform.DOScale(1.1f, 0.3f));
        seq.Append(_comment.rectTransform.DOScale(1f, 0.2f));
        seq.AppendCallback(() =>
        {
            _scoreText.enabled = true;
            SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);
        });
        seq.Append(_scoreText.rectTransform.DOScale(1.1f, 0.3f));
        seq.Append(_scoreText.rectTransform.DOScale(1f, 0.2f));
        seq.AppendCallback(() =>
        {
            _score.enabled = true;
            SoundManager.Instance.SFX_Play(SFX_List.ScoreUp);
        });
        int score = GameManager.Instance.Score;
        seq.Append(DOTween.To(() => 0, x => _score.text = x.ToString(), score, 1f));
        seq.Append(_score.rectTransform.DOScale(1.1f, 0.3f));
        seq.Append(_score.rectTransform.DOScale(1f, 0.2f));
        seq.AppendCallback(() =>
        {
            _restart.gameObject.SetActive(true);
            SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);
        });
        seq.Append(_restart.transform.DOScale(1.1f, 0.3f));
        seq.Append(_restart.transform.DOScale(1f, 0.2f));

        seq.Play();
    }
}
