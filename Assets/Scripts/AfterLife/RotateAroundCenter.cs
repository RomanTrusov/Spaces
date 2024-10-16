using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    // ����� �������� (���)
    public Transform rotationCenter;

    // ��������� ������ ��������
    private float initialRadius;
    public float initialRadiusMin = 5.0f;
    public float initialRadiusMax = 30.0f;

    // ������� ������ ��������
    private float currentRadius;

    // �������� �������� ������ ���
    public float rotationSpeedAroundAxis = 30.0f;

    // �������� �������� ������ ����������� ���
    private float selfRotationSpeed;
    public float selfRotationSpeedAmount;

    // ��������� ������� �� �������
    public float radiusChangeSpeed = 1.0f;

    // ���� ��� ��������� ��������
    private float angle;

    void Start()
    {
        initialRadius = Random.Range(initialRadiusMin,initialRadiusMax);
        // ������������� ��������� ������
        currentRadius = initialRadius;
        // ������������� ��������� ����
        angle = Random.Range(-180f,180f);
        selfRotationSpeed = Random.Range(-selfRotationSpeedAmount, selfRotationSpeedAmount);

    }

    void Update()
    {
        // ��������� ���� �������� ������ ���
        angle += rotationSpeedAroundAxis * Time.deltaTime;

        // ������� ��������� �������
        currentRadius += radiusChangeSpeed * Time.deltaTime * Random.Range(-1,1);

        // ��������� ����� ������� �������
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * currentRadius;
        transform.position = rotationCenter.position + offset;

        // ������� ������ ������ ����� ���
        transform.Rotate(Vector3.up, selfRotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, selfRotationSpeed * Time.deltaTime);
    }
}
