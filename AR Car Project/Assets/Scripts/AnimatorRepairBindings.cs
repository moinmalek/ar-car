using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps animator GameObject names used by repair steps to cached <see cref="Animator"/> references (replaces GameObject.Find).
/// </summary>
public sealed class AnimatorRepairBindings : MonoBehaviour
{
    [Serializable]
    public struct NamedAnimator
    {
        public string objectName;
        public Animator animator;
    }

    [SerializeField] NamedAnimator[] namedAnimators;
    [SerializeField] string idleStateName = "Empty";

    Dictionary<string, Animator> _byName;

    void Awake() => BuildMap();

    void BuildMap()
    {
        if (_byName != null)
            return;

        _byName = new Dictionary<string, Animator>(StringComparer.Ordinal);
        if (namedAnimators == null)
            return;

        for (int i = 0; i < namedAnimators.Length; i++)
        {
            NamedAnimator na = namedAnimators[i];
            if (string.IsNullOrEmpty(na.objectName) || na.animator == null)
                continue;
            _byName[na.objectName] = na.animator;
        }
    }

    public bool TryGetAnimator(string objectName, out Animator animator)
    {
        BuildMap();

        if (string.IsNullOrEmpty(objectName))
        {
            animator = null;
            return false;
        }

        return _byName.TryGetValue(objectName, out animator);
    }

    /// <summary>
    /// Returns animators to the idle pose without relying on animator transitions (issue #11 — deterministic end state).
    /// </summary>
    public void ResetAllToIdle()
    {
        if (_byName == null)
            return;

        foreach (var kv in _byName)
        {
            Animator a = kv.Value;
            if (a != null && !string.IsNullOrEmpty(idleStateName))
                a.Play(idleStateName, 0, 0f);
        }
    }
}
