using UnityEngine;
using System.Collections;

public class WeaponAttack : MonoBehaviour
{
    public GameObject weaponHitBox;
    public GameObject weaponSprite;
    public float attackDuration = 0.2f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;
    private Animator weaponAnimator;

    private void Start()
    {
        if (weaponSprite != null)
        {
            weaponAnimator = weaponSprite.GetComponent<Animator>();
            weaponSprite.SetActive(false);
        }

        if (weaponHitBox != null)
            weaponHitBox.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(Attack());
                lastAttackTime = Time.time;
            }
        }
    }

    private IEnumerator Attack()
    {
        if (weaponHitBox != null)
            weaponHitBox.SetActive(true);

        if (weaponSprite != null)
        {
            weaponSprite.SetActive(true);
            if (weaponAnimator != null)
                weaponAnimator.Play("Sword", 0, 0f);
        }

        yield return new WaitForSeconds(attackDuration);

        if (weaponHitBox != null)
            weaponHitBox.SetActive(false);

        if (weaponSprite != null)
            weaponSprite.SetActive(false);
    }
}
