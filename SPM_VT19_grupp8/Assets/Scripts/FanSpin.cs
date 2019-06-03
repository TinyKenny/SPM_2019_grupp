using UnityEngine;

public class FanSpin : MonoBehaviour
{
    [SerializeField, Min(0.0f)] private float speed = 10;

    void Awake()
    {
        transform.Rotate(Vector3.up * Random.Range(0.0f, 360.0f));
    }

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}