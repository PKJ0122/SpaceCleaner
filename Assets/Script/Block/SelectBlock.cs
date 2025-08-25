using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SelectBlock : MonoBehaviour
{
    const float _correction = 250f;

    GridLayoutGroup _gridLayoutGroup;
    RectTransform _rect;
    Image[] _blocks;

    BlockData _blockData;
    int _catKind;

    public BlockData BlockData => _blockData;
    public int CatKind => _catKind;

    BlockUI _blockUI;

    bool _selecting;
    bool _uiUp;

    Block _selectBlock;

    Dictionary<Image, Tile> _tiles = new();
    Dictionary<Image, Tile> _temporaryTile = new();


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

        GameManager.Instance.OnGameEnd += () =>
        {
            foreach (Image block in _blocks)
            {
                block.gameObject.SetActive(false);
                block.sprite = null;
            }
            _selecting = false;
            _blockData = null;
            _catKind = -1;
            _tiles.Clear();
            _temporaryTile.Clear();
        };

        UIManager.Instance.OnUiUpChange += v => _uiUp = v;
    }

    private void Start()
    {
        _blockUI = UIManager.Instance.Get<BlockUI>();
    }

    private void Update()
    {
        if (_uiUp) return;

        if (!_selecting)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                Touch touch = Input.GetTouch(0);

                if (Input.touchCount > 0)
                {
                    touch = Input.GetTouch(0);
                }
                else
                {
                    touch = new Touch
                    {
                        position = Input.mousePosition,
                        phase = TouchPhase.Began
                    };
                }

                BlockSelect(touch);
                Vector3 position = new Vector2(touch.position.x, touch.position.y + _correction);
                _rect.position = position;
            }
        }
        else
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                Touch touch = Input.GetTouch(0);

                if (Input.touchCount > 0)
                {
                    touch = Input.GetTouch(0);
                }
                else
                {
                    touch = new Touch
                    {
                        position = Input.mousePosition,
                    };
                }

                if (touch.phase == TouchPhase.Moved)
                {

                    Vector3 position = new Vector2(touch.position.x, touch.position.y + _correction);
                    _rect.position = position;

                    bool allBlockOverLap = GetAllOverLapCheck();

                    if (allBlockOverLap)
                    {
                        if (_tiles.Count != 0)
                        {
                            Tile beforeTile = _tiles.First().Value;
                            Tile currentTile = _temporaryTile[_tiles.First().Key];

                            if (beforeTile == currentTile) return;
                        }

                        UIManager.Instance.Get<TileUI>().EraseTemporaryTile();

                        foreach (KeyValuePair<Image, Tile> item in _tiles)
                        {
                            item.Value.ResetTile();
                        }

                        _tiles.Clear();

                        foreach (KeyValuePair<Image, Tile> item in _temporaryTile)
                        {
                            Sprite catSprite = ResourcesRepository.Instance.CatData.CatSprites[_catKind];
                            item.Value.ChangeTemporaryTile(catSprite);
                            _tiles.Add(item.Key, item.Value);
                        }

                        UIManager.Instance.Get<TileUI>().DrawTemporaryTile(_temporaryTile);
                    }
                    else
                    {
                        UIManager.Instance.Get<TileUI>().EraseTemporaryTile();

                        foreach (KeyValuePair<Image, Tile> item in _tiles)
                        {
                            item.Value.ResetTile();
                        }

                        _tiles.Clear();
                    }


                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    UIManager.Instance.Get<TileUI>().EraseTemporaryTile();

                    bool allBlockOverLap = GetAllOverLapCheck();

                    if (allBlockOverLap)
                    {
                        foreach (KeyValuePair<Image, Tile> item in _temporaryTile)
                        {
                            Sprite catSprite = ResourcesRepository.Instance.CatData.CatSprites[_catKind];
                            item.Value.ChangeTile(catSprite);
                        }

                        UIManager.Instance.Get<TileUI>().DelTile(_temporaryTile);
                        GameManager.Instance.Score += _temporaryTile.Count;
                        _selectBlock.DropComplete();
                        SoundManager.Instance.SFX_Play(SFX_List.BlockPickUp);
                    }
                    else
                    {
                        foreach (KeyValuePair<Image, Tile> item in _temporaryTile)
                        {
                            item.Value.ResetTile();
                        }
                    }

                    DropCancel(allBlockOverLap);
                    _tiles.Clear();
                    _temporaryTile.Clear();
                }
            }
        }
    }

    bool GetAllOverLapCheck()
    {
        int needBlockCount = _blockData.H * _blockData.V;

        bool allBlockOverLap = true;

        _temporaryTile.Clear();

        for (int i = 0; i < needBlockCount; i++)
        {
            if (_blocks[i].sprite == null) continue;

            Tile tile = GetOverLapTile(_blocks[i].transform);
            if (tile == null || tile.State == TileState.Full)
            {
                allBlockOverLap = false;
                break;
            }
            _temporaryTile.Add(_blocks[i], tile);
        }

        return allBlockOverLap;
    }

    Tile GetOverLapTile(Transform transform)
    {
        TileUI ui = UIManager.Instance.Get<TileUI>();

        GraphicRaycaster _graphicRaycaster = ui.GraphicRaycaster;
        EventSystem _eventSystem = ui.EventSystem;

        PointerEventData pointerData = new PointerEventData(_eventSystem)
        {
            position = transform.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult item in results)
        {
            if (item.gameObject.TryGetComponent<Tile>(out Tile tile))
            {
                return tile;
            }
        }

        return null;
    }

    public void ChangeBlockData(BlockData blockData, int catKind, Sprite catSprite)
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
    }

    public void DropCancel(bool tileChange)
    {
        if (_blockData == null) return;

        int needBlockCount = _blockData.H * _blockData.V;

        for (int i = 0; i < needBlockCount; i++)
        {
            _blocks[i].gameObject.SetActive(false);
            _blocks[i].sprite = null;
        }
        if (!tileChange)
        {
            _selectBlock.DropCancel();
        }
        _selecting = false;
        _selectBlock = null;
        _blockData = null;
    }

    void BlockSelect(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            GraphicRaycaster _graphicRaycaster = _blockUI.GraphicRaycaster;
            EventSystem _eventSystem = _blockUI.EventSystem;

            PointerEventData pointerData = new PointerEventData(_eventSystem)
            {
                position = touch.position
            };

            List<RaycastResult> results = new List<RaycastResult>();
            _graphicRaycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                if (results[0].gameObject.TryGetComponent(out Block block))
                {
                    if (!block.enabled) return;

                    BlockData blockData = block.BlockData;
                    int catKind = block.CatKind;
                    Sprite catSprite = ResourcesRepository.Instance.CatData.CatSprites[catKind];

                    _selectBlock = block;
                    _selecting = true;
                    ChangeBlockData(blockData, catKind, catSprite);
                    block.SelectBlock();
                    SoundManager.Instance.SFX_Play(SFX_List.BlockPickUp);
                }
            }
        }
    }
}
