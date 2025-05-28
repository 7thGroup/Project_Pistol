using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchivementItemAnimation : MonoBehaviour
{
    [SerializeField] private AchivementDataContainer achivementDataContainer;
    
    public void OnEnable()
    {
        achivementDataContainer.image = GetComponent<Image>();

        if (achivementDataContainer.rt == null || achivementDataContainer.image == null)
        {
            Debug.LogWarning("RectTransform or Image is null");
            return;
        }

        // 애니메이션 시작
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Color color = achivementDataContainer.image.color;
        achivementDataContainer.image.color = color;

        // 위치 정의
        Vector2 midPos = new Vector2(760f, -230f);     // 화면
        Vector2 endPos = new Vector2(1200f, -230f);    // 오른쪽 밖

        float riseDuration = 0.5f;
        float waitDuration = 1.2f;
        float exitDuration = 0.4f;


        // 아래 → 중간 (페이드 인)
        float elapsed = 0f;
        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            achivementDataContainer.rt.anchoredPosition = Vector2.Lerp(achivementDataContainer.startPos, midPos, t);
            color.a = t;
            achivementDataContainer.image.color = color;
            yield return null;
        }

        achivementDataContainer.rt.anchoredPosition = midPos;
        color.a = 1f;
        achivementDataContainer.image.color = color;

        // 대기
        yield return new WaitForSeconds(waitDuration);

        // 중간 → 오른쪽 (페이드 아웃)
        elapsed = 0f;
        while (elapsed < exitDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / exitDuration;
            achivementDataContainer.rt.anchoredPosition = Vector2.Lerp(midPos, endPos, t);
            color.a = 1f - t;
            achivementDataContainer.image.color = color;
            yield return null;
        }

        achivementDataContainer.rt.anchoredPosition = endPos;
        color.a = 0f;
        achivementDataContainer.image.color = color;
    }
}
