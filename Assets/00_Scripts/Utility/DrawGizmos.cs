#if UNITY_EDITOR
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        // 길이를 보기 좋게 1이 아닌 3 정도로 늘려서 그리기
        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.forward * 100f;

        Gizmos.DrawLine(start, end);
    }
}
#endif
