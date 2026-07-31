using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class WorldGeneration : MonoBehaviour
{
    public static List<WorldGeneration> worldGenerators = new List<WorldGeneration>();
    [SerializeField]
    private List<GameObject> prefabs;
    [SerializeField]
    private float spawnRadius = 100f;
    [SerializeField]
    private int spawnCount = 10;
    private bool readyToRebake = false;
    private void Awake()
    {
        worldGenerators.Add(this);
    }
    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnObject();
        }
        readyToRebake = true;
        if (RebakeCheck())
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
    public static bool RandomPointOnNavMesh(Vector3 center, float range, out Vector3 point)
    {
        Vector3 randomPoint;
        NavMeshHit hit;
        int counter = 0;
        do
        {
            if (counter == 10)
            {
                point = Vector3.zero;
                return false;
            }
            randomPoint = center + Random.insideUnitSphere * range;
            counter++;
        }
        while (!NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas));
        point = hit.position;
        return true;
    }
    public bool ReadyToRebake() => readyToRebake;
    public static bool RebakeCheck()
    {
        foreach (WorldGeneration obj in worldGenerators)
        {
            if (!obj.readyToRebake)
            {
                return false;
            }
        }
        return true;
    }
    void SpawnObject()
    {
        Vector3 randomPoint;
        if (RandomPointOnNavMesh(transform.position, spawnRadius, out randomPoint))
        {
            GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Count)], randomPoint, Quaternion.identity);
            obj.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
            obj.transform.SetParent(transform);
        }
    }
}
