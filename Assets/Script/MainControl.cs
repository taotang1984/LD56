using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainControl : MonoBehaviour
{
    public int goalPoints;

    private void Start() {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }

    public void StartGame()
    {
        SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
    }


    public void RestartGame()
    {
        SceneManager.UnloadSceneAsync("Main");
        goalPoints = 15;
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
    public void NextLevel()
    {
        SceneManager.UnloadSceneAsync("Main");
        goalPoints += 5;
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
}
