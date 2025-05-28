using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;

public class RankDisplayUI : MonoBehaviour
{
    [SerializeField] private Text userName;
    [SerializeField] private Text scoreText;
    [SerializeField] private RawImage weaponImage;
    [SerializeField] private Text rankText;
    [SerializeField] private RawImage characterImage;

    public void SetUI(int index, DocumentSnapshot data)
    {
        userName.text = data.GetValue<string>("UserName");
        scoreText.text = data.GetValue<int>("BestScore").ToString();
        weaponImage.texture = ResourceManager.Instance.Load<Texture>($"Sprites/{data.GetValue<string>("Weapon")}");
        rankText.text = index.ToString();
        characterImage.texture = ResourceManager.Instance.Load<Texture>($"Sprites/{data.GetValue<string>("Character")}");
    }
}
