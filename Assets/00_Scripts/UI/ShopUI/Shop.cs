using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Shop : MonoBehaviour
{

    [SerializeField] public List<CharacterSO> characterItem;
    [SerializeField] private List<CharacterUI> characterUIs;
    public GameObject content;
    [SerializeField] private bool isShow = false;
    public TextMeshProUGUI goldText;
    public GameObject characterItemPrefab; // 캐릭터 아이템 UI 프리팹
    public Transform itemParent; // UI를 넣을 부모 오브젝트 (ScrollView의 Content 등)
    public GameObject canvas;
    public Shop()
    {

    }
    private void Start()
    {

        goldText.text = "Gold: " + GameManager.Instance.gold;
        for (int i = 0; i < characterItem.Count; i++)
        {
            //characterUIs[i].Setup(characterItem[i]); // 여기에 SO 넘겨줌
        }
    }
    public void ShowContent()
    {
        isShow = !isShow;              // 현재 상태를 반전시킴
        content.SetActive(isShow);     // 반영된 상태로 콘텐츠 활성화/비활성화
    }
    public void CloseShop()
    {
        canvas.gameObject.SetActive(false);
    }
}
