using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    inGame,
    inMenu,
    gameOver,
    gameWin,
}


public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;
    [SerializeField] private Canvas inMenuCanvas, inGameCanvas, gameOverCanvas, gameWinCanvas; //At menu scene, there are 3 empty canvas that we dont use there, but in this way we need them to not get an exception, you can keep them or try to manage the exception by other ways.
    public GameState currentGameState;
    public static bool gameWon, gameOver;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button playAgainButton;

    //public static bool setNewLevelState = false; <<<You can use this in Update method for reset some parameters without loading the entire scene as I do here, I left an example below.

    private void Awake()
    {
        sharedInstance = this;
    }

    //In this GameManager I use Canvas as examples for every state, you can set whatever you want to happen at every state of your game.
    void SetGameState(GameState newGameState)
    {
        if(newGameState == GameState.inGame)
        {
            inGameCanvas.enabled = true;
            gameOverCanvas.enabled = false;
            gameWinCanvas.enabled = false;

        }else if(newGameState == GameState.gameOver)
        {
            inGameCanvas.enabled = false;
            gameOverCanvas.enabled = true;
            gameWinCanvas.enabled = false;
            resetButton.Select();
        }
        else if(newGameState == GameState.gameWin)
        {
            inGameCanvas.enabled = false;
            gameOverCanvas.enabled = false;
            gameWinCanvas.enabled = true;
            playAgainButton.Select();
        }
        else if(newGameState == GameState.inMenu)
        {
            SceneManager.LoadScene("Menu");
        }

        currentGameState = newGameState;
    }

    private void Update()
    {  
        //I know is not the best practice to use if statements inside another if, but for this example code is enough, if you want to use it, you can use a switch statement instead.
        if (sharedInstance.currentGameState == GameState.inGame)
        {
            if (gameWon)
            {
                SetGameState(GameState.gameWin);
            }
            else if (gameOver)
            {
                SetGameState(GameState.gameOver);
            }
        }

        /*
       >>> Example of using setNewLevelState without loading a scene.
       
        if (setNewLevelState)
        {
            SpawnerEnemies.enemiesDefeat = 0;
            gameWon = false;
            gameOver = false;
            SetGameState(GameState.inGame);
            setNewLevelState = false;
        } 
        <<< 
        */

    }

    private void OnGUI()
    {
        GUILayout.TextArea("Game State is: " + currentGameState);
    }

    public void StartGameButton()
    {
        SpawnerEnemies.enemiesDefeat = 0;
        gameWon = false;
        gameOver = false;
        SceneManager.LoadScene("Game");
        SetGameState(GameState.inGame);
    }

    public void GoToMenu()
    {
        SetGameState(GameState.inMenu);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
}
