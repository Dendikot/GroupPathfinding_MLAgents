using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

enum Directions
{
    Left,
    Forward,
    Right,
    Back,
    Stay
}

public class AvoidProjectileAgent : Agent
{
    private Rigidbody m_RigidBody;

    [SerializeField] private int m_Speed = 15;

    [SerializeField] private Transform m_Shooter;
    private BaseGun m_GunController;

    [SerializeField] private Transform m_Bullet;

    [SerializeField] private Transform m_Target;

    [SerializeField] private Transform m_Walls;

    private Transform m_Arrow;

    private float m_Distance = 0;

    private float m_Timer = 0;

    private Vector3[] m_VecDirect = { new Vector3(-90,0,90), new Vector3(-90, 0, -180), new Vector3(-90, 0, -90), new Vector3(-90, 0, 0) };

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_GunController = m_Shooter.GetComponent<BaseGun>();
        m_Arrow = transform.GetChild(0);
    }

    private void RotateArrow(Directions dir)
    {
        switch (dir)
        {
            case Directions.Left:
                m_Arrow.localRotation = Quaternion.Euler(m_VecDirect[0]);
                break;
            case Directions.Forward:
                m_Arrow.localRotation = Quaternion.Euler(m_VecDirect[1]);
                break;
            case Directions.Right:
                m_Arrow.localRotation = Quaternion.Euler(m_VecDirect[2]);
                break;
            case Directions.Back:
                m_Arrow.localRotation = Quaternion.Euler(m_VecDirect[3]);
                break;
            default:
                break;
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(
            Random.Range(-12f, 12f),
            0.2f,
            Random.Range(-12f, 12f));

        m_Shooter.localPosition = new Vector3(
            Random.Range(-15f, 15f),
            Random.Range(1.5f, 6f),
            Random.Range(-15f, 15f));

        m_Distance = Vector3.Distance(transform.localPosition, m_Target.localPosition);

        m_Target.localPosition = new Vector3(
            Random.Range(-15f, 13f),
            0.3f,
            Random.Range(-13f, 13f));

        m_GunController.BulletSpeed = Academy.Instance.EnvironmentParameters.GetWithDefault("bullet_speed", m_GunController.BulletSpeed);

        if (m_GunController.BulletSpeed > 1000 )
        {
            if (!m_Walls.gameObject.activeSelf) m_Walls.gameObject.SetActive(true);
        } else
        {
            return;
        }

        for (int i = 0; i < m_Walls.childCount; i++)
        {
            Transform child = m_Walls.GetChild(i).transform;
            child.localPosition = new Vector3(Random.Range(-12f, 12f), child.localPosition.y, child.localPosition.z);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Jump((Directions)actions.DiscreteActions.Array[0]);

        float m_CurrentDist = Vector3.Distance(transform.localPosition, m_Target.localPosition) + 0.0001f;

        float i = 1f / (float)MaxStep;

        if (m_CurrentDist < m_Distance - 0.3f)
        { 
            AddReward(i);
        }
        else
        {
            AddReward(-i);
        }

        m_Distance = m_CurrentDist;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(m_Shooter.localPosition);
        sensor.AddObservation(m_Bullet.forward);
        sensor.AddObservation(m_Target.localPosition);

        //pass diff vector instead
        Vector3 diffGoal = m_Target.localPosition - transform.localPosition;
        sensor.AddObservation(diffGoal);
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
        RotateArrow(dir);
    }

    private void Update()
    {
        /*m_Timer += Time.deltaTime;
        if (m_Timer >= 5)
        {
            m_Timer = 0;
            AddReward(0.1f);
        }*/

        /*if (Input.GetKeyDown(KeyCode.A))
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
        else if (collision.transform.tag == "Goal")
        {
            AddReward(1f);
            EndEpisode();
        }
        else if (collision.transform.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Projectile")
        {
            AddReward(-1f);
            EndEpisode();
        }
        else if (other.transform.tag == "Goal")
        {
            AddReward(1f);
            EndEpisode();
        }
        else if (other.transform.tag == "Wall")
        {
            AddReward(-1f);
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
