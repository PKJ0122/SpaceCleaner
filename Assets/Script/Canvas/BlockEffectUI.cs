using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class BlockEffectUI : UIBase
{
    IObjectPool<PoolObject> _pool;
    PoolObject _blockPrefeb;

    TileUI _tileUI;

    RectTransform _panel;

    protected override void Awake()
    {
        base.Awake();
        _blockPrefeb = Resources.Load<PoolObject>("Image - Block");
        _panel = transform.Find("Panel").GetComponent<RectTransform>();
        CreatePool();
    }

    private void Start()
    {
        _tileUI = UIManager.Instance.Get<TileUI>();
    }

    public void BlockDelEffect(List<TileData> tiles)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < tiles.Count; i++)
        {
            PoolObject blockObject = _pool.Get();

            TileData tileData = tiles[i];

            Vector2 anchoredPosition = Vector2.zero;

            RectTransform tileRect = tileData.tile.GetComponent<RectTransform>();
            RectTransform rect = blockObject.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel,
                                                                    tileRect.position,
                                                                    null,
                                                                    out anchoredPosition);

            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = tileRect.sizeDelta;
            rect.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            blockObject.GetComponent<Image>().sprite = tileData.tileSprite;

            seq.Join(rect.DOScale(0f,1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                blockObject.RelasePool();
            }));
        }

        SoundManager.Instance.SFX_Play(SFX_List.BlockDel);
        seq.Play();
    }

    public void AllDelEffect(Sprite sprite)
    {
        PoolObject blockObject = _pool.Get();

        RectTransform rect = blockObject.GetComponent<RectTransform>();
        RectTransform tileRect = _tileUI.Back;

        Vector2 anchoredPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel,
                                                                tileRect.position,
                                                                null,
                                                                out anchoredPosition);

        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = tileRect.sizeDelta;
        rect.localScale = Vector3.zero;
        blockObject.GetComponent<Image>().sprite = sprite;
        blockObject.transform.SetAsLastSibling();

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOScale(1.2f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.Append(rect.DOScale(0f, 0.5f));

        seq.Play().OnComplete(() => blockObject.RelasePool());
        SoundManager.Instance.SFX_Play(SFX_List.AllDel);
        SoundManager.Instance.SFX_Play(SFX_List.Great);
    }

    public void CreatePool()
    {
        int capacity = 36;
        IObjectPool<PoolObject> pool = new ObjectPool<PoolObject>(() => Create(_blockPrefeb),
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
