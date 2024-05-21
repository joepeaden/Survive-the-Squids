using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    // Since the PlayerProgression object is in this scene, it makes it more likely
    // that it will be for sure done loading before the menu scene is brought up.
    // However it would be a good idea to not load menu until progression
    // is verified to be done loading.

    private void Start()
    {
        SceneManager.LoadScene("Menu");
    }
}
