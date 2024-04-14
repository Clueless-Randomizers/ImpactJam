using DG.Tweening;
using UnityEngine;

namespace Prototype.Scripts
{
    public class ActorVisualHandler : MonoBehaviour
    {
        private Actor actor;
        public SpriteRenderer selectedSprite;
        public GameObject destroyParticle;

        private void Start()
        {
            actor = GetComponent<Actor>();
            //actor.animationEvent.attackEvent.AddListener(Attack);
        }
        public void Select()
        {
            selectedSprite.transform.DOScale(0, .2f).From().SetEase(Ease.OutBack);
            selectedSprite.enabled = true;
        }

        public void Deselect()
        {
            selectedSprite.enabled = false;
        }

        void Attack()
        {
            if(actor.damageableTarget)
                Instantiate(destroyParticle, actor.damageableTarget.transform.position, Quaternion.identity);
        }

        private void OnDestroy()
        {
            if (destroyParticle && Application.isPlaying)
                Instantiate(destroyParticle, transform.position, Quaternion.identity);
        }
    }
}
