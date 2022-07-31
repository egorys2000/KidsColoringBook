using UnityEngine.SceneManagement;
using UnityEngine;
using System.Threading.Tasks;

public class SceneTransmit : MonoBehaviour
{
    [SerializeField] public string SceneToLoad;

    public void SwitchScene() 
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public async Task SwitchSceneAsync() 
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneToLoad);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            //Debug.Log("loading...");
            await Task.Yield();
        }
    }
}
