using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneLoader : ScriptableObject
{
    public void Load(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
