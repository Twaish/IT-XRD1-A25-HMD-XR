using UnityEngine;

public class PlayerScoreManager : MonoBehaviour
{
    [SerializeField] private PointSystem pointSystem;

    [Header("Point Sources")]
    [SerializeField] private DroneSpawner droneSpawner;
    [SerializeField] private EnemySpawner robotSpawner;
    [SerializeField] private TimerManager timerManager;

    [SerializeField] private BladeDeflect bigDeflectArea;
    [SerializeField] private BladeDeflect smallDeflectArea;

    [Header("Point Values")]
    [SerializeField] private int dronePoints = 100;
    [SerializeField] private int robotPoints = 100;
    [SerializeField] private int bigDeflectPoints = 5;
    [SerializeField] private int smallDeflectPoints = 5;

    private float lastTimeAwarded = 0f;

    private void Start()
    {
        droneSpawner.OnDroneDeath += HandleDroneDeath;
        // TODO: Implement OnDeath event for robots
        // robotSpawner.OnRobotDeath += HandleRobotDeath;
        
        timerManager.OnTimerUpdated += HandleTimerUpdate;

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

    private void HandleTimerUpdate(float currentTime)
    {
        if (currentTime - lastTimeAwarded >= 0.5f)
        {
            pointSystem.AddPoints(1);
            lastTimeAwarded = currentTime;
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
