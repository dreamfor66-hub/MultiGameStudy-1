using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rogue.Ingame.Temp
{
    public class ResetScene : MonoBehaviour
    {
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
            {
                SceneManager.LoadScene("GameBase");
            }
        }

    }
}