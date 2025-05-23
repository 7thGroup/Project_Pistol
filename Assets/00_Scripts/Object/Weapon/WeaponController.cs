using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private Weapon weapon;
    private WeaponStatHandler stat;
    private int curAmmo;
    private float lastFireTime;
    [HideInInspector] public bool isReloading;

    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    private GameObject muzzleFlashPrefab;
    private GameObject casingPrefab;
    private GameObject bulletImpactPrefab;

    public int CurAmmo => curAmmo;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();

        muzzleFlashPrefab = ResourceManager.Instance.Load<GameObject>("Prefabs/FX/MuzzleFlash");
        casingPrefab = ResourceManager.Instance.Load<GameObject>("Prefabs/Props/Case");
        bulletImpactPrefab = ResourceManager.Instance.Load<GameObject>("Prefabs/FX/BulletHole");

        isReloading = false;
    }

    private void Start()
    {
        stat = weapon.Stat;
        curAmmo = stat.MaxAmmo;
    }

    #region 발사

    public void Fire(bool isAds)
    {
        lastFireTime = Time.time;
        if (lastFireTime < 1f)
        {
            return;
        }

        if (curAmmo > 0)
        {
            if (curAmmo != 1)
            {
                weapon.Anim.SetTrigger("Fire");
            }
            else if (curAmmo == 1)
            {
                weapon.Anim.SetBool("OutOfAmmo", true);
            }
            else
            {
                weapon.Anim.SetBool("OutOfAmmo", true);
            }

            SoundManager.Instance.PlaySFXForName(weapon.Data.name, barrelLocation.position, gameObject);
            ShootRay(isAds);
            EjectCasing();
            MuzzleFlash();
            StageManager.Instance.ShotCount++;
            AnalyticsManager.Instance.roomShotCount++;

            curAmmo--;
        }
        else
        {
            // 탄창 없을 경우
            SoundManager.Instance.PlaySFXForName("EmptyTrigger", barrelLocation.position, gameObject);
        }

        lastFireTime = 0f;
    }

    private void ShootRay(bool isAds)
    {
        Vector3 shootDirection;

        shootDirection = barrelLocation.forward;

        Vector3 loweredPosition = barrelLocation.position;
        Ray ray = new Ray(loweredPosition, shootDirection);
        Debug.DrawRay(loweredPosition, shootDirection * 100f, Color.red,30f ); 
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 탄흔 처리
            if (bulletImpactPrefab)
            {
                Quaternion hitRotation = Quaternion.LookRotation(hit.normal);
                GameObject impact = ObjectPoolManager.Instance.GetObject(bulletImpactPrefab, hit.point, hitRotation, 5f);
                impact.transform.SetParent(hit.collider.transform);

                // AutoReturn 처리
                //ObjectPoolManager.Instance.AutoReturnToPool(impact, 5f);  // 5초 후 반환
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                BaseTarget target = hit.collider.GetComponentInParent<BaseTarget>();
                
                Vector3 hitDir = (hit.point - barrelLocation.position).normalized; //힘을 줘야 할 방향
                target?.TakeDamage(stat.Damage, hit.collider, hitDir);
            }
            else
            {
                StageManager.Instance.DestroyTargetCombo = 0;
            }
        }
        else
        {
            StageManager.Instance.DestroyTargetCombo = 0;
        }
    }

    private void EjectCasing()
    {
        if (casingPrefab && casingExitLocation)
        {
            GameObject casing = ObjectPoolManager.Instance.GetObject(casingPrefab,
                casingExitLocation.position,
                casingExitLocation.rotation,
                6f
            );
            
            ShellCasing sc = casing.GetComponent<ShellCasing>();

            if (sc != null)
            {
                
                float power = stat.Damage * 40f;
                Vector3 direction = -casingExitLocation.right * 0.3f - casingExitLocation.up * 0.8f;
                sc.SetEjectData(power, casingExitLocation.position, direction);
            }
        }
    }

    private void MuzzleFlash()
    {
        if (muzzleFlashPrefab)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            flash.transform.SetParent(barrelLocation);
        }
    }

    #endregion

    #region 장전

    public void ReloadWeapon(bool isAds)
    {
        if (curAmmo == stat.MaxAmmo && isAds)
        {
            return;
        }

        curAmmo = 0;
        weapon.Anim.SetTrigger("Reload");

        SoundManager.Instance.PlaySFXForName("Reload", barrelLocation.position, gameObject);

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(stat.ReloadTime);

        weapon.Anim.SetBool("OutOfAmmo", false);

        curAmmo = stat.MaxAmmo;
    }

    #endregion
}
