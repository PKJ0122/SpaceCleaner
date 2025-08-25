using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public BlockData BlockData;
    public Button button;
    public Image image;
    public GridLayoutGroup GridLayoutGroup;
    public Sprite sprite;

    Image[] images;

    public void Awake()
    {
        images = image.GetComponentsInChildren<Image>(true)
                      .Where(image => image.gameObject != this.image.gameObject)
                      .ToArray();

        foreach (var item in images)
        {
            Button button = item.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                int index = Array.IndexOf(images, item);
                Debug.Log($"X : {index / BlockData.H} , Y : {index % BlockData.H}");
            });
        }

        button.onClick.AddListener(() =>
        {
            foreach (var item in images)
            {
                item.gameObject.SetActive(false);
                item.sprite = null;
            }

            int blockEa = BlockData.H * BlockData.V;
            GridLayoutGroup.constraintCount = BlockData.H;

            for (int i = 0; i < blockEa; i++)
            {
                images[i].gameObject.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(GridLayoutGroup.GetComponent<RectTransform>());
            foreach (var item in BlockData.blockCoordinates)
            {
                int index = BlockData.H * item.Y + item.X;
                images[index].sprite = sprite;
            }
        });
    }
}
