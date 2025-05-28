using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CharacterUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public Text priceText;
    public CharacterSO data;
    public TextMeshProUGUI alarmText;
    public TextMeshProUGUI description;
    public int CharacterCost;
    public Canvas canvas;
    public GameObject alarmPanel;
    private void Start()
    {
        SetData();

    }

    void SetData()
    {
        nameText.text = data.name;
        priceText.text = data.Cost.ToString();
        description.text =
                $"Handling: {data.HDL,-3}\nRecoil: {data.RCL,-3}\n" +
                $"Step: {data.STP,-3}\nSpeed: {data.SPD,-3}";

    }
    public void OnBuy()
    {
        Debug.Log($"구매: {data.name}");

        if (GameManager.Instance.gold >= data.Cost)
        {
            GameManager.Instance.gold -= data.Cost;
            GameManager.Instance.Shop.goldText.text = "Gold: " + GameManager.Instance.gold;
            GameManager.Instance.Shop.characterItem.Add(data);
            Destroy(gameObject);
        }
        else
        {

            alarmText.text = "재화가 부족합니다!";
            alarmPanel.SetActive(true);
            StartCoroutine(TextTimer());

        }
    }

    IEnumerator TextTimer() // 재화 부족 알림 텍스트
    {
        yield return new WaitForSeconds(3f);
        alarmPanel.SetActive(false);
    }

    public void CloseShop()
    {
        canvas.gameObject.SetActive(false);
    }
}
