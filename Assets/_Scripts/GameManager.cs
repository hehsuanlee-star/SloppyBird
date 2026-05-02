using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private BirdController controller;
    [SerializeField] private UIManager _UI;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform startPos;
    private bool isGameStart;
    public int ObstaclePoolRange = 10;
    public int Score;
    private CancellationTokenSource _spawnCts;
    public float initialInterval = 2;
    public float minIntervval = 0.1f;
    public float gameTimer;
    public float spawnInterval => Mathf.Lerp(initialInterval, minIntervval, Mathf.Clamp01(gameTimer * 2 / timeToMax));
    public float initialSpeed = 5;
    public float maxSpeed = 20;
    public float timeToMax = 120f; 
    public float travelSpeed => Mathf.Lerp(initialSpeed, maxSpeed, Mathf.Clamp01(gameTimer / timeToMax));
    private Queue<GameObject> InactiveObstacles;

    private void Awake()
    {
        InactiveObstacles = new Queue<GameObject>();
        //Player
        controller = player.GetComponent<BirdController>();
        controller.OnTriggeredDead += GameOver;
        //UI
        _UI.GameStart += GameInit;
    }

    private void Start()
    {
        _UI.ShowGameTitle();
    }

    private void GameInit()
    {
        Score = 0;
        controller.ReturnOrigin();
        _UI.ScoreUpdate(Score);
        foreach (var go in FindObjectsByType<Obstacle>(FindObjectsSortMode.None))
            go.gameObject.SetActive(false);
        //Clear Old Pool
        InactiveObstacles.Clear();
        //insintiate initial obstacle pool
        for (int i = 0; i < ObstaclePoolRange; i++)
        {
            SpawnObstacleToPool();
        }
        //start
        StartGame();
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
    }
    private async void StartGame()
    {
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
        _spawnCts = new CancellationTokenSource();
        var token = _spawnCts.Token;
        isGameStart =true;
        controller.StartMotion();
        while (isGameStart)
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
        if (InactiveObstacles.Count > 0)
        {
            var activeObstacle = InactiveObstacles.Dequeue();
            var obstacle = activeObstacle.GetComponent<Obstacle>();
            obstacle.SetRandomLocation();
            obstacle.SetSpeed(travelSpeed);
            activeObstacle.SetActive(true);
        }
        else
        {
            SpawnObstacleToPool();
        }
    }

    private void SpawnObstacleToPool()
    {
        var spawnedObstacle = Instantiate(obstaclePrefab, startPos.position, Quaternion.identity);
        var obstacle = spawnedObstacle.GetComponent<Obstacle>();
        obstacle.OnSurpassed += ObstacleSurpassed;
        obstacle.OnCollided += GameOver;
        obstacle.OnRecycled += ObstacleRecycle;
        obstacle.SetStartPos(startPos.position);
        spawnedObstacle.SetActive(false);
        InactiveObstacles.Enqueue(spawnedObstacle);
    }

    private void Reset()
    {
        gameTimer = 0;
    }
    private void ObstacleSurpassed()
    {
        Score++;
        _UI.ScoreUpdate(Score);
    }

    private void ObstacleRecycle(GameObject obstacle)
    { 
        obstacle.SetActive(false);
        InactiveObstacles.Enqueue(obstacle);
    }
    private void GameOver()
    { 
        isGameStart = false;
        controller.PauseMotion();
        foreach (var go in FindObjectsByType<Obstacle>(FindObjectsSortMode.None))
            go.Stop();
        Debug.Log("GameOver");
        _UI.ShowGameOver();
        Reset();
    }

    private void OnDestroy()
    {
        foreach (var obstacle in FindObjectsByType<Obstacle>(FindObjectsSortMode.None))
        { 
            obstacle.OnSurpassed -= ObstacleSurpassed;
            obstacle.OnCollided -= GameOver;
            obstacle.OnRecycled -= ObstacleRecycle; 
        }
        controller.OnTriggeredDead -= GameOver;
    }
}
