using UnityEngine;

public class AerialTarget : BaseTarget
{
    private Vector3 hitDirection;
    private Rigidbody rb;
    [SerializeField] private GameObject targetObj;
    [SerializeField] private Transform targetPoint;
    
    private DroneMovement droneMovement;

    private void Awake()
    {
        droneMovement = GetComponent<DroneMovement>();
        droneMovement.enabled = false;
    }

    protected override void Start()
    {
        base.Start();

        rb = targetObj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = targetObj.gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void OnDisable()
    {
        droneMovement.enabled = false;
    }

    public override void TakeDamage(float amount, Collider hitCollider, Vector3 hitDirection)
    {
        if (currentHp <= 0) return;
        StageManager.Instance.HitCount++;
        StageManager.Instance.HeadHitCount++;
        AnalyticsManager.Instance.roomHitCount++;
        AnalyticsManager.Instance.headHitCount++;
        amount *= data.DamageRate;

        float realDamage = Mathf.Min(amount, currentHp);
        currentHp -= realDamage;

        hpBar.fillAmount = currentHp / data.Hp;
        
        if (currentHp > 0)
        {
            //anim.SetTrigger("Hit");
        }
        else
        {
            StageManager.Instance.DestroyTargetCombo++;
            AnalyticsManager.Instance.roomCombo++;
            if (StageManager.Instance.MaxDestroyTargetCombo <= StageManager.Instance.DestroyTargetCombo)
            {
                StageManager.Instance.MaxDestroyTargetCombo = StageManager.Instance.DestroyTargetCombo;
            }
            this.hitDirection = hitDirection;
            Die();
            AddForceTarget();
        }

        int totalScore = (int)(BaseScore(false, realDamage) + RangeScore() + ComboScore(StageManager.Instance.DestroyTargetCombo) + QuickShotScore(StageManager.Instance.IsQuickShot));
        StageManager.Instance.GameScore += totalScore;
        AnalyticsManager.Instance.roomScore += totalScore;
        var hudUI = UIManager.Instance.GetMainUI<HUDUI>();

        int headShotScore = (int)BaseScore(false, realDamage);
        int comboScore = (int)ComboScore(StageManager.Instance.DestroyTargetCombo);
        int quickShotScore = (int)QuickShotScore(StageManager.Instance.IsQuickShot);
        
        hudUI?.spawnScoreItem.ShowScoreEffect(false, headShotScore, comboScore, quickShotScore, (int)RangeScore());
        
        StageManager.Instance.IsQuickShot = true;
        StageManager.Instance.QuickShotTimer = 0f;

        if (currentHp <= 0)
        {
            this.hitDirection = hitDirection;
            Die();
            AddForceTarget();
        }
    }


    private void AddForceTarget() //타겟이 총알의 방향대로 날아감
    {
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;

            rb.AddForce(hitDirection * 5f + Vector3.up, ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * 5f, ForceMode.Impulse);
        }
    }

    public override void InitData(TargetSO data)
    {
        base.InitData(data);

        if (rb == null && targetObj != null)
        {
            rb = targetObj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = targetObj.gameObject.AddComponent<Rigidbody>();
            }
        }

        rb.useGravity = false;
        rb.isKinematic = true;

        targetObj.transform.position = targetPoint.position;
        targetObj.transform.rotation = targetPoint.rotation;
        
        droneMovement.enabled = true;
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