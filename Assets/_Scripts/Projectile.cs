using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float projectileVelocity = 5.0f;

    [SerializeField]
    private Rigidbody projectileRb;

    private float lifespanInSeconds = 3.0f;

    private int projectileDamage = 7;

    public void Launch(Vector3 direction)
    {
        StartCoroutine(MoveProjectile(direction));
    }

    private IEnumerator MoveProjectile(Vector3 direction)
    {
        float currentLifeSpan = 0.0f;

        while (currentLifeSpan < this.lifespanInSeconds)
        {
            Vector3 nextFrameTargetPosition = this.projectileRb.position + (direction * this.projectileVelocity * Time.fixedDeltaTime);
            this.projectileRb.MovePosition(nextFrameTargetPosition);

            currentLifeSpan += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().TakeDamage(this.projectileDamage);
        }

        this.StopAllCoroutines();

        Destroy(this.gameObject);
    }
}
