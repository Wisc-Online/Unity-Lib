using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity
{
    [System.Serializable]
    public class Layer
    {
        public int layerIndex;

        public string GetName()
        {
            return LayerMask.LayerToName(layerIndex);
        }
    }
}