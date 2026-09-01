using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [SerializeField] private LoadingUI _loadingUI;

    private const float LOADED_SCENE_RATE = 0.9f;

    public enum SceneType //ここで遷移するシーンを定義
    {
        MainMenuScene,
        TutorialLevel,
        EasyLevel,
        NormalLevel
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
            ValidateScenesInBuild();
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(SceneType scene)
    {
#if UNITY_EDITOR
        if (!SceneIsInBuildSettings(scene.ToString()))
        {
            Debug.LogError($"ScreenManager: Scene '{scene}' is not included in the active build profile or Build Settings. Add it via File -> Build Profiles or File -> Build Settings -> Add Open Scenes.");
            return;
        }
#endif
        Debug.Log($"ScreenManager: Requesting scene change to {scene}");
        StartCoroutine(LoadSceneAsyncWithLoadingUI(scene));
    }

    private IEnumerator LoadSceneAsyncWithLoadingUI(SceneType scene)
    {
        _loadingUI.gameObject.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float progress = asyncOperation.progress;
            _loadingUI.UpdateProgress(progress);

            if (progress >= LOADED_SCENE_RATE)
            {
                _loadingUI.UpdateProgress(1);
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        _loadingUI.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void ValidateScenesInBuild()
    {
        var enumNames = System.Enum.GetNames(typeof(SceneType));
        var buildScenes = UnityEditor.EditorBuildSettings.scenes;
        foreach (var name in enumNames)
        {
            bool found = false;
            foreach (var s in buildScenes)
            {
                if (s.path.EndsWith("/" + name + ".unity") || s.path.EndsWith("\\" + name + ".unity") || s.path.EndsWith(name + ".unity"))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                Debug.LogWarning($"ScreenManager: Scene '{name}.unity' for SceneType.{name} not found in Build Settings or the active build profile. Add it via File -> Build Settings -> Add Open Scenes, or File -> Build Profiles if you use the Build Profiles package.");
        }
    }

    private static bool SceneIsInBuildSettings(string sceneName)
    {
        var buildScenes = UnityEditor.EditorBuildSettings.scenes;
        foreach (var s in buildScenes)
        {
            if (s.path.EndsWith("/" + sceneName + ".unity") || s.path.EndsWith("\\" + sceneName + ".unity") || s.path.EndsWith(sceneName + ".unity"))
                return true;
        }
        return false;
    }
#endif
}
