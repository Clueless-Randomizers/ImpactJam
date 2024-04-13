using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyDisplayManager : MonoBehaviour
{
	[SerializeField] private Image _currencySpriteElement;
	[SerializeField] private TMP_Text _currencyValueElement;
	[SerializeField] private TMP_Text _currencyNameElement;
	private SO_Currency _currency;
	
	public void SetCurrency( SO_Currency currency) {
		_currency = currency;
		_currency.CurrencyChange.AddListener(UpdateCurrencyText);

		_currencyValueElement.text = $"{_currency.Value}";
		_currencyNameElement.text = $"{_currency.PresentableName}";
		_currencySpriteElement.sprite = _currency.Icon;
	}

	private void UpdateCurrencyText () {
		_currencyValueElement.text = $"{_currency.Value}";
	}
}
