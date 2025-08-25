using UnityEngine;

public class ResourcesRepository : Singleton<ResourcesRepository>
{
    BlockDatas _blockDatas;
    public BlockDatas BlockDatas
    {
        get
        {
            if (_blockDatas == null)
            {
                _blockDatas = Resources.Load<BlockDatas>("BlockDatas");
            }

            return _blockDatas;
        }
    }

    CatData _catData;
    public CatData CatData
    {
        get
        {
            if (_catData == null)
            {
                _catData = Resources.Load<CatData>("CatData");
            }

            return _catData;
        }
    }
}
