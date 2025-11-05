using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public unsafe class Player : MonoBehaviour
{
    // Falling object to spawn
    public GameObject fallingObject;

    // Memory object reference if it can find it somewhere in the scene
    private Memory memoryObject;

    // Update player's position
    private Vector3 playerPosition;

    // Time to spawn falling objects
    private float spawnTimer;

    private MemoryArena memoryArena;

    private bool fallingObjectsSpawned = false;

    private FallObject* fallObject;
    
    [SerializeField] private GameObject[] fallingObjects;

    [Header("Player Parameters")]

    // Made this variable show up in inspector to modify player's speed
    [SerializeField] private float playerSpeed = 5.0f;

    [Header("Falling Object Parameters")]

    // Can modify the number of falling objects spawning in the inspector
    [SerializeField] private int numberOfFallingObjects = 6;

    [SerializeField] private float fallingObjectXLocation = -5.0f;
    [SerializeField] private float fallingObjectXOffset = 2.0f;

    // Delegates stored here
    delegate void HandleMovement();
    HandleMovement playerMovement;

    delegate void DestroyPlayer();
    DestroyPlayer destroyPlayer;

    delegate void FallingObjectsLogic();
    FallingObjectsLogic spawnFallingObjects;

    delegate void ReleaseMemoryArena();
    ReleaseMemoryArena releaseMemoryArena;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure the player can grab reference to the game object that has the Memory script inside it
        memoryObject = FindAnyObjectByType<Memory>();

        // Load the falling object prefab
        fallingObject = Resources.Load<GameObject>("Falling Object");
    }

    // Update is called once per frame
    void Update()
    {
        // Call player movement delegate and invoke it
        playerMovement = MovePlayer;
        playerMovement();

        // Call destroy player delegate once the ESCAPE key has been pressed and invoke it
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            destroyPlayer = DeallocatePlayer;
            destroyPlayer();
        }

        // If falling objects aren't spawned yet
        if (!fallingObjectsSpawned)
        {
            spawnTimer += Time.deltaTime;

            // Spawn falling objects after 1 second
            if (spawnTimer >= 1.0f)
            {
                spawnFallingObjects = SpawnFallingObjects;
                spawnFallingObjects();
            }
        }

        // Otherwise, the falling objects have spawned
        else if (fallingObjectsSpawned)
        {
            // Eventually release memory arena after falling objects have been destroyed
            releaseMemoryArena = FreeMemoryArena;
            releaseMemoryArena();
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

        // Return the total memory of the arena in bytes
        Debug.Log("Memory Arena size in bytes: " + memoryArena.GetMemorySize());

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

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move player around
        playerPosition += new Vector3(horizontal, vertical, 0.0f) * playerSpeed * Time.deltaTime;

        // Clamp player's position to be in the screen at all times
        playerPosition.x = Mathf.Clamp(playerPosition.x, -10.5f, 10.5f);
        playerPosition.y = Mathf.Clamp(playerPosition.y, -3.5f, 3.5f);

        // Set transform position of player to the player position variable
        transform.position = playerPosition;
    }

    private void DeallocatePlayer()
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

        // Destroy player
        Destroy(gameObject);

        // Release player group handle from memory
        Addressables.Release(memoryObject.mainMenu.playerGroupHandle);

        // Load the main menu button images once again after quitting
        memoryObject.mainMenu.gameObject.SetActive(true);
        memoryObject.mainMenu.LoadMainMenu();
    }
}
