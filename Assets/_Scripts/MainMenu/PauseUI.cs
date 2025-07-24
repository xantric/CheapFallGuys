using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    


    public void Resume() {
        GameManager.Instance.ResumeGame();
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
    }
}
