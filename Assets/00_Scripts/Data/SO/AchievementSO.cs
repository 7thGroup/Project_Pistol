using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "AchievementSO", menuName = "SO/AchievementSO")]
public class AchievementSO : ScriptableObject
{
    public string ID;
    public string Name;
    public string Description;
}

[System.Serializable]
public class AchievementData
{
    public string ID;
    public string Name;
    public string Description;
}
