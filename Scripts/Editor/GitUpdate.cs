using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace FVTC.LearningInnovations.Unity.Editor
{
    class GitUpdate
    {
        const string UNITY_LIB_MODULE_URL = "https://github.com/Wisc-Online/Unity-Lib.git";

        [MenuItem("Learning Innovations/Git/Update/Unity-Lib")]
        static void Update()
        {
            GitHelper.UpdateSubmodule(UNITY_LIB_MODULE_URL);
        }
    }
}
