using System;
using System.Collections.Generic;
using _Scripts.ScriptableObjects;
using UnityEngine;

namespace _Scripts
{
	public class CurrencyManager : MonoBehaviour
	{
		[SerializeField] private SO_Currency[] _currencies;
		[SerializeField] private GameObject _currencyPrefab;
		private Dictionary<string, SO_Currency> _currencyRegister = new();

		// Start is called before the first frame update
		private void Awake()
		{
			// Register resource Manager so others can use it easily.
			GameManager.CurrencyManager = this;

			// This for-loop should hook up all the currency updating and stuff.
			for ( int i = 0; i < _currencies.Length; i++ ) {
				SO_Currency currency = _currencies[ i ];
			
				SO_Currency _instantiatedCurrency = Instantiate( currency, null );
				GameObject _newCurrencyUI = Instantiate( _currencyPrefab, transform );

				_currencies[ i ] = _instantiatedCurrency;

				string _currencyKey = _instantiatedCurrency.PresentableName;
				_currencyRegister.Add( _currencyKey, _instantiatedCurrency );

				if ( _newCurrencyUI.TryGetComponent( out CurrencyDisplayManager currencyDisplayController ) ) {
					currencyDisplayController.SetCurrency( _instantiatedCurrency );
				}
			}
		}
		/// <summary>
		/// Returns all currencies registered.
		/// </summary>
		/// <returns></returns>
		public SO_Currency[] GetCurrencies () {
			return _currencies;
		}
		/// <summary>
		/// Returns currency by name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SO_Currency GetCurrency (string name) {
			return (_currencyRegister.ContainsKey( name ) ) ? _currencyRegister[ name ] :default;
		}
		/// <summary>
		/// Checks if there are enough resources to buy the asked goods.
		/// </summary>
		/// <param name="prices"></param>
		/// <returns></returns>
		public bool CanAffordPurchase ( PurchasePrice[] prices) {
			foreach ( PurchasePrice price in prices) {
				SO_Currency _currency = GetCurrency(price.Currency.name);
				if (_currency.Value < price.Value) {
					Debug.Log($"Not enough of resource {_currency.name}  have {_currency.Value}, need: {price.Value}");
					return false;
				}
			}

			return true;
		}
		/// <summary>
		/// Deducts purchase prices from available resources.
		/// </summary>
		/// <param name="purchasePrices"></param>
		public void DeductPurchase ( PurchasePrice[] purchasePrices ) {
			foreach (PurchasePrice price in purchasePrices) {
				SO_Currency _currency = GetCurrency( price.Currency.name );

				// the -1 * price.Value makes the positive value into a negative one.
				_currency.Value = -1 * price.Value;
			}
		}
	}
	/// <summary>
	/// This struct is used when storing Purchase Prices in Arrays and Lists
	/// </summary>
	[Serializable]
	public struct PurchasePrice {
		public int Value;
		public SO_Currency Currency;
	}
}
