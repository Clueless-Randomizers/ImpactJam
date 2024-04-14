using DG.Tweening;
using UnityEngine;

namespace Prototype.Scripts
{
    public class Resource : MonoBehaviour
    {
        [SerializeField] ResourceType resourceType;
        [SerializeField] int amount;
        Damageable damageable;
        public bool isHover;

        //HoverVisual
        private Renderer myRenderer;
        private Color emissionColor;

        void Awake()
        {
            damageable = GetComponent<Damageable>();
            damageable.onDestroy.AddListener(GiveResource);
            damageable.onHit.AddListener(HitResource);

            myRenderer = GetComponent<Renderer>();
            if (myRenderer)
                emissionColor = myRenderer.material.GetColor("_EmissionColor");
        }

        void GiveResource()
        {
            BuildingManager.instance.AddResource(resourceType, amount);
        }

        void HitResource()
        {
            //visual
            transform.DOComplete();
            transform.DOShakeScale(.5f, .2f, 10, 90, true);
        }

        private void OnMouseEnter()
        {
            isHover = true;
            if (myRenderer)
                myRenderer.material.SetColor("_EmissionColor", Color.grey);
        }
        private void OnMouseExit()
        {
            isHover = false;
            if (myRenderer)
                myRenderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }
}
