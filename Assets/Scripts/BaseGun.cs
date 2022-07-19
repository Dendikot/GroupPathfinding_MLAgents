using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Bullet;

    [SerializeField]
    private float m_BulletSpeed = 10;

    public float BulletSpeed
    {
        get
        {
            return m_BulletSpeed;
        }
        set
        {
            m_BulletSpeed = value;
        }
    }

    [SerializeField]
    private Transform[] m_Targets;

    private float m_ShootRate = 0;

    private Rigidbody m_BulletRigidBody;

    [SerializeField]
    private Vector3 m_DeathPosition;

    private Bullet m_BulletScript;

    [SerializeField]
    private float m_Rate = 5;

    private int m_CurrentTarg = 0;

    private void Awake()
    {   
        m_BulletRigidBody = m_Bullet.GetComponent<Rigidbody>();
        m_Bullet.transform.localPosition = m_DeathPosition;
        m_BulletScript = m_Bullet.GetComponent<Bullet>();
        m_BulletScript.RegenPos = m_DeathPosition;
    }

    private void Fire()
    {
        if (m_BulletSpeed <= 0)
        {
            return;
        }

        m_Bullet.transform.localPosition = transform.localPosition;
        m_BulletRigidBody.AddForce(transform.forward * m_BulletSpeed);

        m_CurrentTarg = Random.Range((int)0, (int)3);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(m_Targets[m_CurrentTarg]);

        m_ShootRate += Time.deltaTime;

        if (Vector3.Distance( m_Bullet.transform.localPosition, transform.localPosition) > 20 )
        {
            m_BulletScript.Reset();
            
        }

        if (m_ShootRate >= m_Rate)
        {
            m_ShootRate = 0;
            Fire();
        }
    }
}
