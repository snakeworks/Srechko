using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class SceneTransition : MonoBehaviour
{   
    public const string DefaultTransitionName = "Default";

    private static readonly Dictionary<string, SceneTransition> _transitions = new();

    private void Awake()
    {
        _transitions.Add(name, this);
        Init();
        gameObject.SetActive(false);
    }

    protected abstract void Init();
    
    /// <summary>
    /// Checks if a scene transition by this <c>name</c> exists and returns it.
    /// If no arguments are passed, returns a transition with the name <c>Default</c>.
    /// </summary>
    /// <param name="name">Name of the transition.</param>
    /// <returns>The requested scene transition, if it exists, otherwise null.</returns>
    public static SceneTransition Get(string name = DefaultTransitionName)
    {
        if (_transitions.TryGetValue(name, out var transition))
        {
            return transition;
        }
        return null;
    }

    /// <summary>
    /// Plays the 'in' part of the transition.
    /// </summary>
    /// <param name="sequence">The sequence that the tweens must be appended or inserted into.</param>
    public abstract void TweenIn(Sequence sequence);

    /// <summary>
    /// Plays the 'out' part of the transition.
    /// </summary>
    /// <param name="sequence">The sequence that the tweens must be appended or inserted into.</param>
    public abstract void TweenOut(Sequence sequence);
}
