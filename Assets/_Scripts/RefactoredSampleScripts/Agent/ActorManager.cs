using Prototype.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.RefactoredSampleScripts.Agent
{
    public class ActorManager : MonoBehaviour
    {
        public static ActorManager instance;
        [SerializeField] LayerMask actorLayer = default;
        [SerializeField] Transform selectionArea = default;
        public List<Actor> allActors = new List<Actor>();
        [SerializeField] List<Actor> selectedActors = new List<Actor>();
        Camera mainCamera;
        Vector3 startDrag;
        Vector3 endDrag;
        Vector3 dragCenter;
        Vector3 dragSize;
        bool dragging;
        private void Awake()
        {
            instance = this;
        }
        void Start()
        {
            mainCamera = Camera.main;
            foreach (Actor actor in GetComponentsInChildren<Actor>())
            {
                allActors.Add(actor);
            }

            selectionArea.gameObject.SetActive(false);
        }

        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                dragging = false;
				return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                startDrag = Utility.MouseToTerrainPosition();
                endDrag = startDrag;
            }
            else if (Input.GetMouseButton(0))
            {
                endDrag = Utility.MouseToTerrainPosition();

                if (Vector3.Distance(startDrag, endDrag) > 1)
                {
                    selectionArea.gameObject.SetActive(true);
                    dragging = true;
                    dragCenter = (startDrag + endDrag) / 2;
                    dragSize = (endDrag - startDrag);
                    selectionArea.transform.position = dragCenter;
                    selectionArea.transform.localScale = dragSize + Vector3.up;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (dragging)
                {
                    SelectActors();
                    dragging = false;
                    selectionArea.gameObject.SetActive(false);
                } 
                else
                {
					Actor _actor = Utility.MouseToActor();

					if (_actor != default) {
						selectedActors.Add( _actor );
						_actor.visualHandler.Select();
					} else {
						DeselectActors();
					}
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (!dragging)
                {
                    SetTask();
                }
            }
        }

        void SetTask()
        {
            if (selectedActors.Count == 0) { 
                return;
            }
            
            Collider collider = Utility.CameraRay().collider;
            if (collider.CompareTag("Terrain"))
            {
                foreach (Actor actor in selectedActors)
                {
                    GatheringAI _gatheringAI = actor.GetComponent<GatheringAI>();
                    _gatheringAI.CancelGatheringAndGoHere(Utility.MouseToTerrainPosition());
                }
            }
            
            // If the tag is Player, then we will check if we can find different components to decide what to do.
            if (collider.CompareTag("Player")) { return; }

            if (collider.TryGetComponent(out GatheringBuilding building))
            {
                foreach (Actor actor in selectedActors)
                {
                    actor.AttachBuilding(building);
                }
            }
            
            if (collider.TryGetComponent(out Damageable damageable))
            {
                foreach (Actor actor in selectedActors)
                {
                    actor.AttackTarget(damageable);
                }
            }
        }

        void SelectActors()
        {
            DeselectActors();
            dragSize.Set(Mathf.Abs(dragSize.x / 2), 1, Mathf.Abs(dragSize.z / 2));
            RaycastHit[] hits = Physics.BoxCastAll(dragCenter, dragSize, Vector3.up, Quaternion.identity, 0, actorLayer.value);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.TryGetComponent(out Actor actor))
                {
                    selectedActors.Add(actor);
                    actor.visualHandler.Select();
                }
            }
        }
        public void DeselectActors()
        {
            foreach (Actor actor in selectedActors)
                actor.visualHandler.Deselect();

            selectedActors.Clear();
        }

        private void OnDrawGizmos()
        {
            Vector3 center = (startDrag + endDrag) / 2;
            Vector3 size = (endDrag - startDrag);
            size.y = 1;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
