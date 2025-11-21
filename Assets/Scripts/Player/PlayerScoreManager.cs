using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private PointSystem pointSystem;

    [Header("Point Sources")]
    [SerializeField] private DroneSpawner droneSpawner;
    [SerializeField] private EnemySpawner robotSpawner;
    // [SerializeField] private TimeManager timeManager;

    [SerializeField] private BladeDeflect bigDeflectArea;
    [SerializeField] private BladeDeflect smallDeflectArea;

    [Header("Point Values")]
    [SerializeField] private int dronePoints = 100;
    [SerializeField] private int robotPoints = 100;
    [SerializeField] private int bigDeflectPoints = 5;
    [SerializeField] private int smallDeflectPoints = 5;

    private float timeAccumulator = 0f;

    private void Start()
    {
        droneSpawner.OnDroneDeath += HandleDroneDeath;
        // TODO: Implement OnDeath event for robots
        // robotSpawner.OnRobotDeath += HandleRobotDeath;
        
        // TODO: Implement TimeManager
        // timeManager.OnTimerUpdated += HandleTimerUpdate;

        bigDeflectArea.OnDeflect += HandleBigDeflect;
        smallDeflectArea.OnDeflect += HandleSmallDeflect;
        

    }

    private void HandleDroneDeath(Drone drone)
    {
        pointSystem.AddPoints(dronePoints);
    }

    private void HandleRobotDeath(Enemies robot)
    {
        pointSystem.AddPoints(robotPoints);
    }

    private void HandleTimerUpdate(float time)
    {
        timeAccumulator += time;

        if (timeAccumulator >= 0.5f)
        {
            pointSystem.AddPoints(1);
            timeAccumulator -= 0.5f;
        }
    }

    private void HandleBigDeflect()
    {
        pointSystem.AddPoints(bigDeflectPoints);
    }

    private void HandleSmallDeflect()
    {
        pointSystem.AddPoints(smallDeflectPoints);
    }
}
