using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _controlsScreen;

    public void PlayGame(){
        SceneManager.LoadScene(1);
    }

    public void Controls(){
        if(!_controlsScreen.activeInHierarchy){
            _controlsScreen.SetActive(true);
        }
        else{
            _controlsScreen.SetActive(false);
        }
    } 

    public void QuitGame(){
        Application.Quit();
    }
}
