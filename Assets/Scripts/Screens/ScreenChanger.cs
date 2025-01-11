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
        if (Instance != null && Instance != this)
        {
            Debug.Log("[Singleton] Destroying duplicate ScreenChanger instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        Debug.Log("Resetting game...");
        StartCoroutine(ReloadSceneAndEnsureSingleInstance());
    }

    private IEnumerator ReloadSceneAndEnsureSingleInstance()
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Find the "Main" screen and ensure only one instance is active
        GameObject[] mainScreens = GameObject.FindGameObjectsWithTag("Main Screen");
        if (mainScreens.Length > 1)
        {
            Debug.LogWarning($"Found {mainScreens.Length} Main screens. Destroying duplicates...");
            for (int i = 1; i < mainScreens.Length; i++)
            {
                Destroy(mainScreens[i]);
            }
        }

        // Activate the remaining Main screen
        mainScreens[0].SetActive(true);
        Debug.Log("Game reset complete.");
    }

    

    // public void ResetGame()
    // {
    //     Debug.Log("Resetting game");
    //     StartCoroutine(ReloadSceneAndDeactivateStartGame());
    // }
    //
    // private IEnumerator ReloadSceneAndDeactivateStartGame()
    // {
    //     // Load the scene asynchronously
    //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    //
    //     // Wait until the asynchronous scene fully loads
    //     while (!asyncLoad.isDone)
    //     {
    //         yield return null;
    //     }
    //
    //     // Deactivate the "Start Game" object after the scene has loaded
    //     GameObject startGame = Utils.Instance.FindInactiveObjectByName(OPENING_SCREEN);
    //     if (startGame != null)
    //     {
    //         startGame.SetActive(false);
    //         Debug.Log("Start Game object deactivated.");
    //     }
    //     else
    //     {
    //         Debug.LogError("Start Game object not found.");
    //     }
    //
    //     // Reactivate other game objects if needed
    //     // GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
    //     // mainObject.SetActive(true);
    // }


    private IEnumerator WaitForEnter()
    {
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        ResetGame();
    }
    
    //regular function thar run until enter is pressed
    
    public void ActivateWinningGame()
    {
        GameObject winningObject = Utils.Instance.FindInactiveObjectByName(WINNING_SCREEN);
        winningObject.SetActive(true);

        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(false);
        
        Debug.Log("Winning Game activated.");
        
        StartCoroutine(WaitForEnter());
    }

    public void ActivateGameOver()
    {
        GameObject gameOverObject = Utils.Instance.FindInactiveObjectByName(GAME_OVER_SCREEN);
        gameOverObject.SetActive(true);

        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(false);
        
        Debug.Log("Game Over activated.");
        StartCoroutine(WaitForEnter());

    }


    public void StartTheGame()
    {

        GameObject mainObject = Utils.Instance.FindInactiveObjectByName(MAIN_SCREEN);
        mainObject.SetActive(true);

        GameObject startGame = Utils.Instance.FindInactiveObjectByName(OPENING_SCREEN);
        startGame.SetActive(false);
    }

}
