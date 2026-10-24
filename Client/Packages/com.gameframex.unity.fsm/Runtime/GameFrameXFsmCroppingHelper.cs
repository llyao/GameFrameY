using UnityEngine;

namespace GameFrameX.Fsm.Runtime
{
    [UnityEngine.Scripting.Preserve]
    public class GameFrameXFsmCroppingHelper : MonoBehaviour
    {
        [UnityEngine.Scripting.Preserve]
        private void Start()
        {
            _ = typeof(IFsmManager);
            _ = typeof(IFsm<>);
            _ = typeof(FsmState<>);
            _ = typeof(FsmBase);
            _ = typeof(FsmManager);
            _ = typeof(FsmManager);
        }
    }
}