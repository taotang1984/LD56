using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour, IPointerClickHandler
{
    public MainControl mainControl;
    private void Start() {
        mainControl = FindObjectOfType<MainControl>();
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        mainControl.StartGame();
    }

}
