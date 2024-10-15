using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    // ����� �������� (���)
    public Transform rotationCenter;

    // ��������� ������ ��������
    public float initialRadius = 5.0f;

    // ������� ������ ��������
    private float currentRadius;

    // �������� �������� ������ ���
    public float rotationSpeedAroundAxis = 30.0f;

    // �������� �������� ������ ����������� ���
    public float selfRotationSpeed = 90.0f;

    // ��������� ������� �� �������
    public float radiusChangeSpeed = 1.0f;

    // ���� ��� ��������� ��������
    private float angle;

    void Start()
    {
        // ������������� ��������� ������
        currentRadius = initialRadius;
        // ������������� ��������� ����
        angle = 0.0f;
    }

    void Update()
    {
        // ��������� ���� �������� ������ ���
        angle += rotationSpeedAroundAxis * Time.deltaTime;

        // ������� ��������� �������
        currentRadius += radiusChangeSpeed * Time.deltaTime;

        // ��������� ����� ������� �������
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * currentRadius;
        transform.position = rotationCenter.position + offset;

        // ������� ������ ������ ����� ���
        transform.Rotate(Vector3.up, selfRotationSpeed * Time.deltaTime);
    }
}
