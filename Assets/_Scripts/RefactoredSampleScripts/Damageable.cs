using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.RefactoredSampleScripts
{
    public class Damageable : MonoBehaviour
    {
        [HideInInspector] public UnityEvent onDestroy = new UnityEvent();
        [HideInInspector] public UnityEvent onHit = new UnityEvent();

        [SerializeField] int totalHealth = 100;
        public int CurrentHealth;
		public bool Indestructible = false;

        private void Start()
        {
            CurrentHealth = totalHealth;
        }

        public void Hit(int damage)
        {
			if (Indestructible) {
				return;
			}
            onHit.Invoke();
            CurrentHealth -= damage;
            if (CurrentHealth <= 0) {
				Destroy();
			}
        }
        void Destroy()
        {
            onDestroy.Invoke();
            Destroy(gameObject);
        }
    }
}
