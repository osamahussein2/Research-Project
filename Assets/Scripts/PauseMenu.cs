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

        // Hide in game UI when pause menu is created only if the player isn't null
        if (player != null) player.inGameUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGame()
    {
        // Show in game UI again when resuming
        player.inGameUI.gameObject.SetActive(true);

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
