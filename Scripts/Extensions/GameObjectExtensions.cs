using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Extensions
{
    public static class GameObjectExtensions
    {
        public static GameObject GetParent(this GameObject child)
        {
            return child.transform.parent == null ? null : child.transform.parent.gameObject;
        }

        public static GameObject GetParentRoot(this GameObject child)
        {
            return child.transform.parent == null ? child : child.GetParentRoot();
        }

        public static void DontDestroyOnLoad(this UnityEngine.Object target)
        {
#if UNITY_EDITOR // Skip Don't Destroy On Load when editor isn't playing so test runner passes.
            if (UnityEditor.EditorApplication.isPlaying)
#endif
                UnityEngine.Object.DontDestroyOnLoad(target);
        }
    }
}
