using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
   private bool keyPressed = false;
    void Update()
    {
        if (Input.anyKey && !keyPressed)
        { 
            SceneManager.LoadSceneAsync(0);
        }
        else
        {
                // put your camera move code here 
        }
    }
}
