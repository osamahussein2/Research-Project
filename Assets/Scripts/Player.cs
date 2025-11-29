using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public unsafe class Player : MonoBehaviour
{
    // Memory object reference if it can find it somewhere in the scene
    private Memory memoryObject;

    // Update player's position
    private Vector3 playerPosition;

    [Header("Player Parameters")]

    // Made this variable show up in inspector to modify player's speed
    [SerializeField] private float playerSpeed = 5.0f;

    public AssetLabelReference pauseMenuAssetLabel;
    [HideInInspector] public AsyncOperationHandle pauseMenuGroupHandle;

    bool isGamePaused = false; // Set game paused to false right away

    // Collectible using object pooling
    private ObjectPool collectiblePool;

    // Modify disable object pool time in inspector
    [SerializeField] private float disableObjectPoolTime = 5.0f;

    // Falling object spawner script reference
    private FallingObjectSpawner fallingObjectSpawner;

    // Falling object to spawn
    private GameObject fallingObjectSpawnerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure the player can grab reference to the game object that has the Memory script inside it
        memoryObject = FindAnyObjectByType<Memory>();

        // Find the object pool to spawn collectibles
        collectiblePool = FindAnyObjectByType<ObjectPool>();

        // Load the falling object spawner prefab
        fallingObjectSpawnerPrefab = Resources.Load<GameObject>("FallingObjectSpawner");

        // If it's not null, spawn the falling object spawner in
        if (fallingObjectSpawnerPrefab != null)
        {
            Instantiate(fallingObjectSpawnerPrefab);
            fallingObjectSpawnerPrefab = null; // Release prefab's memory after spawning
        }

        // Grab a reference to the game object with the falling object spawner script
        fallingObjectSpawner = FindAnyObjectByType<FallingObjectSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call player movement delegate and invoke it
        MovePlayer();

        // Call handle pause game logic using a key code and game paused boolean
        HandlePauseGame(KeyCode.Escape, isGamePaused);

        // Press a key to spawn a collectible
        PressToSpawnCollectibles(KeyCode.Space);
    }

    private void HandlePauseGame(KeyCode keyCode_, bool gamePaused_)
    {
        // Call allocate pause menu delegate once the passed in key has been pressed and invoke it
        if (Input.GetKeyDown(keyCode_) && !gamePaused_)
        {
            AllocatePauseMenu();
        }

        // Call deallocate pause menu delegate once the passed in key has been pressed and invoke it
        else if (Input.GetKeyDown(keyCode_) && gamePaused_)
        {
            // Show in-game UI again
            InGameUI.GetGameObject().SetActive(true);

            DeallocatePauseMenu();
        }
    }

    private void PressToSpawnCollectibles(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode)) SpawnCollectible();
    }

    private void SpawnCollectible()
    {
        if (collectiblePool != null)
        {
            // Spawn the collectible in the pool
            GameObject collectible = collectiblePool.GetGameObject();

            // Randomize its spawn location
            collectible.transform.position = new Vector3(Random.Range(-9.0f, 9.0f), Random.Range(-3.0f, 3.0f), 0.0f);

            // Start hiding the object in a certain amount of seconds upon spawning it in pool
            StartCoroutine(DisableCollectible(collectible));
        }
    }

    IEnumerator DisableCollectible(GameObject collectible)
    {
        // Hide the object in pool after a few seconds
        yield return new WaitForSeconds(disableObjectPoolTime);

        if (collectiblePool != null) collectiblePool.ReturnGameObject(collectible);
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

    public void DeallocatePlayer()
    {
        // Destroy any falling objects
        if (fallingObjectSpawner != null) fallingObjectSpawner.DestroyFallingObjects();

        if (collectiblePool != null)
        {
            // Find any active collectibles
            GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");

            foreach (GameObject collectible in collectibles)
            {
                // Stop disable collectible coroutine
                StopCoroutine(DisableCollectible(collectible));

                // Hide the all collectible objects in the pool
                collectiblePool.ReturnGameObject(collectible);
            }
        }

        // Clear all memory from the falling object spawner script
        if (fallingObjectSpawner != null) fallingObjectSpawner.ClearFallingObjectsMemory();

        // Valid player object
        if (memoryObject.mainMenu.playerGroupHandle.IsValid())
        {
            // Destroy player
            Destroy(gameObject);

            // Release player group handle from memory
            Addressables.Release(memoryObject.mainMenu.playerGroupHandle);
        }

        // Valid in-game UI object
        if (memoryObject.mainMenu.inGameUI_GroupHandle.IsValid())
        {
            // Activate game object so that it can be destroyed afterwards (prevents null error when destroying object)
            InGameUI.GetGameObject().SetActive(true);

            // Destroy in-game UI
            Destroy(InGameUI.GetGameObject());

            // Release player group handle from memory
            Addressables.Release(memoryObject.mainMenu.inGameUI_GroupHandle);
        }

        // Load the main menu button images once again after quitting
        memoryObject.mainMenu.gameObject.SetActive(true);
        memoryObject.mainMenu.LoadMainMenu();
    }

    public void AllocatePauseMenu()
    {
        // Only 1 game object will be loaded here (pause menu) so that's why LoadAssetAsync was used here
        Addressables.LoadAssetAsync<GameObject>(pauseMenuAssetLabel).Completed += pauseHandle =>
        {
            // Load pause menu
            Instantiate(pauseHandle.Result);

            // Assign pause menu group handle to the lambda's captured pause menu handle variable
            pauseMenuGroupHandle = pauseHandle;

            // Pause
            Time.timeScale = 0.0f;

            // Set game paused to true so that when the game is actually paused, player can't pause again
            isGamePaused = true;
        };
    }

    public void DeallocatePauseMenu()
    {
        if (pauseMenuGroupHandle.IsValid())
        {
            // Destroy pause menu
            Destroy(GameObject.FindGameObjectWithTag("PauseMenu"));

            // Release pause menu group handle from memory
            Addressables.Release(pauseMenuGroupHandle);

            // Resume
            Time.timeScale = 1.0f;

            // Set game paused to false so that the player can pause game again
            isGamePaused = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            // Hide the collided collectible in pool
            collectiblePool.ReturnGameObject(collision.gameObject);
        }
    }
}
