using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utilities
{
    public static void RestartGame(int sceneNo)
    {
        SceneManager.LoadScene(sceneNo);
        Time.timeScale = 1.0f;
    }
}
