using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;
using Query = Firebase.Database.Query;

public class PopupRankingBoard : PopupUI
{
    [SerializeField] private GameObject contentsPos;
    [SerializeField] private RankDisplayUI contentUI;

    private QuerySnapshot snapshot;

    private async void OnEnable()
    {
        for (int i = contentsPos.transform.childCount - 1; i >= 0; i--)
        {
            var child = contentsPos.transform.GetChild(i).gameObject;
            ObjectPoolManager.Instance.ReturnToPool(child);
        }

        try
        {
            await InitAsync();
            SetRank();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void SetRank()
    {
        try
        {
            var sortedDocs = snapshot.Documents
                .OrderByDescending(doc => doc.GetValue<double>("BestScore"))
                .ToList(); // 리스트로 고정

            var scoreItems = new List<RankDisplayUI>();

            foreach (var document in sortedDocs)
            {
                var scoreItem = ObjectPoolManager.Instance.GetObject<RankDisplayUI>(
                    contentUI, Vector3.zero, Quaternion.identity);

                scoreItem.transform.SetParent(contentsPos.transform, false);
                scoreItem.SetUI(scoreItems.Count + 1, document);

                scoreItems.Add(scoreItem);
            }

            // 하이어라키 순서를 최종적으로 보장
            for (int i = 0; i < scoreItems.Count; i++)
            {
                scoreItems[i].transform.SetSiblingIndex(i);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;
        }
    }

    private async Task InitAsync()
    {
        var query = FirebaseManager.Instance.DB.CollectionGroup("rank").WhereGreaterThan("BestScore", 0).OrderByDescending("BestScore").Limit(20);
        try
        {
            snapshot = await query.GetSnapshotAsync(Source.Server);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
