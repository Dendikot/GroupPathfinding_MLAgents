using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;

    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;


    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;

        targetTransform.localPosition = new Vector3(Random.Range(-20, 10),
                                           0.5f,
                                           Random.Range(-10, 20));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goal")
        {
            floorMeshRenderer.material = winMaterial;
            SetReward(1f);
            EndEpisode();
        }
        else if (other.tag == "Wall")
        {
            floorMeshRenderer.material = loseMaterial;
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heruistic");
        Debug.Log(actionsOut);
        ActionSegment<float> continiousAction = actionsOut.ContinuousActions;

        continiousAction[0] = Input.GetAxisRaw("Horizontal");
        continiousAction[1] = Input.GetAxisRaw("Vertical");
    }
}
