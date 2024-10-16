using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    // Центр вращения (ось)
    public Transform rotationCenter;

    // Начальный радиус вращения
    private float initialRadius;
    public float initialRadiusMin = 5.0f;
    public float initialRadiusMax = 30.0f;

    // Текущий радиус вращения
    private float currentRadius;

    // Скорость вращения вокруг оси
    public float rotationSpeedAroundAxis = 30.0f;

    // Скорость вращения вокруг собственной оси
    private float selfRotationSpeed;
    public float selfRotationSpeedAmount;

    // Изменение радиуса во времени
    public float radiusChangeSpeed = 1.0f;

    // Угол для кругового движения
    private float angle;

    void Start()
    {
        initialRadius = Random.Range(initialRadiusMin,initialRadiusMax);
        // Устанавливаем начальный радиус
        currentRadius = initialRadius;
        // Устанавливаем начальный угол
        angle = Random.Range(-180f,180f);
        selfRotationSpeed = Random.Range(-selfRotationSpeedAmount, selfRotationSpeedAmount);

    }

    void Update()
    {
        // Обновляем угол вращения вокруг оси
        angle += rotationSpeedAroundAxis * Time.deltaTime;

        // Плавное изменение радиуса
        currentRadius += radiusChangeSpeed * Time.deltaTime * Random.Range(-1,1);

        // Вычисляем новую позицию объекта
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * currentRadius;
        transform.position = rotationCenter.position + offset;

        // Вращаем объект вокруг своей оси
        transform.Rotate(Vector3.up, selfRotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, selfRotationSpeed * Time.deltaTime);
    }
}
