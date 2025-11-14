using UnityEngine;
using UnityEngine.AddressableAssets;

public class PauseMenu : MonoBehaviour
{
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find any object with the player script inside it to allocate/deallocate pause menu
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        // Only deallocate the pause menu when resuming
        player.pauseMenu = player.DeallocatePauseMenu;
        player.pauseMenu();
    }

    public void QuitToMainMenu()
    {
        // Deallocate the pause menu and the player when quitting to main menu
        player.pauseMenu = player.DeallocatePauseMenu;
        player.pauseMenu();

        player.destroyPlayer = player.DeallocatePlayer;
        player.destroyPlayer();
    }
}
