using UnityEngine;

[CreateAssetMenu(fileName = "BlockData",menuName = "ScriptableObject/BlockData")]
public class BlockData : ScriptableObject
{
    public int H;
    public int V;
    public BlockCoordinate[] blockCoordinates;
}