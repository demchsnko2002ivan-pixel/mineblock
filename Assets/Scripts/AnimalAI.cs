using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAI : MonoBehaviour
{
    enum state {walk, idle, rest, run, dead}
    [SerializeField]
    private state currentState = state.idle;
    private NavMeshAgent agent;
    private Animator animator;
    private float timer = 0f;
    private Vector3 destination = Vector3.zero;
    [Header("��������� ������������")]
    [Tooltip("��������, � ������� ���� �������������� � �������")]
    public float rotationSpeed = 5f;
    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float runSpeed = 3f;

    [Tooltip("����������� ������� ���� (� ��������), ��� ������� ���������� ������������. �������� �������� ������ �� ������ ��������.")]
    public float angleThreshold = 2f; // ������� � 2-5 ��������    

    [Tooltip("����, �� ������� ��������� ������� ��� �����")]
    public LayerMask terrainLayer;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
            timer -= Time.deltaTime;
        if (timer < 0f)
        {
            switch (Random.Range(1, 4))
            {
                case 1:
                    SetState(state.idle);
                    break;
                case 2:
                    SetState(state.rest);
                    break;
                case 3:
                    if (currentState != state.rest)
                    {
                        SetState(state.walk);
                    }
                    else
                    {
                        SetState(state.idle);
                    }
                    break;
            }
            timer = Random.Range(5, 20);
        }
        switch (currentState)
        {
            case state.idle:

                break;
            case state.walk:
                float distance = Vector3.Distance(transform.position, destination);
                if (distance < 1f)
                {
                    SetState(state.idle);
                }
                break;
            case state.run:

                break;
            case state.rest:

                break;
            case state.dead:

                break;
        }
    }
    void AlignUnit(Vector3 surfaceNormal)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;

        float rotationSpeed = 5f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    private void SetState(state state)
    {
        currentState = state;
        switch (state)
        {
            case state.idle:
                agent.isStopped = true;
                animator.SetBool("walk", false);
                animator.SetBool("rest", false);
                animator.SetBool("run", false);
                break;
            case state.walk:
                agent.isStopped = false;
                agent.speed = walkSpeed;
                animator.SetBool("walk", true);
                if (WorldGeneration.RandomPointOnNavMesh(transform.position, 20, out destination))
                {
                    agent.destination = destination;
                }
                break;
            case state.run:
                agent.isStopped = false;
                agent.speed = runSpeed;
                animator.SetBool("run", true);
                animator.SetBool("rest", false);
                if (WorldGeneration.RandomPointOnNavMesh(transform.position + GetRunDirection() * 100, 20, out destination))
                {
                    agent.destination = destination;
                }
                break;
            case state.rest:
                agent.isStopped = true;
                animator.SetBool("rest", true);
                break;
            case state.dead:
                agent.isStopped = true;
                animator.enabled = false;
                break;
        }
    }
    
    private Vector3 GetRunDirection()
    {
        Vector3 usPosition = transform.position;

        Vector3 otherPosition = PlayerController.Instance.transform.position;

        Vector3 directionAToB = otherPosition - usPosition;

        Vector3 directionOpposite = -directionAToB;

        Vector3 normalizedOppositeDirection = directionOpposite.normalized;
        return normalizedOppositeDirection;
    }
    void FixedUpdate()
    {
        Vector3 rayStart = transform.position + transform.up * 0.5f;
        Vector3 rayDirection = -transform.up; // ����������� ����    

        RaycastHit hit;
        float rayDistance = 2f; // ����������� ���������� ��� �������� ��� ������    

        if (Physics.Raycast(rayStart, rayDirection, out hit, rayDistance, terrainLayer))
        {
            Vector3 surfaceNormal = hit.normal;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;

            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            if (angleDifference > angleThreshold)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            //(�����������)    
            // AlignUnitPosition(hit.point);
        }
    }
    public void RunAway()
    {
        SetState(state.run);
    }

    void AlignUnitPosition(Vector3 hitPoint)
    {
        float verticalOffset = 0.5f; // ��������� ��� �������� � ����������� �� ������ ������ �����    

        Vector3 targetPosition = hitPoint + transform.up * verticalOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * rotationSpeed);
    }
}
