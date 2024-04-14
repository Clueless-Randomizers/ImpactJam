using DG.Tweening;
using UnityEngine;

namespace _Scripts.RefactoredSampleScripts
{
    public class Resource : MonoBehaviour
    {
        [SerializeField] ResourceType resourceType;
        [SerializeField] int amount;
        Damageable damageable;
        public bool isHover;

        //HoverVisual
        private Renderer _renderer;
        private Color emissionColor;

        void Awake()
        {
            damageable = GetComponent<Damageable>();
            damageable.onDestroy.AddListener(GiveResource);
            damageable.onHit.AddListener(HitResource);

            _renderer = GetComponent<Renderer>();
            if (_renderer)
                emissionColor = _renderer.material.GetColor("_EmissionColor");
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
            if (_renderer)
                _renderer.material.SetColor("_EmissionColor", Color.grey);
        }
        private void OnMouseExit()
        {
            isHover = false;
            if (_renderer)
                _renderer.material.SetColor("_EmissionColor", emissionColor);
        }
    }
}
