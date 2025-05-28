using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ShopManager : SingletonBehaviour<ShopManager>
{
    public bool isOpen = false;

    #region
    [SerializeField] public List<CharacterSO> characterItem;
    [SerializeField] private List<CharacterUI> characterUIs;
    [SerializeField] public List<GameObject> characterPrefab;
    #endregion 
    public GameObject content;
    [SerializeField] private bool isShow = false;
    public TextMeshProUGUI goldText;
    public GameObject characterItemPrefab; // 캐릭터 아이템 UI 프리팹
    public Transform itemParent; // UI를 넣을 부모 오브젝트 (ScrollView의 Content 등)
    public GameObject canvas;

    public GameObject c0004Prefab; // Inspector에서 할당할 프리팹
    public Transform spawnPosition; // 생성할 위치

    public int gold = 3000;
    protected override void Awake()
    {
        base.Awake();
        goldText.text = "Gold: " + gold;

        foreach (CharacterSO character in characterItem)
        {
            if (character.name == "c0004")
            {
                Instantiate(c0004Prefab, spawnPosition.position, spawnPosition.rotation, itemParent);
            }
        }
    }

    public void ShowContent()
    {
        isShow = !isShow;              // 현재 상태를 반전시킴
        content.SetActive(isShow);     // 반영된 상태로 콘텐츠 활성화/비활성화
    }
    public void SetShop() // 상점 view
    {
        if (isOpen)
        {
            canvas.gameObject.SetActive(false);
            isOpen = false;
        }
        else
        {
            canvas.gameObject.SetActive(true);
            isOpen = true;
        }
    }
    public void CloseShop()
    {
        canvas.gameObject.SetActive(false);
    }
}
