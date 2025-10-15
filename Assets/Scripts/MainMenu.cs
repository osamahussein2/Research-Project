using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Memory object for grabbing references to inactive/active game objects
    public Memory memoryObject;

    // Main menu objects
    public AssetLabelReference mainMenuAssetLabel;
    public List<Image> mainMenuImages;

    [HideInInspector] public AsyncOperationHandle mainMenuGroupHandle;

    // Player object for loading it after pressing play
    public AssetLabelReference playerAssetLabel;
    [HideInInspector] public AsyncOperationHandle playerGroupHandle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start off by loading the main menu images
        LoadMainMenu();
    }

    public void PressPlayButton()
    {
        if (mainMenuGroupHandle.IsValid())
        {
            // Set play button sprite to null
            mainMenuImages[0].sprite = null;

            // Set quit button sprite to null
            mainMenuImages[1].sprite = null;

            // Release main menu group handle from memory
            Addressables.Release(mainMenuGroupHandle);
            
            // Hide main menu canvas as well
            gameObject.SetActive(false);

            // Allocate player into the game
            LoadPlayerPrefab();
        }
    }

    public void PressQuitButton()
    {
        if (mainMenuGroupHandle.IsValid())
        {
            // Set play button sprite to null
            mainMenuImages[0].sprite = null;

            // Set quit button sprite to null
            mainMenuImages[1].sprite = null;

            // Release main menu group handle from memory
            Addressables.Release(mainMenuGroupHandle);

            // Stop the editor from running afterwards
            EditorApplication.isPlaying = false;
        }
    }

    public void LoadPlayerPrefab()
    {
        // Only 1 game object will be loaded here (player) so that's why LoadAssetAsync was used here
        Addressables.LoadAssetAsync<GameObject>(playerAssetLabel).Completed += playerHandle =>
        {
            // Load player
            Instantiate(playerHandle.Result);

            // Assign player group handle to the lambda's captured player handle variable
            playerGroupHandle = playerHandle;
        };
    }

    // Load main menu
    public void LoadMainMenu()
    {
        Addressables.LoadAssetsAsync<Sprite>(mainMenuAssetLabel, null).Completed += handle =>
        {
            // Load play button sprite
            mainMenuImages[0].sprite = handle.Result[0];

            // Load quit button sprite
            mainMenuImages[1].sprite = handle.Result[1];

            // Assign main menu group handle to the lambda's captured handle variable
            mainMenuGroupHandle = handle;
        };
    }
}
