using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.ScriptableObjects
{
    /// <summary>
    /// This ScriptableObject is the base for any currency in this game.
    /// </summary>
    [Serializable]
    [CreateAssetMenu( menuName = "ScriptableAssets/New Currency" )]
    public class SO_Currency : ScriptableObject
    {
        [SerializeField] int _currentCurrency = 0;
        [SerializeField] Sprite _currencyImage;
        /// <summary>
        /// Icon used for interface.
        /// </summary>
        public Sprite Icon { get { return _currencyImage; } }

        /// <summary>
        /// Subscribe to currency changes.
        /// </summary>
        public UnityEvent CurrencyChange;

        /// <summary>
        /// This variable always uses addition.
        /// </summary>
		
        public int Value {
            get { return _currentCurrency; }
            set {
                if (value != _currentCurrency) {
                    _currentCurrency = Mathf.Max(0, value);
                    CurrencyChange?.Invoke();
                    //Debug.Log($"Value of {name} set to {_currentCurrency}");
                }
            }
        }
		
        /// <summary>
        /// Returns string representation of name without (clone) 
        /// </summary>
        public string PresentableName { get { return name.Split( "(" )[ 0 ]; }  }
    }
}