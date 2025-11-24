using UnityEngine;

public class PersistentRig : MonoBehaviour
{
    public float TimeOfCreation;
    void Awake()
    {
        TimeOfCreation = Time.time;
        GameObject[] rigs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var rig in rigs)
        {
            if (rig == this.gameObject) continue;

            if (!rig.TryGetComponent(out PersistentRig other))
            {
                continue;
            }

            if (other.TimeOfCreation < TimeOfCreation)
            {
                Destroy(gameObject);
            } else if (other.TimeOfCreation > TimeOfCreation)
            {
                Destroy(rig);
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
