using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light fireLight;
    public float minIntensity = 2f;
    public float maxIntensity = 5f;
    public float flickerSpeed = 0.1f;

    private void Update()
    {
        fireLight.intensity = Mathf.Lerp(fireLight.intensity, Random.Range(minIntensity, maxIntensity), flickerSpeed);
    }
}
