using UnityEngine;

namespace _Project.Scripts.Render
{
    public class Render25DExecutor
    {
        public static Vector3 LookAtTarget(Vector3 targetPos, Vector3 originPos)
        {
            Vector3 targetToward = targetPos - originPos;
            targetToward.y = 0;
            
            return targetToward;
        }
    }

}
