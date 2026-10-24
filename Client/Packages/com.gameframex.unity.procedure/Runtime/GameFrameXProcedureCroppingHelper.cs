using UnityEngine;

namespace GameFrameX.Procedure.Runtime
{
    [UnityEngine.Scripting.Preserve]
    public class GameFrameXProcedureCroppingHelper : MonoBehaviour
    {
        [UnityEngine.Scripting.Preserve]
        private void Start()
        {
            _ = typeof(IProcedureManager);
            _ = typeof(ProcedureBase);
            _ = typeof(ProcedureManager);
            _ = typeof(ProcedureComponent);
        }
    }
}