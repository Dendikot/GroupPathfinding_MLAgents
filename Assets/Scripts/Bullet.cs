using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 m_RegenPos;
    private Rigidbody m_RigidBody;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    public Vector3 RegenPos
    {
        set
        {
            m_RegenPos = value;
        }
    }

    private void Start()
    {
        //Destroy(gameObject, 3);
        transform.localPosition = m_RegenPos;
        m_RigidBody.velocity = Vector3.zero;
    }


    public void Reset()
    {
        transform.localPosition = m_RegenPos;
        m_RigidBody.velocity = Vector3.zero;
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Destroy(gameObject);
        //GetComponent<Collider>().enabled = false;
        transform.localPosition = m_RegenPos;
        m_RigidBody.velocity = Vector3.zero;
    } 
}
