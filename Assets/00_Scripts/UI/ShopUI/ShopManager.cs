using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ShopManager : SingletonBehaviour<ShopManager>
{
    public bool isOpen = false;

    #region 캐릭터리스트
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
    public GameObject c0005Prefab; // Inspector에서 할당할 프리팹
    public Transform spawnPosition4; // 생성할 위치
    public Transform spawnPosition5; // 생성할 위치
    public int gold;
    bool isC0004 = false;
    bool isC0005 = false;

   
    protected override void Awake()
    {
        base.Awake();
        gold = 1000;
        goldText.text = "Gold: " + gold;
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
    public void CloseShop() // 닫기 버튼
    {
        canvas.gameObject.SetActive(false);
    }

    public void LobbySetCharacter(CharacterSO character)
    {
        // Instantiate(c0004Prefab, spawnPosition.position, spawnPosition.rotation);
        Debug.Log(character.name);
        if (character.name.Equals("C0004"))
        {
            Instantiate(c0004Prefab, spawnPosition4.position, spawnPosition4.rotation);
           
        }
        if (character.name.Equals("C0005"))
        {
            Instantiate(c0005Prefab, spawnPosition5.position, spawnPosition5.rotation);
            
        }
    }
}
