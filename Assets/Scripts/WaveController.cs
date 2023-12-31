using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveController : MonoBehaviour
{

    private int currentWave = 1; //The current wave number we assume starts from 1
    private int maxWaves = 4; //The maximum number of waves in the game in this level we assume 1 level so far
    [SerializeField] private TextMeshProUGUI waveText; // Reference to the wave count text

    [SerializeField] private List<EnemySpawner> enemySpawners; // Reference to the enemy spawners

    [SerializeField] private Button startButton; //Reference to the start button

    [SerializeField] private Button continueButton; //Reference to the continue button

    private List<GameObject> activeEnemies;

    [SerializeField] private MoneyCounter moneyCounter; // Reference to the MoneyCounter script

    private AudioManager audioManager; //reference to the audio manager that plays the bg music


    private bool startButtonClicked = false; //Track if the start button has been clicked

    public bool IsStartButtonClicked()
    {
        return startButtonClicked;
    }

    [SerializeField] private AudioSource winSoundEffect;


    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();

        activeEnemies = new List<GameObject>();
        // Debug.Log("INITIAL Active enemies:" + activeEnemies.Count);
        continueButton.gameObject.SetActive(false); //Hide the continue button in the UI from the start
        UpdateWaveText(); //Update the wave count text in the canvas UI from the start

        moneyCounter = FindFirstObjectByType<MoneyCounter>(); //Find the MoneyCounter component

        moneyCounter.LoadMoney(); // Load the money value when the script starts

        //add an onlcick listener for the start button
        startButton.onClick.AddListener(OnStartButtonClicked);

        continueButton.onClick.AddListener(OnContinueButtonClicked); //add an onlcick listener for the continue button

    }

    void OnStartButtonClicked()
    {
        if (currentWave == 1 && !startButtonClicked) //Check if the current wave is 1
        {
            startButtonClicked = true; //Set the start button clicked to true
            // Debug.Log("Start button clicked!");
            startButton.gameObject.SetActive(false); //Hide the start button in the UI
            currentWave = 1; //Set the current wave to 1
            StartCoroutine(SpawnWave()); //Start spawning enemies
        }
    }

    public IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(0.5f); //Wait for half a second to let it load in @TODO might have to fix this later so no delay is present but enemy was invisible otherwise  
        while (currentWave < maxWaves) //Check if the current wave is less than the maximum number of waves
        {
            if (startButtonClicked)
            {
 
                foreach (EnemySpawner enemySpawner in enemySpawners)
                {
                    yield return new WaitForSeconds(5.0f); //Wait for 5 seconds
                    enemySpawner.SpawnEnemy(this); //Spawn an enemy
                    // Debug.Log("Active enemies:" + activeEnemies.Count);
                }
            }

            currentWave++; //Increment the current wave
            UpdateWaveText(); //Update the wave count text in the canvas UI
        }
        if (currentWave == maxWaves)
        {                
            yield return new WaitForSeconds(2.0f); //Wait for bug
            Debug.Log("Reached max waves. stop spawning enemies");
            foreach (EnemySpawner enemySpawner in enemySpawners)
            {
                // Debug.Log("Active enemies STOP SPAWNING:" + activeEnemies.Count);
                enemySpawner.StopSpawning(); //Stop spawning enemies
            }
            yield return new WaitUntil(() => activeEnemies.Count == 0); //Wait until all enemies are dead

            //Save the level completion immediately after the last enemy is killed
            SetLevelCompleted(SceneManager.GetActiveScene().name); //Set the level to completed
            
            //Click Continue to start the next wave 
            continueButton.gameObject.SetActive(true); // Show the continue button in the UI

            //sound play if there is an audio source attached
            if (winSoundEffect != null)
            {
                winSoundEffect.Play();
            }

            // Wait until the continue button is clicked
            yield return new WaitUntil(() => continueButtonClicked);

            Debug.Log("Continue button clicked!");

            // Load the next level
            LoadNextLevel();
        }

        Debug.Log("Game appears to be over, and you seem to have survived! The final enemy count: " + activeEnemies.Count);
    }

    private void LoadNextLevel()
    {
        if (activeEnemies.Count == 0)
        {
            Debug.Log("Won it all!");

            string activeSceneName = SceneManager.GetActiveScene().name; //Get the name of the active scene


            //Load different scenes based on the active scene name
            switch (activeSceneName)
            {
                case "Test":  //really need to change the name of this scene to Level1 
                    continueButton.gameObject.SetActive(true); //Show the continue button in the UI
                    SceneManager.LoadScene("LevelTwo"); //might need to change this name too to Level2
                    break;
                case "LevelTwo":
                if (audioManager != null)
                    {                
                    audioManager.StopBackgroundMusic(); //stop the background music
                    }
                    SceneManager.LoadScene("LevelThree");
                    break;
                case "LevelThree":
                    SceneManager.LoadScene("LevelFour");
                    break;
                case "LevelFour":
                    continueButton.gameObject.SetActive(false); //Hide the continue button to just go straight to the win screen
                    SceneManager.LoadScene("Win");
                    break;
            }
        }        
    }

    private void SetLevelCompleted(string levelName)
    {
        PlayerPrefs.SetInt(levelName + "_Completed", 1); //Set the level to completed
    }

    private bool continueButtonClicked = false;

    private void OnContinueButtonClicked()
    {
        continueButtonClicked = true;
    }

    public List<GameObject> GetActiveEnemies()
    {
        return activeEnemies;
    }

    public void RemoveEnemy(GameObject thingToRemove)
    {
        activeEnemies.Remove(thingToRemove);
        // Debug.Log("WaveController REMOVE AFTER active enemies count: "+activeEnemies.Count);        
    }

    public void AddEnemy(GameObject thingToAdd)
    {
        // Debug.Log("WaveController ADD BEFORE active enemies count: "+activeEnemies.Count);
        activeEnemies.Add(thingToAdd);
        // Debug.Log("WaveController ADD AFTER active enemies count: "+activeEnemies.Count);        
    }

    public void ClearEnemies()
    {
        // Debug.Log("WaveController CLEAR BEFORE active enemies count: "+activeEnemies.Count);
        activeEnemies.Clear();
        // Debug.Log("WaveController CLEAR AFTER active enemies count: "+activeEnemies.Count);        
    }

    private void UpdateWaveText()
    {
        waveText.text = "Wave: " + currentWave + "/" + maxWaves; //Update the wave count text in the canvas UI
    }
}
