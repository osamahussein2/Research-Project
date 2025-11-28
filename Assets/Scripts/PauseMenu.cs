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

        // Hide in game UI when pause menu is created
        InGameUI.GetGameObject().SetActive(false);
    }

    public void ResumeGame()
    {
        // Show in game UI again when resuming
        InGameUI.GetGameObject().SetActive(true);

        // Only deallocate the pause menu when resuming
        player.DeallocatePauseMenu();
    }

    public void QuitToMainMenu()
    {
        // Deallocate the pause menu and the player when quitting to main menu
        player.DeallocatePauseMenu();
        player.DeallocatePlayer();
    }
}
