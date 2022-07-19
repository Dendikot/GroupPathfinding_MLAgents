using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AvoidProjectileSimple : Agent
{
    private Rigidbody m_RigidBody;

    [SerializeField] private int m_Speed = 15;

    [SerializeField] private Transform m_Shooter;

    [SerializeField] private Transform m_Bullet;

    private float m_Timer = 0;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
        m_Shooter.localPosition = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(1.5f, 6f),
            Random.Range(-5f, 5f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Jump((Directions)actions.DiscreteActions.Array[0]);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(m_Shooter.localPosition);
        sensor.AddObservation(m_Bullet.localPosition);
    }

    private void Jump(Directions dir)
    {
        switch (dir)
        {
            case Directions.Left:
                m_RigidBody.velocity = Vector3.left * m_Speed;
                break;
            case Directions.Forward:
                m_RigidBody.velocity = Vector3.forward * m_Speed;
                break;
            case Directions.Right:
                m_RigidBody.velocity = Vector3.right * m_Speed;
                break;
            case Directions.Back:
                m_RigidBody.velocity = Vector3.back * m_Speed;
                break;
            case Directions.Stay:
                m_RigidBody.velocity = Vector3.zero;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= 5)
        {
            m_Timer = 0;
            AddReward(0.01f);
        }

       /* if (Input.GetKeyDown(KeyCode.A))
        {
            Jump(Directions.Left);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Jump(Directions.Forward);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Jump(Directions.Right);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Jump(Directions.Back);
        }*/
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discAction = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.A))
        {
            discAction[0] = 0;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discAction[0] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discAction[0] = 2;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discAction[0] = 3;
        }
        else
        {
            discAction[0] = 4;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Projectile")
        {
            AddReward(-1f);
            EndEpisode();
        }
        /*
        else if (collision.transform.tag == "Goal")
        {
            AddReward(1f);
            EndEpisode();
        }*/
        /*else if (collision.transform.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Goal")
        {
            AddReward(1f);
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Goal")
        {
            AddReward(1f);
            EndEpisode();
        }
    }
}
