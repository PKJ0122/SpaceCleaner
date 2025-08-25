using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileUI : UIBase
{
    const float BIG_TILE_SIZE = 1.2f;
    const int MAX_COUNT = 8;
    const int ALL_DEL_PLUS_AMOUNT = 3000;
    const int PLUS_AMOUNT = 30;

    Tile[] _tiles;
    List<int> _h = new List<int>();
    List<int> _v = new List<int>();
    List<Tile> _drawingTiles = new List<Tile>();

    RectTransform _back;
    public RectTransform Back => _back;

    int _combo = 0;


    protected override void Awake()
    {
        base.Awake();

        _back = transform.Find("Image - Back").GetComponent<RectTransform>();
        int tilesCount = _back.childCount;
        _tiles = new Tile[tilesCount];

        for (int i = 0; i < tilesCount; i++)
        {
            _tiles[i] = transform.Find($"Image - Back/Image - Tile ({i})").GetComponent<Tile>();
            _tiles[i].Index = i;
        }

        GameManager.Instance.OnGameEnd += () =>
        {
            _combo = 0;
        };
    }

    private void Start()
    {
        UIManager.Instance.Get<ResultUI>().OnRestart += () =>
        {
            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].ResetTile();
            }
        };
    }

    public bool PossibleBlockLay(BlockData blockData)
    {
        for (int y = 0; y < MAX_COUNT; y++)
        {
            int V = blockData.V;
            if (y + V > MAX_COUNT) break;

            for (int x = 0; x < MAX_COUNT; x++)
            {
                int H = blockData.H;
                if (x + H > MAX_COUNT) break;

                bool possibleBlockLay = true;

                foreach (BlockCoordinate item in blockData.blockCoordinates)
                {
                    int currentX = x + item.X;
                    int currentY = y + item.Y;

                    int currentIndex = currentY * MAX_COUNT + currentX;

                    if (_tiles[currentIndex].State == TileState.Full)
                    {
                        possibleBlockLay = false;
                        break;
                    }
                }

                if (possibleBlockLay) return possibleBlockLay;
            }
        }

        return false;
    }

    public void DrawTemporaryTile(Dictionary<Image, Tile> dic)
    {
        _h.Clear();
        _v.Clear();
        _drawingTiles.Clear();

        foreach (KeyValuePair<Image, Tile> item in dic)
        {
            int tileX = item.Value.Index % MAX_COUNT;
            int tileY = item.Value.Index / MAX_COUNT;

            if (!_h.Contains(tileY))
            {
                _h.Add(tileY);
            }

            if (!_v.Contains(tileX))
            {
                _v.Add(tileX);
            }
        }

        for (int i = 0; i < _h.Count; i++)
        {
            bool draw = true;
            int y = _h[i];

            for (int j = 0; j < MAX_COUNT; j++)
            {
                int x = j;
                int index = y * MAX_COUNT + x;

                if (_tiles[index].State == TileState.None)
                {
                    draw = false;
                    break;
                }
            }

            if (draw)
            {
                for (int j = 0; j < MAX_COUNT; j++)
                {
                    int x = j;
                    int index = y * MAX_COUNT + x;

                    _tiles[index].ChangeScale(new Vector3(BIG_TILE_SIZE, BIG_TILE_SIZE, BIG_TILE_SIZE));
                    _drawingTiles.Add(_tiles[index]);
                }
            }
        }

        for (int i = 0; i < _v.Count; i++)
        {
            bool draw = true;
            int x = _v[i];

            for (int j = 0; j < MAX_COUNT; j++)
            {
                int y = j;
                int index = y * MAX_COUNT + x;

                if (_tiles[index].State == TileState.None)
                {
                    draw = false;
                    break;
                }
            }

            if (draw)
            {
                for (int j = 0; j < MAX_COUNT; j++)
                {
                    int y = j;
                    int index = y * MAX_COUNT + x;

                    _tiles[index].ChangeScale(new Vector3(BIG_TILE_SIZE, BIG_TILE_SIZE, BIG_TILE_SIZE));
                    _drawingTiles.Add(_tiles[index]);
                }
            }
        }
    }

    public void EraseTemporaryTile()
    {
        foreach (Tile item in _drawingTiles)
        {
            item.ChangeScale(Vector3.one);
        }

        _drawingTiles.Clear();
    }

    public void DelTile(Dictionary<Image, Tile> dic)
    {
        _h.Clear();
        _v.Clear();
        _drawingTiles.Clear();

        int count = 0;

        List<TileData> tileDatas = new List<TileData>();

        foreach (KeyValuePair<Image, Tile> item in dic)
        {
            int tileX = item.Value.Index % MAX_COUNT;
            int tileY = item.Value.Index / MAX_COUNT;

            if (!_h.Contains(tileY))
            {
                _h.Add(tileY);
            }

            if (!_v.Contains(tileX))
            {
                _v.Add(tileX);
            }
        }

        for (int i = 0; i < _h.Count; i++)
        {
            bool del = true;
            int y = _h[i];

            for (int j = 0; j < MAX_COUNT; j++)
            {
                int x = j;
                int index = y * MAX_COUNT + x;

                if (_tiles[index].State == TileState.None)
                {
                    del = false;
                    break;
                }
            }

            if (del)
            {
                for (int j = 0; j < MAX_COUNT; j++)
                {
                    int x = j;
                    int index = y * MAX_COUNT + x;

                    Tile tile = _tiles[index];
                    TileData tileData = new TileData()
                    {
                        tile = tile,
                        tileSprite = tile.TileImage.sprite
                    };
                    tileDatas.Add(tileData);
                }
                count++;
            }
        }

        for (int i = 0; i < _v.Count; i++)
        {
            bool del = true;
            int x = _v[i];

            for (int j = 0; j < MAX_COUNT; j++)
            {
                int y = j;
                int index = y * MAX_COUNT + x;

                if (_tiles[index].State == TileState.None)
                {
                    del = false;
                    break;
                }
            }

            if (del)
            {
                for (int j = 0; j < MAX_COUNT; j++)
                {
                    int y = j;
                    int index = y * MAX_COUNT + x;

                    Tile tile = _tiles[index];
                    TileData tileData = new TileData()
                    {
                        tile = tile,
                        tileSprite = tile.TileImage.sprite
                    };
                    tileDatas.Add(tileData);
                }
                count++;
            }
        }

        foreach (TileData tileData in tileDatas)
        {
            Tile tile = tileData.tile;
            tile.ChangeScale(Vector3.one);
            tile.ResetTile();
        }

        _combo = count == 0 ? 0 : _combo + 1;

        if (tileDatas.Count == 0) return;

        UIManager.Instance.Get<BlockEffectUI>().BlockDelEffect(tileDatas);
        UIManager.Instance.Get<ComboUI>().Show(dic.First().Value.GetComponent<RectTransform>(), _combo);
        GameManager.Instance.Score += count * PLUS_AMOUNT * (_combo * _combo);

        foreach (Tile item in _tiles) if (item.State != TileState.None) return;


        UIManager.Instance.Get<BlockEffectUI>().AllDelEffect(tileDatas[0].tileSprite);
        GameManager.Instance.Score += ALL_DEL_PLUS_AMOUNT;
    }
}
