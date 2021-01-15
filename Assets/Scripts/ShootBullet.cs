using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    [SerializeField] private float speed = 40;
    [SerializeField] private GameObject bulletGO;
    [SerializeField] private Transform shootFrom;
    [SerializeField] private AudioClip shootClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        //audio
        GameObject bullet = Instantiate(bulletGO, shootFrom.position, shootFrom.rotation);
        bullet.GetComponent<Rigidbody>().velocity = speed * shootFrom.forward;
        Destroy(bullet, 3.0f);
    }
}
