using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreTextAnimation : MonoBehaviour, IPoolable
{
    private RectTransform animatedRt;
    public Image image;
    public TextMeshProUGUI text;

    public void OnGetFromPool()
    {
        Transform child = transform.childCount > 0 ? transform.GetChild(0) : null;

        if (child == null)
        {
            Debug.LogWarning("Child가 없음");
            return;
        }

        animatedRt = child.GetComponent<RectTransform>();
        image = child.GetComponent<Image>();
        text = child.GetComponentInChildren<TextMeshProUGUI>();

        StopAllCoroutines(); // 안전 장치
        StartCoroutine(AnimateScoreEntry());
    }

    public void OnReturnToPool()
    {
        StopAllCoroutines();
    }

    private IEnumerator AnimateScoreEntry()
    {
        Vector3 startPos = new Vector3(0, -70f, 0);
        Vector3 midPos = Vector3.zero;
        float riseDuration = 0.5f;
        float elapsed = 0f;

        animatedRt.localPosition = startPos;
        var color = image.color;
        color.a = 0f;
        image.color = color;

        if (text != null)
        {
            var textColor = text.color;
            textColor.a = 0f;
            text.color = textColor;
        }

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            animatedRt.localPosition = Vector3.Lerp(startPos, midPos, t);
            color.a = t;
            image.color = color;

            if (text != null)
            {
                var tc = text.color;
                tc.a = t;
                text.color = tc;
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        Vector3 leftPos = new Vector3(-400f, 0f, 0f);
        float shiftDuration = 0.3f;
        elapsed = 0f;

        while (elapsed < shiftDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shiftDuration;
            animatedRt.localPosition = Vector3.Lerp(midPos, leftPos, t);
            color.a = 1f - t;
            image.color = color;

            if (text != null)
            {
                var tc = text.color;
                tc.a = 1f - t;
                text.color = tc;
            }

            yield return null;
        }
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }
}
