using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("you clicked start");
        // Load the game scene
        SceneManager.LoadScene("SampleScene"); // Use the exact name of your game scene
        
    }

    public void QuitGame()
    {
        // Quits the application (works in a built game, use Debug.Log for testing in the editor)
        Application.Quit();
        Debug.Log("Game has quit!");
    }

    // Add functions for Options, etc., as needed
    public void OpenOptionsMenu(GameObject optionsMenuCanvas)
    {
        optionsMenuCanvas.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
