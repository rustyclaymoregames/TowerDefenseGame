using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void RetryGame()
    {
        // Load the game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1 ); //might need to change this logic as it loads the next screen in the build index
    }
    public void PlayGame()
    {
        // Load the game scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //might need to change this logic as it loads the next screen in the build index
    }

    public void PlayGameAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2); //retrurns to the game scene
    }

    public void QuitGame()
    {
        // Quit the game
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Level1()
    {
        SceneManager.LoadScene("Test");
    }

    public void Level2()
    {
        SceneManager.LoadScene("LevelTwo");
    }

    public void Level3()
    {
        SceneManager.LoadScene("LevelThree");
    }

    public void Level4()
    {
        SceneManager.LoadScene("LevelFour");
    }

    public void Map()
    {
        SceneManager.LoadScene("Map");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NewGame()
    {
        //Delete all the player prefs related to stored levels
        PlayerPrefs.DeleteKey("Test_Completed");
        PlayerPrefs.DeleteKey("LevelTwo_Completed");
        PlayerPrefs.DeleteKey("LevelThree_Completed");
        PlayerPrefs.DeleteKey("LevelFour_Completed");

        Debug.Log("New Game Started. All completion data has been reset.");
    }
}
