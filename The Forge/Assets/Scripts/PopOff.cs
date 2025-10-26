using UnityEngine;
using System.Collections;

public class PopOff : MonoBehaviour
{
    public float rotationSpeed; // degrees per second
    public float moveSpeed;
    public float downDuration; 
    public float spinDuration;   // how long to spin in seconds

    public void Run()
    {
        StartCoroutine(SpinForSeconds());
    }

    private IEnumerator SpinForSeconds()
    {
        float elapsed = 0f;

        while (elapsed < downDuration)
        {
            float delta = Time.deltaTime;

            transform.position += Vector3.down * moveSpeed * delta;

            elapsed += delta;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < spinDuration)
        {
            float delta = Time.deltaTime;

            transform.Rotate(0f, 0f, rotationSpeed * delta);
            transform.position += Vector3.up * moveSpeed * delta;
            transform.position += Vector3.right * moveSpeed * delta;
            
            elapsed += delta;
            yield return null;
        }
    }
}
