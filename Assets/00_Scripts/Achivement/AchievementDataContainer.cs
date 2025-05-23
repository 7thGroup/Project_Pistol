using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AchivementDataContainer : MonoBehaviour, IPoolable
{
    public TextMeshProUGUI titleText;
    public RectTransform rt;
    public Image image;
    public Vector2 startPos = new Vector2(760f, -330f);


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = startPos;
    }
    public void SetData(AchievementSO so)
    {
        titleText.text = so.Name;
    }
    public void OnGetFromPool()
    {
        var textColor = titleText.color;
        textColor.a = 0f;

        var img = GetComponent<Image>();
        if (img != null)
        {
            var color = img.color;
            color.a = 0;
            img.color = color;
        }


        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
    public void OnReturnToPool() { }
}
