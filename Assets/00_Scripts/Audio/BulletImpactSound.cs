using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class BulletImpactSound : MonoBehaviour ,IPoolable
{
    //TODO: 재질 별로 소리 다르게
    [SerializeField] private AudioClip[] bulletImpactSFX;

    [SerializeField] private AudioClip[] bulletImpactToWoodSFX;
    [SerializeField] private AudioClip[] bulletImpactToMetalSFX;
    [SerializeField] private AudioClip[] bulletImpactToSandSFX;
    [SerializeField] private GameObject pooledAudioPrefab;


    public void OnGetFromPool()
    {
        Invoke(nameof(PlayRandomImpactSound), 0.1f);
    }
    public void OnReturnToPool() { }

    private void PlayRandomImpactSound()
    {
        Transform parent = transform.parent;
        AudioClip[] sfxArray = bulletImpactSFX;

        if (parent != null)
        {
            if (parent.CompareTag("Sand"))
            {
                sfxArray = bulletImpactToSandSFX;
            }
            else if (parent.CompareTag("Target") || parent.CompareTag("Metal"))
            {
                sfxArray = bulletImpactToMetalSFX;
            }
            else if (parent.CompareTag("Wood"))
            {
                sfxArray = bulletImpactToWoodSFX;

            }
        }

        if (sfxArray == null || sfxArray.Length == 0) return;

        int randomIndex = Random.Range(0, sfxArray.Length);
        SoundManager.Instance.PlaySFXForClip(sfxArray[randomIndex], gameObject.transform.position);

    }
}
