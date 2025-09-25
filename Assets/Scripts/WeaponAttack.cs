using UnityEngine;
using System.Collections;

public class WeaponAttack : MonoBehaviour
{
    public GameObject weaponHitBox;
    public GameObject weaponSprite;
    public float attackDuration = 0.2f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        if (weaponHitBox != null) weaponHitBox.SetActive(true);
        if (weaponSprite != null) weaponSprite.SetActive(true);

        yield return new WaitForSeconds(attackDuration);

        if (weaponHitBox != null) weaponHitBox.SetActive(false);
        if (weaponSprite != null) weaponSprite.SetActive(false);
    }
}
