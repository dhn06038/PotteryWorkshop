using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCylinder : MonoBehaviour
{
    public float rotationSpeed = 30f; // ȸ�� �ӵ� (����: ��/��)

    private void Update()
    {
        // Y���� �������� ȸ��
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
