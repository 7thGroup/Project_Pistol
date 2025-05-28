using DataDeclaration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemCard : MonoBehaviour
{
    private ItemSO data;
    [SerializeField] private Image rewardImage;
    [SerializeField] private TextMeshProUGUI rewardName;
    [SerializeField] private TextMeshProUGUI rewardDescription;
    [SerializeField] private TextMeshProUGUI timeCost;
    [SerializeField] private Button rewardButton;
    
    public Button RewardButton => rewardButton;

    private void Awake()
    {
        rewardButton.onClick.AddListener(OnClickRewardButton);
    }

    public void SetItemData(ItemSO item)
    {
        data = item;
        var itemImage = ResourceManager.Instance.Load<Sprite>($"Sprites/Item/{item.ID}");
        rewardDescription.text = item.Description;
        rewardImage.sprite = itemImage;
        rewardName.text = item.Name;
        timeCost.text = $"비용: 제한 시간 -{item.cost:N2}s";
    }

    private void OnClickRewardButton()
    {
        if (data.ApplicationTarget == (int)ItemApplyType.Player)
        {
            StageManager.Instance.Player.Stat.IncreaseStat(data.RCL, data.HDL, data.STP, data.SPD);
            ((HUDUI)UIManager.Instance.CurMainUI).UpdateStatValue();
        }
        else
        {
           //StageManager.Instance.Player.Weapon.Stat.ChangeStat(data.DMG, data.ShootRecoil, data.MaxAmmo, data.WeaponParts);
            if (data.WeaponParts > 0) // 무기 파츠면
            {
                var part = FindWeaponPartByItemSO(data.ID);
                if (part != null)
                {
                    StageManager.Instance.Player.Weapon.ToggleAttachment(part.name);
                }
                else
                {
                    Debug.LogWarning($"무기 파츠 {data.ID}를 게임오브젝트 찾을 수 없습니다.");
                }
            }
        }
        // 비용 차감
        StageManager.Instance.RemainTime -= data.cost;
    }
    
    private GameObject FindWeaponPartByItemSO(string itemID)
    {
        var weapon = StageManager.Instance.Player.Weapon;

        var children = weapon.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.gameObject.name.Equals(itemID, System.StringComparison.OrdinalIgnoreCase))
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
