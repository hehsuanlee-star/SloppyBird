using UnityEngine;
public class GameManager : MonoBehaviour
{
    //Player
    [SerializeField] private GameObject player;
    private BirdController controller;
    //UI
    [SerializeField] private UIManager _UI;
    //obstacle
    [SerializeField] private ObstacleSpawner _spawner;


    public int Score;
    private void Awake()
    {
        //Spawner
        _spawner.OnObstacleSurpassed += ScoreUpdate;
        _spawner.GetPlayerLocX(player.transform.position.x);
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
        _spawner.SpawnerInit();
        StartGame();
    }


    private void StartGame()
    {
        controller.StartMotion();
        _spawner.StartSpawning();
    }

    private void ScoreUpdate()
    {
        Score++;
        _UI.ScoreUpdate(Score);
    }

    private void GameOver()
    {
        controller.PauseMotion();
        _spawner.EndSpawnerProcess();
        Debug.Log("GameOver");
        _UI.ShowGameOver();
    }

    private void OnDestroy()
    {
        controller.OnTriggeredDead -= GameOver;
    }
}
