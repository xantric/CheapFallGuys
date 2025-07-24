using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    
    public void MainMenu() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    public void RestartGame() {
        GameManager.Instance.RestartLevel();
    }

    public void Quit(){
        Application.Quit();
        Debug.Log("Application Quit");
    }
}
