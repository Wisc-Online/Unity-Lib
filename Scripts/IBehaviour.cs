namespace FVTC.LearningInnovations.Unity
{
    interface IBehaviour
    {
        void OnAwake();
        void OnStart();
        void OnUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
    }
}
