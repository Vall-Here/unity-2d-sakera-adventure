using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : Singleton<CanvasController>
{
    private Canvas canvas;

    protected override void Awake() {
        base.Awake();
        canvas = GetComponent<Canvas>();
        
    }

 
    public void SetCanvasCameraRender() {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 10;  
    }

    private void OnEnable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        SetCanvasCameraRender();  
    }

}
