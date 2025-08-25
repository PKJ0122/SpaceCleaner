using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    #region Score
    const int MIN_REFRESH_POSSIBLE_COUNT = 0;

    int _score;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            OnScoreChange?.Invoke(Score);

            if (value > BestScore)
            {
                BestScore = value;
            }
        }
    }

    public int BestScore
    {
        get
        {
            if (!PlayerPrefs.HasKey("BastScore"))
            {
                PlayerPrefs.SetInt("BastScore", 0);
            }

            return PlayerPrefs.GetInt("BastScore");
        }
        set
        {
            PlayerPrefs.SetInt("BastScore", value);
            RefreshPossibleBestScore = true;
            OnBestScoreChange?.Invoke(value);
        }
    }

    bool _refreshPossibleBestScore;

    public bool RefreshPossibleBestScore
    {
        get => _refreshPossibleBestScore;
        set
        {
            if (_refreshPossibleBestScore == value) return;

            _refreshPossibleBestScore = value;
            OnRefreshBestScore?.Invoke(value);
        }
    }

    public Action<int> OnScoreChange;
    public Action<int> OnBestScoreChange;
    public Action<bool> OnRefreshBestScore;
    #endregion

    readonly Dictionary<Block, bool> _blockDropAvailabilitys = new();
    public Dictionary<Block, bool> BlockDropAvailabilitys => _blockDropAvailabilitys;

    public Action OnGameEnd;


    void Start()
    {
        GameStart();

        OnGameEnd += () =>
        {
            List<Block> keysToUpdate = new List<Block>();
            foreach (KeyValuePair<Block, bool> item in BlockDropAvailabilitys)
            {
                keysToUpdate.Add(item.Key);
            }
            foreach (Block key in keysToUpdate)
            {
                BlockDropAvailabilitys[key] = true;
            }
        };

        OnBestScoreChange?.Invoke(BestScore);
    }

    public void RegisterBlock(Block block)
    {
        if (!BlockDropAvailabilitys.TryAdd(block, true))
        {
            Debug.Log("이미 존재하는 블록을 재등록 하려고 하고있습니다.");
        }
    }

    public void DropComplete(Block block)
    {
        if (!BlockDropAvailabilitys.ContainsKey(block))
        {
            Debug.Log("존재하지 않는 Block 키값으로 값을 수정하려 하고있습니다.");
            return;
        }

        BlockDropAvailabilitys[block] = false;

        foreach (KeyValuePair<Block, bool> item in BlockDropAvailabilitys)
        {
            if (item.Value)
            {
                PossibleBlockLay();
                return;
            }
        }

        List<Block> keysToUpdate = new List<Block>();
        foreach (KeyValuePair<Block, bool> item in BlockDropAvailabilitys)
        {
            keysToUpdate.Add(item.Key);
        }
        foreach (Block key in keysToUpdate)
        {
            BlockDropAvailabilitys[key] = true;
        }

        BlockRefresh();
        PossibleBlockLay();
    }

    public void PossibleBlockLay()
    {
        TileUI tileUI = UIManager.Instance.Get<TileUI>();

        bool possibleBlockLay = false;

        foreach (KeyValuePair<Block, bool> item in BlockDropAvailabilitys)
        {
            if (item.Value)
            {
                if (tileUI.PossibleBlockLay(item.Key.BlockData))
                {
                    possibleBlockLay = true;
                    break;
                }
            }
        }

        if (!possibleBlockLay)
        {
            GameEnd();
        }
    }

    public void GameStart()
    {
        Score = 0;
        _refreshPossibleBestScore = BestScore == MIN_REFRESH_POSSIBLE_COUNT;

        BlockRefresh();
    }

    public void GameEnd()
    {
        UIManager.Instance.Get<ResultUI>().Show();
        OnGameEnd?.Invoke();
    }

    void BlockRefresh()
    {
        ResourcesRepository repository = ResourcesRepository.Instance;
        SoundManager.Instance.SFX_Play(SFX_List.UiUpDown);

        foreach (KeyValuePair<Block, bool> item in BlockDropAvailabilitys)
        {
            Block block = item.Key;

            BlockData[] blockDatas = repository.BlockDatas.blockDatas;
            int blockKind = Random.Range(0, blockDatas.Length);
            BlockData blockData = blockDatas[blockKind];

            CatData catData = repository.CatData;
            int catKind = Random.Range(0, repository.CatData.CatSprites.Length);
            Sprite catSprite = catData.CatSprites[catKind];

            block.enabled = true;
            block.ChangeBlockData(blockData, catKind, catSprite);
        }
    }
}
