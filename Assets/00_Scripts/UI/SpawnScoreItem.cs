using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnScoreItem : MonoBehaviour
{
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private Transform scorePosition;



    /// <summary>
    /// 점수 올라갈때 Score 띄우는 메서드
    /// </summary>
    public void ShowScoreEffect(bool isHeadShot, int headShotScore, int comboScore, int quickShotScore, int rangeScore)
    {
        if (scorePrefab == null || scorePosition == null) return;

        StartCoroutine(SpawnScoreTexts());

        IEnumerator SpawnScoreTexts()
        {
            List<(string label, int score)> scoreItems = new();

            string hitLabel = isHeadShot ? "Head Shot" : "Body Shot";
            scoreItems.Add((hitLabel, headShotScore));

            if (comboScore > 0)
                scoreItems.Add(("Combo Bonus", comboScore));

            if (quickShotScore > 0)
                scoreItems.Add(("Quick Shot Bonus", quickShotScore));

            if (rangeScore > 0)
                scoreItems.Add(("Range Bonus", rangeScore));

            foreach (var item in scoreItems)
            {
                CreateScoreText(item.label, item.score);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }


    void CreateScoreText(string label, int score)
    {
        var go = ObjectPoolManager.Instance.GetObject(scorePrefab, Vector3.zero, Quaternion.identity, 2f);
        go.transform.SetParent(scorePosition, false);
        go.transform.localRotation = Quaternion.identity;

        go.transform.SetAsLastSibling();

        var text = go.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = $"{label} +{score:0000}";
        }
    }

}


