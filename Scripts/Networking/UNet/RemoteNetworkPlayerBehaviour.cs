namespace FVTC.LearningInnovations.Unity.Networking.UNet
{
    public abstract class RemoteNetworkPlayerBehaviour : NetworkPlayerBehaviour
    {
        public override void OnStart()
        {
            base.OnStart();

            if (isLocalPlayer)
            {
                enabled = false;
                Destroy(this);
            }
            else
            {
                OnPlayerStart();
            }
        }
        protected virtual void OnPlayerStart()
        {
            // do nothing, to be overridden by decedents
        }
    }
}