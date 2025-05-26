using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float range = 100f;
    public Camera fpsCam; // Reference to the camera
    public int damage = 25;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        // Cast a ray from the center of the camera
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Check if the hit object is an enemy
            if (hit.transform.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }
}
