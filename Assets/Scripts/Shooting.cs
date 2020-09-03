using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public GameObject hitImpact;
    public LineRenderer lineRenderer;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    public IEnumerator Shoot()
    {
        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        //rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, 30, layerMask);
        if (hitInfo)
        {
            //Debug.Log(hitInfo.collider.transform.gameObject);

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 10);
        }
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.002f);
        lineRenderer.enabled = false;
    }
}
