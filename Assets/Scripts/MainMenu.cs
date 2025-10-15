using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AssetLabelReference mainMenuAssetLabel;
    public List<Image> mainMenuImages;

    [HideInInspector] public AsyncOperationHandle mainMenuGroupHandle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
}
