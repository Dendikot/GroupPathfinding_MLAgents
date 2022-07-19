using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class FindTargetAgent : Agent
{
    private Rigidbody m_RigidBody;

    [SerializeField] private int m_Speed = 10;

    [SerializeField] private Transform m_TargetTransform;

    [SerializeField] private GameObject[] obstacles;

    private float m_TargetDistance = 0;

    private void generateMaze()
    {
/*        float x = Random.Range(-10, 10);
        float z = 45f;

        float zAdd = Random.Range(7, 10);



*/
    }

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();

        m_TargetDistance = Vector3.Distance(m_TargetTransform.localPosition, transform.localPosition );
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, -51f, -30f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        Jump((Directions)actions.DiscreteActions.Array[0]);

        //float dist = Vector3.Distance(m_TargetTransform.localPosition, transform.localPosition) + 0.0001f;
        float dist = Mathf.Abs( m_TargetTransform.localPosition.z) - Mathf.Abs(transform.localPosition.z);
        if (m_TargetDistance > dist)
        {
            AddReward(0.01f);
        }
        else
        {
            AddReward(-0.01f);
        }

        m_TargetDistance = dist;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

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
            default:
                break;
        }
    }

    private void Update()
    {



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
        Debug.Log("Heruistic");


        ActionSegment<int> discreteAction = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.D))
        {
            discreteAction[0] = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteAction[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteAction[0] = 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteAction[0] = 3;
        }

        /*ActionSegment<int> discAction = actionsOut.DiscreteActions;

        if (Input.GetKeyDown(KeyCode.A))
        {
            discAction[0] = 0;
        } 
        else if (Input.GetKeyDown(KeyCode.W))
        {
            discAction[0] = 1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            discAction[0] = 2;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            discAction[0] = 3;
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Goal")
        {
            AddReward(1f);
            EndEpisode();
        }
    }
}