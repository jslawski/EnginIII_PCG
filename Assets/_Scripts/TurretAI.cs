using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    public LayerMask targetLayers;

    [SerializeField]
    private GameObject projectilePrefab;

    private Transform turretTransform;
    private Transform targetTransform;

    private SphereCollider triggerZone;

    private Coroutine shootCoroutine;

    private float shootDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        this.turretTransform = GetComponent<Transform>();
        this.triggerZone = GetComponent<SphereCollider>();
    }

    private bool TargetIsInLineOfSight()
    {
        Vector3 targetDirection = (this.targetTransform.position - this.turretTransform.position).normalized;

        RaycastHit hitInfo;
        if (Physics.Raycast(this.turretTransform.position, targetDirection, out hitInfo, this.triggerZone.radius, targetLayers.value))
        {
            if (hitInfo.collider.gameObject.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }

    private void LookAtTarget()
    {
        this.turretTransform.LookAt(targetTransform);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.targetTransform != null)
        {
            if (this.TargetIsInLineOfSight())
            {
                this.LookAtTarget();

                if (this.shootCoroutine == null)
                {
                    this.shootCoroutine = StartCoroutine(this.Shoot());
                }
            }
        }
    }

    private IEnumerator Shoot()
    {
        while (this.targetTransform != null)
        {
            //Launch projectile
            GameObject projectileInstance = Instantiate(this.projectilePrefab, this.turretTransform.position, new Quaternion());
            projectileInstance.GetComponent<Projectile>().Launch((this.targetTransform.position - this.turretTransform.position).normalized);

            //Wait for next shot
            yield return new WaitForSeconds(this.shootDelay);
        }

        this.shootCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            this.targetTransform = other.gameObject.GetComponent<Transform>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            this.targetTransform = null;
        }
    }
}
