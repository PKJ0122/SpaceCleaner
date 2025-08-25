using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class ComboUI : UIBase
{
    IObjectPool<PoolObject> _pool;
    PoolObject _comboPrefeb;

    RectTransform _panel;


    protected override void Awake()
    {
        base.Awake();
        _comboPrefeb = Resources.Load<PoolObject>("Text (TMP) - Combo");
        _panel = transform.Find("Panel").GetComponent<RectTransform>();
        CreatePool();
    }

    public void Show(RectTransform rect, int combo)
    {
        if (combo <= 1) return;

        PoolObject comboObject = _pool.Get();
        TMP_Text comboText = comboObject.GetComponent<TMP_Text>();
        RectTransform comboRect = comboObject.GetComponent<RectTransform>();

        comboText.text = $"Combo<size=100> {combo}";
        comboRect.localScale = new Vector3(1, 1, 1);

        Vector2 anchoredPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel,
                                                                rect.position,
                                                                null,
                                                                out anchoredPosition);
        comboRect.anchoredPosition = anchoredPosition;

        Sequence seq = DOTween.Sequence();

        seq.Append(comboRect.DOLocalJump(anchoredPosition,25f,1,0.5f).SetEase(Ease.OutQuad));
        seq.AppendInterval(0.5f);
        seq.Append(comboRect.DOScale(0,0.5f));
        seq.Play().OnComplete(() =>
        {
            comboObject.RelasePool();
        });
        SoundManager.Instance.SFX_Play(SFX_List.Combo);
    }

    public void CreatePool()
    {
        int capacity = 36;
        IObjectPool<PoolObject> pool = new ObjectPool<PoolObject>(() => Create(_comboPrefeb),
                                                                        OnPoolItem,
                                                                        OnReleaseItem,
                                                                        OnDestroyItem,
                                                                        true,
                                                                        capacity,
                                                                        capacity
                                                                        );
        _pool = pool;
    }

    PoolObject Create(PoolObject poolObject)
    {
        PoolObject creatingObject = Instantiate(poolObject).SetPool(_pool);
        creatingObject.transform.SetParent(_panel);

        return creatingObject;
    }

    public void OnPoolItem(PoolObject item)
    {
        item.gameObject.SetActive(true);
    }

    public void OnReleaseItem(PoolObject item)
    {
        item.gameObject.SetActive(false);
    }

    public void OnDestroyItem(PoolObject item)
    {
        Destroy(item.gameObject);
    }
}
