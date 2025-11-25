using System;
using UnityEngine;

public class DisabledEnemy : MonoBehaviour
{

    private bool isOff = false;
    public Material defaultEyeMaterial;
    public Material offEyeMaterial;

    private MeshRenderer reyeRenderer;
    private MeshRenderer leyeRenderer;
    public float blinkFrequency = 0.5f;
    private float timeToSwitch = 0f;
    private float timeToBlink = 0f;
    private bool pulse = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeToBlink = blinkFrequency;
        reyeRenderer = transform.Find("REye")?.GetComponent<MeshRenderer>();
        leyeRenderer = transform.Find("LEye")?.GetComponent<MeshRenderer>();

        if (reyeRenderer == null || leyeRenderer == null)
        {
            Debug.LogWarning("One or both eye renderers (REye/LEye) missing on enemy.");
        }

        // Ensure initial state matches isStunned
        UpdateEyeMaterials();
    }

    // Update is called once per frame

    /*            
        if (timeToBlink == blinkFrequency && isOff)
            {
                timeToBlink = blinkFrequency/10;
                Debug.Log("i am quicker" + timeToBlink);
            }
            else
            {
                timeToBlink = blinkFrequency;
                Debug.Log("i am original" + timeToBlink);
            }
    */
    void Update()
    {
        timeToSwitch += Time.deltaTime;
        if (timeToSwitch > timeToBlink && !isOff)
        {
                    timeToBlink = blinkFrequency/4;
            //timeToBlink == blinkFrequency? timeToBlink = blinkFrequency/2 : timeToBlink = blinkFrequency;
            timeToSwitch = 0f;
            isOff = true;
            UpdateEyeMaterials();
            Debug.Log("Material should have changed" + timeToBlink);
        }
        else if (timeToSwitch > timeToBlink && isOff)
        {
            if (pulse)
                {
                    pulse = false;
                    timeToBlink = blinkFrequency/4;
                    Debug.Log("i am quicker" + timeToBlink);
                }
                else
                {
                    pulse = true;
                    timeToBlink = blinkFrequency;
                    Debug.Log("i am original" + timeToBlink);
                }
            //timeToBlink == blinkFrequency? timeToBlink = blinkFrequency/2 : timeToBlink = blinkFrequency;
            timeToSwitch = 0f;
            isOff = false;
            UpdateEyeMaterials();
            Debug.Log("Material should have changed" + timeToBlink);
        }
    }

private void UpdateEyeMaterials()
    {
        Material targetMat = !isOff ? offEyeMaterial : defaultEyeMaterial;
        
        // Use sharedMaterial if you don’t need per-instance overrides (performance +)
        if (reyeRenderer != null) reyeRenderer.sharedMaterial = targetMat;
        if (leyeRenderer != null) leyeRenderer.sharedMaterial = targetMat;
    }
}
