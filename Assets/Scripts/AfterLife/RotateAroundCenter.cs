using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    // Центр вращения (ось)
    public Transform rotationCenter;

    // Начальный радиус вращения
    public float initialRadius = 5.0f;

    // Текущий радиус вращения
    private float currentRadius;

    // Скорость вращения вокруг оси
    public float rotationSpeedAroundAxis = 30.0f;

    // Скорость вращения вокруг собственной оси
    public float selfRotationSpeed = 90.0f;

    // Изменение радиуса во времени
    public float radiusChangeSpeed = 1.0f;

    // Угол для кругового движения
    private float angle;

    void Start()
    {
        // Устанавливаем начальный радиус
        currentRadius = initialRadius;
        // Устанавливаем начальный угол
        angle = 0.0f;
    }

    void Update()
    {
        // Обновляем угол вращения вокруг оси
        angle += rotationSpeedAroundAxis * Time.deltaTime;

        // Плавное изменение радиуса
        currentRadius += radiusChangeSpeed * Time.deltaTime;

        // Вычисляем новую позицию объекта
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * currentRadius;
        transform.position = rotationCenter.position + offset;

        // Вращаем объект вокруг своей оси
        transform.Rotate(Vector3.up, selfRotationSpeed * Time.deltaTime);
    }
}
