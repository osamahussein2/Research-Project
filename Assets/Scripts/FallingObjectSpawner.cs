using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public unsafe class FallingObjectSpawner : MonoBehaviour
{
    // Falling object to spawn
    private GameObject fallingObject;

    // Time to spawn falling objects
    private float spawnTimer;
    private bool fallingObjectsSpawned = false;

    private MemoryArena memoryArena;

    private FallObject* fallObject;
    [SerializeField] private GameObject[] fallingObjects;

    [Header("Falling Object Parameters")]

    // Can modify the number of falling objects spawning in the inspector
    [SerializeField] private int numberOfFallingObjects = 6;

    [SerializeField] private float fallingObjectXLocation = -5.0f;
    [SerializeField] private float fallingObjectXOffset = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fallingObject = Resources.Load<GameObject>("Falling Object");
    }

    // Update is called once per frame
    void Update()
    {
        // If falling objects aren't spawned yet
        if (!fallingObjectsSpawned)
        {
            spawnTimer += Time.deltaTime;

            // Spawn falling objects after 1 second
            if (spawnTimer >= 1.0f)
            {
                SpawnFallingObjects();
            }
        }

        // Otherwise, the falling objects have spawned
        else if (fallingObjectsSpawned)
        {
            // Eventually release memory arena after falling objects have been destroyed
            FreeMemoryArena();
        }
    }

    private void SpawnFallingObjects()
    {
        for (int i = 0; i < numberOfFallingObjects; i++)
        {
            // Initialize the falling object's starting position using the struct
            FallObject fallObjectReference = new FallObject(fallingObjectXLocation + (i * fallingObjectXOffset),
                5.0f);

            // Convert the regular struct to a pointer to the struct
            fallObject = &fallObjectReference;
            Instantiate(fallingObject); // Spawn the falling object

            // Once it's spawned, we can set the starting position
            if (fallingObject)
            {
                fallingObject.GetComponent<FallingObject>().SetStartPosition(fallObject);
            }
        }

        // Allocate a memory arena for the fall object struct pointer to get the object's starting position
        int sizeOfObject = UnsafeUtility.SizeOf<FallObject>();
        memoryArena = new MemoryArena(sizeOfObject * numberOfFallingObjects, 16);
        fallObject = memoryArena.Allocate<FallObject>(sizeOfObject);

        // Reset offset memory after memory arena allocation has finished
        memoryArena.ResetOffsetMemory();

        // Find falling objects once they're spawned to check if they're destroyed where we free memory
        fallingObjects = GameObject.FindGameObjectsWithTag("Falling");

#if DEBUG
        StringBuilder memoryArenaSize = new StringBuilder("Memory Arena size in bytes: ");
        memoryArenaSize.Append(memoryArena.GetMemorySize());

        // Return the total memory of the arena in bytes
        Debug.Log(memoryArenaSize);
#endif

        spawnTimer = 0.0f; // Reset spawn timer
        fallingObjectsSpawned = true; // Set spawned falling objects to true
    }

    private void FreeMemoryArena()
    {
        for (int i = 0; i < fallingObjects.Length; i++)
        {
            if (fallingObjects[i] == null)
            {
                // Find remaining falling objects once one of the falling objects becomes null
                fallingObjects = GameObject.FindGameObjectsWithTag("Falling");
            }
        }

        // Free memory arena and fall object pointer here once falling objects aren't found anymore
        if (fallingObjects.Length <= 0)
        {
            if (memoryArena != null)
            {
                memoryArena.Dispose();
                memoryArena = null;
            }

            if (fallObject != null)
            {
                fallObject->Dispose();
                fallObject = null;
            }

            // Set spawned falling objects back to false to start spawning more
            fallingObjectsSpawned = false;
        }
    }

    public void DestroyFallingObjects()
    {
        // Destroy any falling objects
        for (int i = 0; i < fallingObjects.Length; i++)
        {
            if (fallingObjects[i] != null)
            {
                fallingObjects[i].GetComponent<FallingObject>().DestroyObject();
                fallingObjects[i] = null;
            }
        }
    }

    public void ClearFallingObjectsMemory()
    {
        // Release memory arena if there is one allocated already
        if (memoryArena != null)
        {
            memoryArena.Dispose();
            memoryArena = null;
        }

        // Release fall object too
        if (fallObject != null)
        {
            fallObject->Dispose();
            fallObject = null;
        }

        // Set falling object to null
        fallingObject = null;

        // Destroy falling object spawner as well
        Destroy(gameObject);
    }
}