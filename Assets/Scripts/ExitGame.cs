using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ExitGamee()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // isjungia play zaidima redaktoriuje
        #else
            Application.Quit(); // isjungia buildinta zaidima
        #endif
    }
}
