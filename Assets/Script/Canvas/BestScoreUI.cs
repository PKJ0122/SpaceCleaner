using DG.Tweening;
using TMPro;

public class BestScoreUI : UIBase
{
    int _beforeScore;

    TMP_Text _score;


    protected override void Awake()
    {
        base.Awake();
        _score = transform.Find("Text (TMP) - BastScore").GetComponent<TMP_Text>();

        _beforeScore = GameManager.Instance.BestScore;

        GameManager.Instance.OnBestScoreChange += v =>
        {
            DOTween.To(() => _beforeScore, x => _score.text = $"<sprite=0>  <size=50>{v}", v, 0.5f);
            _beforeScore = v;
        };
    }
}
