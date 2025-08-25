using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    GridLayoutGroup _gridLayoutGroup;
    RectTransform _rect;
    Image[] _blocks;

    BlockData _blockData;
    int _catKind;

    public BlockData BlockData => _blockData;
    public int CatKind => _catKind;


    public void Awake()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _rect = GetComponent<RectTransform>();
        _blocks = transform.GetComponentsInChildren<Image>(true)
                           .Where(image => image.gameObject != gameObject)
                           .ToArray();
        
        foreach (Image block in _blocks)
        {
            block.gameObject.SetActive(false);
        }

        GameManager.Instance.RegisterBlock(this);
    }

    public void ChangeBlockData(BlockData blockData, int catKind , Sprite catSprite)
    {
        _blockData = blockData;
        _catKind = catKind;

        _gridLayoutGroup.constraintCount = blockData.H;
        int needBlockCount = blockData.H * blockData.V;

        for (int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i].gameObject.SetActive(i < needBlockCount);
            _blocks[i].color = new Color(1f, 1f, 1f, 0f);
        }

        foreach (var item in blockData.blockCoordinates)
        {
            int index = blockData.H * item.Y + item.X;
            _blocks[index].sprite = catSprite;
            _blocks[index].color = new Color(1f, 1f, 1f, 1f);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.5f, 0.3f));
        seq.Append(transform.DOScale(1f, 0.2f));
        seq.Play();
    }

    public void SelectBlock()
    {
        int needBlockCount = _blockData.H * _blockData.V;

        for (int i = 0; i < needBlockCount; i++)
        {
            _blocks[i].gameObject.SetActive(false);
        }
        enabled = false;
    }

    public void DropComplete()
    {
        GameManager.Instance.DropComplete(this);
    }

    public void DropCancel()
    {
        int needBlockCount = _blockData.H * _blockData.V;
        
        for (int i = 0; i < needBlockCount; i++)
        {
            _blocks[i].gameObject.SetActive(true);
        }
        enabled = true;
    }
}