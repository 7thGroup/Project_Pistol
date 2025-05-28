using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class StartSceneCameraController : MonoBehaviour
{
    [SerializeField] private List<CinemachineVirtualCamera> vCamList;
    [SerializeField] private List<CinemachineDollyCart> cartList;

    private int curIdx;
    private int maxIdx;
    
    private float speed;
    
    private void Awake()
    {
        curIdx = 0;
        maxIdx = vCamList.Count;
        speed = 0.2f;
    }

    private void Update()
    {
        CirculateCamera();
    }

    private void CirculateCamera()
    {
        cartList[curIdx].m_Position += speed * Time.deltaTime;
        if (cartList[curIdx].m_Position >= 1f)
        {
            var prevCart = cartList[curIdx];
            vCamList[curIdx].gameObject.SetActive(false);
            curIdx = (curIdx + 1) % maxIdx;
            vCamList[curIdx].gameObject.SetActive(true);
            vCamList[curIdx].gameObject.SetActive(true);
            prevCart.m_Position = 0f;
        }
    }
}
