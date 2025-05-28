using UnityEngine;
using UnityEngine.UI;

public abstract class BaseTarget : MonoBehaviour, IPoolable
{
    [Header("타겟 데이터")]
    [SerializeField] protected TargetSO data;
    protected float currentHp;
    [SerializeField] protected Animator anim;

    [Header("사운드")]
    [SerializeField] protected AudioClip downSound;

    [Header("UI")]
    [SerializeField] protected Image hpBar;
    [SerializeField] protected GameObject targetUI;
    [SerializeField] protected GameObject blip;
    [SerializeField] protected Text lvText;

    [SerializeField] private Transform visual;

    public event System.Action<BaseTarget> OnTargetDisabled;



    protected virtual void Start()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        }
        InitData(data);
    }

    private void OnDisable()
    {
        OnTargetDisabled?.Invoke(this);
    }


    private void Update()
    {
        {
            if (StageManager.Instance.Player && currentHp > 0)
            {
                var origin = transform.eulerAngles;
                transform.LookAt(StageManager.Instance.Player.transform);
                transform.rotation = Quaternion.Euler(new Vector3(origin.x, transform.rotation.eulerAngles.y, origin.z));
            }
        }
    }

    public abstract void TakeDamage(float amount, Collider hitCollider, Vector3 hitDirection);

    // public void OnPlayerEnteredRange()
    // {
    //     if (currentHp <= 0) return;

    // }

    protected virtual void Die()
    {

        if (anim != null)
        {
            anim.SetBool("Die", true);
        }

        blip.SetActive(false);
        if (targetUI != null)
        {

        }
        targetUI.SetActive(false);
        SoundManager.Instance.PlaySFXForClip(downSound, gameObject.transform.position);
        Invoke(nameof(DeactivateTarget), 2f);

    }

    public virtual void InitData(TargetSO data)
    {
        ActivateAll();

        this.data = data;
        currentHp = data.Hp;

        if (targetUI != null)
        {
            lvText.text = $"Lv.{data.Level}";
            if (hpBar != null)
            {
                hpBar.fillAmount = 1f;
            }
        }
    }

    private void DeactivateTarget()
    {
        gameObject.SetActive(false);
    }

    private void ActivateAll()
    {
        if (targetUI != null)
        {
            gameObject.SetActive(true);
            targetUI.SetActive(true);
        }

        blip.SetActive(true);
    }

    public void OnGetFromPool()
    {
    }

    public void OnReturnToPool()
    {
        // 풀로 반환될 때
        if (anim != null)
        {
            anim.SetBool("Die", false);
            Debug.Log("Die");
        }
        InitData(data); // 초기 상태로 복원
    }
}
