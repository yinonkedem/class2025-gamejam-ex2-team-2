using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenChanger : MonoBehaviour
{
    public static ScreenChanger Instance { get; private set; }
    private const string OPENING_SCREEN = "Opening Screen";
    private const string WINNING_SCREEN = "Winning Screen";
    private const string GAME_OVER_SCREEN = "GameOver Screen";
    private const string MAIN_SCREEN = "Main";



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[Singleton] Trying to instantiate a second instance of a singleton class.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    private void Start()
    {
        //Activate Start Game object
        GameObject startGame = Utils.Instance.FindInactiveObjectByName(OPENING_SCREEN);
        startGame.SetActive(true);
        //Deactivate Main object
        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(false);
    }

    public void ResetGame()
    {
        Debug.Log("Resetting game");
        StartCoroutine(ReloadSceneAndDeactivateStartGame());
    }

    private IEnumerator ReloadSceneAndDeactivateStartGame()
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Deactivate the "Start Game" object after the scene has loaded
        GameObject startGame = Utils.Instance.FindInactiveObjectByName(OPENING_SCREEN);
        if (startGame != null)
        {
            startGame.SetActive(false);
            Debug.Log("Start Game object deactivated.");
        }
        else
        {
            Debug.LogError("Start Game object not found.");
        }

        // Reactivate other game objects if needed
        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(true);
    }


    public void ActivateWinningGame()
    {
        GameObject winningObject = Utils.Instance.FindInactiveObjectByName(WINNING_SCREEN);

        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(false);


        winningObject.SetActive(true);

        Debug.Log("Winning Game activated.");
    }

    public void ActivateGameOver()
    {
        GameObject gameOverObject = Utils.Instance.FindInactiveObjectByName(GAME_OVER_SCREEN);

        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(false);

        gameOverObject.SetActive(true);

        Debug.Log("Game Over activated.");

    }


    public void StartTheGame()
    {

        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(true);

        GameObject startGame = Utils.Instance.FindInactiveObjectByName(OPENING_SCREEN);
        startGame.SetActive(false);
        
        // Activate the MultipleTargetCamera scrit in the MainCamera
        Camera.main.GetComponent<MultipleTargetCamera>().enabled = true;
    }

}
