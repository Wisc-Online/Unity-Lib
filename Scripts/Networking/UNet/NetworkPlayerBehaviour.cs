#if !UNITY_2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FVTC.LearningInnovations.Unity.Extensions;

namespace FVTC.LearningInnovations.Unity.Networking.UNet
{
    public abstract class NetworkPlayerBehaviour : NetworkBahaviourBase
    {
        public override void OnStart()
        {
            base.OnStart();

            if ((isLocalPlayer && this.IsNot<ILocalNetworkPlayer>()) || (!isLocalPlayer && this.IsNot<IRemoteNetworkPlayer>()))
            {
                enabled = false;
                Destroy(this);
                Debug.LogFormat("Removing {0} for {1} player.", this.GetType(), isLocalPlayer ? "local" : "remote");
            }
            else
            {
                OnPlayerStart();
            }
        }

        protected virtual void OnPlayerStart()
        {

        }
    }
}
#endif