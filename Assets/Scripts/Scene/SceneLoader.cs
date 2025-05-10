using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static bool IsLoading { get; private set; } = false;

    private static bool SceneExists(string name)
    {
        return Enumerable.Range(0, SceneManager.sceneCount)
            .Select(SceneManager.GetSceneAt)
            .Any(scene => scene.name == name) || 
            Application.CanStreamedLevelBeLoaded(name);
    }

    public static void Load(Scene scene, SceneTransition transition = null)
    {
        if (IsLoading)
        {
            return;
        }

        string sceneName = $"scn_{scene}".Trim();

        if (!SceneExists(sceneName))
        {
            Debug.LogError($"No scene with name '{sceneName}' found.");
            return;
        }

        IsLoading = true;

        if (transition == null)
        {
            transition = SceneTransition.Get();
        }

        PlayerManager.Instance.DisableAllInput();

        transition.gameObject.SetActive(true);
        Sequence inSequence = DOTween.Sequence();
        inSequence.OnComplete(OnTweenInComplete);
        transition.TweenIn(inSequence);

        async void OnTweenInComplete()
        {
            MenuNavigator.Clear();
            LoadingScreen.Instance.Show();

            // Scene load starts
            await SceneManager.LoadSceneAsync(sceneName);
            
            // Small delay because scene transitions don't play their tweens correctly
            // right after the scene has loaded
            await Task.Delay(500);
            
            // Scene load completes
            LoadingScreen.Instance.Hide();

            Sequence outSequence = DOTween.Sequence();
            outSequence.OnComplete(OnTweenOutComplete);
            transition.TweenOut(outSequence);

            void OnTweenOutComplete()
            {
                // Scene load is fully complete.
                transition.gameObject.SetActive(false);
                IsLoading = false;
                PlayerManager.Instance.EnableInput();
            }
        }
    }
}
