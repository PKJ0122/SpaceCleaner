using UnityEngine;
using UnityEngine.Pool;

public class PoolObject : MonoBehaviour
{
    IObjectPool<PoolObject> _pool;


    public PoolObject SetPool(IObjectPool<PoolObject> pool)
    {
        _pool = pool;
        return this;
    }

    public virtual void RelasePool()
    {
        _pool.Release(this);
    }
}
