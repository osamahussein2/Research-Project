using System;
using UnityEngine;

public unsafe struct FallObject : IDisposable
{
    public float startX, startY;

    public FallObject(float startX_, float startY_)
    {
        startX = startX_;
        startY = startY_;
    }

    public void Dispose()
    {
        // Reset to 0 after it's disposed
        startX = 0.0f;
        startY = 0.0f;
    }
}

public class FallingObject : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = UnityEngine.Random.Range(1.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 fallDown = new Vector3(0.0f, -speed, 0.0f);
        transform.position += fallDown * Time.deltaTime;

        // Destroy object after it goes below the screen
        if (WentBelowScreen())
        {
            Destroy(gameObject);
        }
    }

    public unsafe void SetStartPosition(FallObject* fallObject_)
    {
        // Initialize the falling object's starting position along the x coordinate
        transform.position = new Vector3(fallObject_->startX, fallObject_->startY, 0.0f);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public bool WentBelowScreen()
    {
        // Hardcore the value to check below the screen
        return transform.position.y <= -5.0f;
    }
}
