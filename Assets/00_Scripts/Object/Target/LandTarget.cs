using UnityEngine;

public class LandTarget : BaseTarget
{
    public override void TakeDamage(float amount, Collider hitCollider, Vector3 hitDirection)
    {
        if (currentHp <= 0) return;

        StageManager.Instance.HitCount++;
        AnalyticsManager.Instance.headHitCount++;
        bool isHeadShot;

        if (hitCollider != null && hitCollider.name == "Head")
        {
            amount = Mathf.RoundToInt(amount * data.DamageRate * 1.2f);
            isHeadShot = true;
            StageManager.Instance.HeadHitCount++;
        }
        else
        {
            amount *= data.DamageRate;
            isHeadShot = false;
        }

        float realDamage = Mathf.Min(amount, currentHp);
        currentHp -= realDamage;

        hpBar.fillAmount = currentHp / data.Hp;

        if (currentHp > 0)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            StageManager.Instance.DestroyTargetCombo++;
            AnalyticsManager.Instance.roomCombo++;
            if (StageManager.Instance.MaxDestroyTargetCombo <= StageManager.Instance.DestroyTargetCombo)
            {
                StageManager.Instance.MaxDestroyTargetCombo = StageManager.Instance.DestroyTargetCombo;
            }
            Die();
        }

        int totalScore = (int)(BaseScore(isHeadShot, realDamage) + RangeScore() + ComboScore(StageManager.Instance.DestroyTargetCombo) + QuickShotScore(StageManager.Instance.IsQuickShot));
        StageManager.Instance.GameScore += totalScore;
        AnalyticsManager.Instance.roomScore += totalScore;
        var hudUI = UIManager.Instance.GetMainUI<HUDUI>();

        int headShotScore = (int)BaseScore(isHeadShot, realDamage);
        int comboScore = (int)ComboScore(StageManager.Instance.DestroyTargetCombo);
        int quickShotScore = (int)QuickShotScore(StageManager.Instance.IsQuickShot);
        
        hudUI?.spawnScoreItem.ShowScoreEffect(isHeadShot, headShotScore, comboScore, quickShotScore, (int)RangeScore());

        StageManager.Instance.IsQuickShot = true;
        StageManager.Instance.QuickShotTimer = 0f;
    }

    private float BaseScore(bool isHeadShot, float dmg)
    {
        return isHeadShot ? dmg * 1.5f : dmg;
    }

    private float RangeScore()
    {
        var dis = Vector3.Distance(StageManager.Instance.Player.transform.position, transform.position);
        return dis >= 2f ? 200 + Mathf.Floor((dis - 2f) / 0.1f) * 10f : 0;
    }

    private float ComboScore(int combo)
    {
        return combo >= 2 ? Mathf.Min(combo * 100f, 900f) : 0f;
    }

    private float QuickShotScore(bool isQuickShot)
    {
        return isQuickShot ? 500f : 0f;
    }
}
