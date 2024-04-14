using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.RefactoredSampleScripts.Agent
{
    public class AnimationEventListener : MonoBehaviour
    {
        [HideInInspector] public UnityEvent footStepEvent = new UnityEvent();
        [HideInInspector] public UnityEvent attackEvent = new UnityEvent();

        public void FootstepEvent()
        {
            footStepEvent.Invoke();
        }
        public void AttackEvent()
        {
            attackEvent.Invoke();
        }
    }
}
