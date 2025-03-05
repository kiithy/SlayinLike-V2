using UnityEngine;

public class KnightHitBox : MonoBehaviour
{
    private int attackCombo = 0;
    void Update()
    {
        attackCombo = GetComponentInParent<gameplay>().attackCombo;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (attackCombo == 1)
            {
                // deals two damage
                other.GetComponent<Orc>().TakeDamage(1);
            }
            else if (attackCombo == 2)
            {
                // deals four damage
                other.GetComponent<Orc>().TakeDamage(2);
            }
        }
    }
}
