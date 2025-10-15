using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    // Memory object reference if it can find it somewhere in the scene
    private Memory memoryObject;

    // Update player's position
    private Vector3 playerPosition;

    // Made this variable show up in inspector to modify player's speed
    [SerializeField] private float playerSpeed = 5.0f;

    // Delegates stored here
    delegate void HandleMovement();
    HandleMovement playerMovement;

    delegate void DestroyPlayer();
    DestroyPlayer destroyPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure the player can grab reference to the game object that has the Memory script inside it
        memoryObject = FindAnyObjectByType<Memory>();
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
        // Destroy player
        Destroy(gameObject);

        // Release player group handle from memory
        Addressables.Release(memoryObject.mainMenu.playerGroupHandle);

        // Load the main menu button images once again after quitting
        memoryObject.mainMenu.gameObject.SetActive(true);
        memoryObject.mainMenu.LoadMainMenu();
    }
}
