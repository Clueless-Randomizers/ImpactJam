using System.Collections.Generic;
using System.Linq;
using _Scripts.ScriptableObjects;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.RefactoredSampleScripts.Builder
{
    [RequireComponent(typeof(Damageable))]
    public class Building : MonoBehaviour
    {
        [SerializeField]
        private List<SO_Currency> buildingCosts; // This will hold the cost of resources for building
        
        public string buildingName;
        [SerializeField] float height;
        public float radius = 5;
        float originalHeight;
        [SerializeField] int totalWorkToComplete = 100;
        int currentWork;
        public int[] resourceCost = default;
        Transform buildingTransform;
        [HideInInspector] public Damageable attackable;
        public bool isHover = false;
        private bool done;
        [ColorUsage(true, true)]
        [SerializeField] private Color[] stateColors;
        MeshRenderer buildingRender;
        Cinemachine.CinemachineImpulseSource impulse;
        private Tween buildingTween;

        private void Awake()
        {
            attackable = GetComponent<Damageable>();
        }
        public List<SO_Currency> GetBuildingCosts()
        {
            return buildingCosts;
        }

        void Start()
        {
            buildingTransform = transform.GetChild(0);
            buildingRender = buildingTransform.GetComponent<MeshRenderer>();
            impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
            currentWork = 0;
            originalHeight = buildingTransform.localPosition.y;
            buildingTransform.localPosition = Vector3.down * height;
            buildingTween = buildingTransform.DOLocalMoveY(originalHeight, (float)totalWorkToComplete / totalWorkToComplete).From().SetEase(Ease.OutBack).SetAutoKill(false).Pause();

        }
        public void Build(int work)
        {
            currentWork += work;
            buildingTransform.localPosition = Vector3.Lerp(Vector3.down * height, new Vector3(0,originalHeight,0), (float)currentWork / totalWorkToComplete);

            buildingTween.Play();
            //visual
            buildingTransform.DOComplete();
            buildingTransform.DOShakeScale(.5f, .2f, 10, 90, true);
            BuildingManager.instance.PlayParticle(transform.position);
        }
        public bool IsFinished()
        {
            if (currentWork >= totalWorkToComplete && !done && buildingRender)
            {
                done = true;
                buildingRender.material.DOColor(stateColors[1], "_EmissionColor", .1f).OnComplete(() => buildingRender.material.DOColor(stateColors[0], "_EmissionColor", .5f));
                if (impulse)
                    impulse.GenerateImpulse();
            }
            return currentWork >= totalWorkToComplete;
        }
        public bool CanBuild(List<SO_Currency> resources)
        {
            if (resources.Count >= resourceCost.Length)
            {
                for (int i = 0; i < resourceCost.Length; i++)
                {
                    if (resources[i].Value < resourceCost[i])
                    {
                        Debug.Log($"Not enough of resource {i}: have {resources[i].Value}, need {resourceCost[i]}");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                // Handle the case where resources has fewer elements than resourceCost
                Debug.Log("Not enough types of resources");
                return false;
            }
        }
        public int[] Cost()
        {
            return resourceCost;
        }

        private void OnMouseEnter()
        {
            isHover = true;
        }
        private void OnMouseExit()
        {
            isHover = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
