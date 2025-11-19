using UnityEngine;
using UnityEngine.SceneManagement;

public class RigSpawner : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Insane Asylum")
        {
            var spawn = GameObject.Find("AsylumSpawnPoint");
            if (spawn != null)
            {
                transform.position = spawn.transform.position;
                transform.rotation = spawn.transform.rotation;
            }
        }
    }
}
