using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private const int mainMenu = 0;
    [SerializeField] private const int gameOver = 1;
    [SerializeField] private const int inGame = 2;
    [SerializeField] private const int highScore = 3;

    public static void DoLoadScene(Scenes scene)
    {
        switch (scene) {
            case Scenes.MainMenu:
                SceneManager.LoadScene(mainMenu);
                break;
            case Scenes.GameOver:
                SceneManager.LoadScene(gameOver);
                break;
            case Scenes.HighScore:
                SceneManager.LoadScene(highScore);
                break;
            case Scenes.InGame:
                SceneManager.LoadScene(inGame);
                break;
            default:
                Debug.Log("Scene you wanted to load does not exist");
                return;
        }
    }

    public void DoLoadScene(int scene)
    {
        Debug.Log("loading");
        SceneManager.LoadScene(scene);
    }
    
}
