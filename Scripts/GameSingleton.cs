using FVTC.LearningInnovations.Unity.Extensions;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public abstract class GameSingleton<T> : SceneSingleton<T> where T : GameSingleton<T>
    {
       
    }
}