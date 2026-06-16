using System;
using System.ComponentModel;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public UIDocument uiDocument;
    public string playButtonName, quitButtonName;
    private Button playButton, quitButton;

    void Awake()
    {
        VisualElement root = uiDocument.rootVisualElement;
        playButton = root.Q<Button>(playButtonName);
        quitButton = root.Q<Button>(quitButtonName);
        playButton.clicked += StartGame;
        quitButton.clicked += QuitGame;
    }

    void OnDisable()
    {
        playButton.clicked -= StartGame;
        quitButton.clicked -= QuitGame;
    }
    private void StartGame()
    {
        SceneManager.LoadScene(1);

    }

    private void QuitGame()
    {
        Application.Quit(0);
    }

}
