using UnityEngine;
using UnityEngine.AI;

namespace _Scripts
{
    public class AttemptedAiTownsperson : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
    
        private bool isMoving = false;
    
        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating(nameof(PickRandomLocation), 0, 5);
        }

        private void PickRandomLocation()
        {
            Vector3 randomDirection = Random.insideUnitSphere * 20;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 20, 1);
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
        }
    
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
