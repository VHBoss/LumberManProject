using TMPro;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    [SerializeField] TMP_Text countText;
    [SerializeField] RectTransform progress;

    private float sizeX;

    public void SetCount(int count, int totalCount)
    {
        if (sizeX == 0) sizeX = progress.sizeDelta.x;

        countText.text = count + "/" + totalCount;
        progress.sizeDelta = new Vector2(count / (float)totalCount * sizeX, progress.sizeDelta.y);
    }

    public void SetCount(float count, float totalCount)
    {
        if (sizeX == 0) sizeX = progress.sizeDelta.x;

        countText.text = count + "/" + totalCount;
        progress.sizeDelta = new Vector2(count / totalCount * sizeX, progress.sizeDelta.y);
    }

    public void SetCount(int count)
    {
        countText.text = count.ToString();
    }

    public void SetCountSimple(float count, float totalCount)
    {
        if (sizeX == 0) sizeX = progress.sizeDelta.x;

        countText.text = count.ToString("0");
        progress.sizeDelta = new Vector2(count / totalCount * sizeX, progress.sizeDelta.y);
    }
}
