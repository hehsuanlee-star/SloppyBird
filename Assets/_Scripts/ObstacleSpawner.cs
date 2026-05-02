using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{

    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform obStartTransform;
    private CancellationTokenSource _spawnCts;
    private float _playerX;
    public int ObstaclePoolRange = 10;

    public float initialInterval = 2;
    public float minIntervval = 0.1f;
    private float gameTimer;
    public float spawnInterval => Mathf.Lerp(initialInterval, minIntervval, Mathf.Clamp01(gameTimer * 2 / timeToMax));
    public float initialSpeed = 5;
    public float maxSpeed = 20;
    public float timeToMax = 120f;
    public float travelSpeed => Mathf.Lerp(initialSpeed, maxSpeed, Mathf.Clamp01(gameTimer / timeToMax));

    private Queue<GameObject> InactiveObstacles;
    private List<Obstacle> AllObstacles;

    private void Awake()
    {
        InactiveObstacles = new Queue<GameObject>();
        AllObstacles = new List<Obstacle>();
    }

    private void Update()
    {
        TimeRun();
    }
    private void TimeRun()
    {
        if (!bSpawning) return;
        gameTimer += Time.deltaTime;
    }
    private void TimeReset()
    {
        gameTimer = 0;
    }
    public void SpawnerInit()
    {
        foreach (var ob in AllObstacles)
            ob.gameObject.SetActive(false);
        ClearInactiveObstacles();
        TimeReset();
        for (int i = 0; i < ObstaclePoolRange; i++)
        {
            SpawnObstacleToPool();
        }
    }
    public void GetPlayerLocX(float playerX)
    {
        _playerX = playerX;
    }

    private bool bSpawning;

    public void StopSpawning()
    {
        bSpawning = false;
        _spawnCts?.Cancel();
    }
    public async void StartSpawning()
    {
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
        _spawnCts = new CancellationTokenSource();
        var token = _spawnCts.Token;
        bSpawning = true;
        while (bSpawning)
        {
            ActivateObstacleOrSpawnNewOne();
            try
            {
                await Awaitable.WaitForSecondsAsync(spawnInterval, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private void ActivateObstacleOrSpawnNewOne()
    {
        if (InactiveObstacles.Count == 0)
        {
            SpawnObstacleToPool();
        }
        var activeObstacle = InactiveObstacles.Dequeue();
        var obstacle = activeObstacle.GetComponent<Obstacle>();
        obstacle.ResetSpeedandLocation(travelSpeed);
        activeObstacle.SetActive(true);
    }

    private void SpawnObstacleToPool()
    {
        var spawnedObstacle = Instantiate(obstaclePrefab, obStartTransform.position, Quaternion.identity);
        var obstacle = spawnedObstacle.GetComponent<Obstacle>();
        obstacle.OnSurpassed += ObstacleSurpassed;
        obstacle.OnRecycled += ObstacleRecycle;
        obstacle.SetStartPosandPlayerXPos(obStartTransform.transform.position, _playerX);
        spawnedObstacle.SetActive(false);
        InactiveObstacles.Enqueue(spawnedObstacle);
        AllObstacles.Add(obstacle);
    }

    //obstacle informs spawner, spawner informs game manager
    public event Action OnObstacleSurpassed;
    private void ObstacleSurpassed()
    {
        OnObstacleSurpassed?.Invoke();
    }

    private void ObstacleRecycle(GameObject obstacle)
    {
        obstacle.SetActive(false);
        InactiveObstacles.Enqueue(obstacle);
    }

    public void ClearInactiveObstacles()
    {

        InactiveObstacles.Clear();
    }


    public void StopAllObstacles()
    {
        foreach (var ob in AllObstacles)
        {
            ob.Stop();
        }
    }

    public void EndSpawnerProcess()
    {
        StopSpawning();
        StopAllObstacles();
    }
    private void OnDestroy()
    {
        foreach (var ob in AllObstacles)
        {
            ob.OnSurpassed -= ObstacleSurpassed;
            ob.OnRecycled -= ObstacleRecycle;
        }
    }
}
