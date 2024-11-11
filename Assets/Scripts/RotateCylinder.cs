using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCylinder : MonoBehaviour
{
    public float rotationSpeed = 30f; // 회전 속도 (단위: 도/초)

    private void Update()
    {
        // Y축을 기준으로 회전
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
