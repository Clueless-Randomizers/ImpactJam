using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.RefactoredSampleScripts.Agent
{
    [RequireComponent(typeof(Damageable))]
    public class Actor : MonoBehaviour
    {
        [SerializeField] private float attackInterval = 1f;
        NavMeshAgent agent;
        [HideInInspector] public Damageable damageable;
        [HideInInspector] public Damageable damageableTarget;
        [HideInInspector] public Animator animator;
        [HideInInspector] public AnimationEventListener animationEvent;
        [HideInInspector] public Coroutine currentTask;
        [HideInInspector] public ActorVisualHandler visualHandler;

        public bool isHover = false;
        bool isResource;
		
        private GatheringAI _gatheringAI;

		private void Awake()
        {
            damageable = GetComponent<Damageable>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            animationEvent = GetComponentInChildren<AnimationEventListener>();
            visualHandler = GetComponent<ActorVisualHandler>();
            //animationEvent.attackEvent.AddListener(Attack);
            isResource = GetComponent<Resource>() ? true : false;
            _gatheringAI = GetComponent<GatheringAI>();
        }

        public void Update()
        {
            //animator.SetFloat("Speed", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
        }

        public void SetDestination(Vector3 destination)
        {
            agent.destination = destination;
        }
        public WaitUntil WaitForNavMesh()
        {
            return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        }
        void Attack()
        {
            if (damageableTarget)
                damageableTarget.Hit(10);
        }
        public void AttackTarget(Damageable target)
        {
            StopTask();
            damageableTarget = target;

            currentTask = StartCoroutine(StartAttack());

            IEnumerator StartAttack()
            {
                float attackInterval = this.attackInterval; 
                float lastAttackTime = -attackInterval;

                while (damageableTarget)
                {
                    SetDestination(damageableTarget.transform.position);
                    yield return WaitForNavMesh();
                    while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < 4f)
                    {
                        if (Time.time >= lastAttackTime + attackInterval)
                        {
                            lastAttackTime = Time.time;
                            Attack();
                        }
                        yield return null;
                    }
                }

                currentTask = null;
            }
        }

        public void AttachBuilding (GatheringBuilding gatheringBuilding)
        {
            if (_gatheringAI != default)
            {
                _gatheringAI.SetGatheringBuilding(gatheringBuilding);
            }
        }
        public virtual void StopTask()
        {
            damageableTarget = null;
            if (currentTask != null)
                StopCoroutine(currentTask);
        }

        private void OnMouseEnter()
        {
            isHover = true;
        }
        private void OnMouseExit()
        {
            isHover = false;
        }

    }
}
