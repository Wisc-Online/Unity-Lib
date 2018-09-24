using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FVTC.LearningInnovations.Unity.Extensions;

namespace FVTC.LearningInnovations.Unity
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            this.gameObject.DontDestroyOnLoad();
        }
    }
}
