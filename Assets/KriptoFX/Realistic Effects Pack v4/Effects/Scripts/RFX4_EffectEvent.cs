using UnityEngine;

public class RFX4_EffectEvent : MonoBehaviour
{
    public GameObject CharacterEffect;
    public Transform CharacterAttachPoint;
    public float CharacterEffect_DestroyTime = 10;
    [Space]

    public GameObject CharacterEffect2;
    public Transform CharacterAttachPoint2;
    public float CharacterEffect2_DestroyTime = 10;
    [Space]

    public GameObject MainEffect;
    public Transform AttachPoint;
    public Transform OverrideAttachPointToTarget;
    public float Effect_DestroyTime = 10;
    [Space]

    public GameObject AdditionalEffect;
    public Transform AdditionalEffectAttachPoint;
    public float AdditionalEffect_DestroyTime = 10;

    [HideInInspector] public bool IsMobile;

    public void AssignEffect(GameObject effectPrefab)
    {
        MainEffect = effectPrefab;
    }

    public void ActivateEffect(SpellType spellType, Transform parent)
    {
        if (MainEffect == null && spellType != SpellType.Nox || AttachPoint == null)
        {
            return;
        }

        GameObject instance = null;

        if (OverrideAttachPointToTarget == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                Vector3 targetPoint;

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = ray.origin + ray.direction * 100f;
                }

                Vector3 dirFromCamera = (targetPoint - cam.transform.position).normalized;
                if (dirFromCamera.sqrMagnitude < 0.0001f)
                {
                    dirFromCamera = cam.transform.forward;
                }

                switch (spellType)
                {
                    case SpellType.DirectionBased:
                        {
                            Vector3 dirFromAttach = (targetPoint - AttachPoint.position).normalized;
                            if (dirFromAttach.sqrMagnitude < 0.0001f)
                            {
                                dirFromAttach = dirFromCamera;
                            }

                            instance = Instantiate(
                                MainEffect,
                                AttachPoint.position,
                                Quaternion.LookRotation(dirFromAttach));
                            break;
                        }

                    case SpellType.TargetBased:
                        {
                            Vector3 dirToTarget = (targetPoint - AttachPoint.position).normalized;
                            if (dirToTarget.sqrMagnitude < 0.0001f)
                            {
                                dirToTarget = dirFromCamera;
                            }

                            instance = Instantiate(
                                MainEffect,
                                AttachPoint.position,
                                Quaternion.LookRotation(dirToTarget));
                            break;
                        }

                    case SpellType.PositionBased:
                        {
                            Vector3 spawnPos = AttachPoint.position;
                            if (Physics.Raycast(
                                    AttachPoint.position + Vector3.up * 0.1f,
                                    Vector3.down,
                                    out RaycastHit groundHit,
                                    5f))
                            {
                                spawnPos.y = groundHit.point.y;
                            }

                            instance = Instantiate(
                                MainEffect,
                                spawnPos,
                                Quaternion.identity);
                            break;
                        }
                    case SpellType.Lumos:
                        {
                            instance = Instantiate(
                                MainEffect,
                                parent.GetChild(0).transform.position,
                                Quaternion.identity, parent.GetChild(0));
                            break;
                        }
                    case SpellType.Nox:
                        {
                            if (parent.GetChild(0).childCount > 0)
                                Destroy(parent.GetChild(0).GetChild(0).gameObject);
                            break;
                        }
                    default:
                        {
                            instance = Instantiate(
                                MainEffect,
                                AttachPoint.position,
                                Quaternion.LookRotation(dirFromCamera));
                            break;
                        }
                }
            }
            else
            {
                switch (spellType)
                {
                    case SpellType.PositionBased:
                        {
                            Vector3 spawnPos = AttachPoint.position;
                            if (Physics.Raycast(
                                    AttachPoint.position + Vector3.up * 0.1f,
                                    Vector3.down,
                                    out RaycastHit groundHit,
                                    5f))
                            {
                                spawnPos.y = groundHit.point.y;
                            }

                            instance = Instantiate(
                                MainEffect,
                                spawnPos,
                                Quaternion.identity);
                            break;
                        }

                    case SpellType.DirectionBased:
                    case SpellType.TargetBased:
                    default:
                        {
                            instance = Instantiate(
                                MainEffect,
                                AttachPoint.position,
                                Quaternion.LookRotation(AttachPoint.forward));
                            break;
                        }
                }
            }
        }
        else
        {
            // Explicit override target
            Vector3 toTarget = (OverrideAttachPointToTarget.position - AttachPoint.position).normalized;
            if (toTarget.sqrMagnitude < 0.0001f)
            {
                toTarget = Vector3.forward;
            }

            instance = Instantiate(
                MainEffect,
                AttachPoint.position,
                Quaternion.LookRotation(toTarget));
        }

        if (instance == null)
        {
            return;
        }

        UpdateEffectForMobileIsNeed(instance);
        if (Effect_DestroyTime > 0.01f)
        {
            Destroy(instance, Effect_DestroyTime);
        }
    }

    public void ActivateAdditionalEffect()
    {
        if (AdditionalEffect == null) return;
        if (AdditionalEffectAttachPoint != null)
        {
            var instance = Instantiate(
                AdditionalEffect,
                AdditionalEffectAttachPoint.transform.position,
                AdditionalEffectAttachPoint.transform.rotation);
            UpdateEffectForMobileIsNeed(instance);
            if (AdditionalEffect_DestroyTime > 0.01f) Destroy(instance, AdditionalEffect_DestroyTime);
        }
        else AdditionalEffect.SetActive(true);
    }

    public void ActivateCharacterEffect()
    {
        if (CharacterEffect == null || CharacterAttachPoint == null) return;
        var instance = Instantiate(
            CharacterEffect,
            CharacterAttachPoint.transform.position,
            CharacterAttachPoint.transform.rotation,
            CharacterAttachPoint.transform);
        UpdateEffectForMobileIsNeed(instance);
        if (CharacterEffect_DestroyTime > 0.01f) Destroy(instance, CharacterEffect_DestroyTime);
    }

    public void ActivateCharacterEffect2()
    {
        if (CharacterEffect2 == null || CharacterAttachPoint2 == null) return;
        var instance = Instantiate(
            CharacterEffect2,
            CharacterAttachPoint2.transform.position,
            CharacterAttachPoint2.transform.rotation,
            CharacterAttachPoint2);
        UpdateEffectForMobileIsNeed(instance);
        if (CharacterEffect2_DestroyTime > 0.01f) Destroy(instance, CharacterEffect2_DestroyTime);
    }

    void UpdateEffectForMobileIsNeed(GameObject instance)
    {
        //if (IsMobile)
        {
            var effectSettings = instance.GetComponent<RFX4_EffectSettings>();
            if (effectSettings != null)
            {
                //effectSettings.EffectQuality = IsMobile ? RFX4_EffectSettings.Quality.Mobile : RFX4_EffectSettings.Quality.PC;
                //effectSettings.ForceInitialize();
            }
        }
    }
}