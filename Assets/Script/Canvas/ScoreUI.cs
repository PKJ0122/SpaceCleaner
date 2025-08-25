using DG.Tweening;
using TMPro;

public class ScoreUI : UIBase
{
    TMP_Text _score;

    protected override void Awake()
    {
        base.Awake();
        _score = transform.Find("Text (TMP) - Score").GetComponent<TMP_Text>();

        GameManager.Instance.OnScoreChange += v =>
        {
            int current = int.Parse(_score.text);
            DOTween.To(() => current, x => _score.text = x.ToString(), v, 0.5f);
        };
    }
}
