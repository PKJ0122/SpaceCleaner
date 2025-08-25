using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    Image _tileImage;
    public Image TileImage => _tileImage;

    int _index = -1;
    public int Index
    {
        get => _index;
        set => _index = value;
    }

    TileState _state;
    public TileState State => _state;


    public void Awake()
    {
        _tileImage = transform.Find("Image").GetComponent<Image>();
    }

    public void ChangeTemporaryTile(Sprite sprite)
    {
        _tileImage.sprite = sprite;
        _tileImage.color = new Color(1f, 1f, 1f, 0.5f);
        _state = TileState.Half;
    }

    public void ChangeTile(Sprite sprite)
    {
        _tileImage.sprite = sprite;
        _tileImage.color = new Color(1f, 1f, 1f, 1f);
        _state = TileState.Full;
    }

    public void ResetTile()
    {
        _tileImage.sprite = null;
        _tileImage.color = new Color(1f, 1f, 1f, 0f);
        _state = TileState.None;
    }

    public void ChangeScale(Vector3 scale)
    {
        _tileImage.transform.localScale = scale;
    }
}
