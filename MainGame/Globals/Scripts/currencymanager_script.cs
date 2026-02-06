using Godot;
using System;
using System.Collections.Generic;

// List of all currencies
public enum CURRENCIES {
	COIN, // Numerical ID: 0
	BODY, // Numerical ID: 1
	METAL // Numerical ID: 2
}
public partial class currencymanager_script : Node
{

	// Dictionary to map currency ids -> names of currencies
	private static readonly Dictionary<CURRENCIES, string> currencyNames = new(){
		{CURRENCIES.COIN, "Coin"},
		{CURRENCIES.BODY, "Body"},
		{CURRENCIES.METAL, "Metal"}
	};

	// Struct to hold all data for each currency
	private struct Currency {
		// Currency is stored as an int, change this if we need
		// decimals or values > ~ 2 billion
		public int value = 0; // Amount stored in this currency

		public int id; // Internal id of currency (see enum above)

		public string name; // String name of currency

		public Currency(CURRENCIES id) {
			this.id = (int)id;
			this.name = currencyNames[id];
		}
	}

	// Array to hold all currencies
	private Currency[] currencies;

	[Signal]
	public delegate void AddCurrencyEventHandler(int id, int amount);

	[Signal]
	public delegate void RemoveCurrencyEventHandler(int id, int amount);

	/*
	* Dynamically creates each currency defined in enum and dictionary above
	* all currencies default at 0
	*/
	public currencymanager_script() {

		var currencyIDS = Enum.GetValues(typeof(CURRENCIES));
		currencies = new Currency[currencyIDS.Length];


		foreach(CURRENCIES c in currencyIDS){
			currencies[(int)c] = new Currency(c);
			GD.Print("Created new currency with id " + c + " and name " + currencies[(int)c].name);

		}
	}

	/*
	* Add amount to specified currency, returns false if this fails
	* for any reason (out of range)
	*/
	public bool add_currency(int id, int amount) {
		if (id > currencies.Length || id < 0) {
			return false;
		}
		currencies[id].value += amount;
		EmitSignal(SignalName.AddCurrency, id, amount);
		return true;
	}

	/*
	* Remove amount from specified currency, returns false if this fails
	* for any reason (not enough currency or out of range)
	*/
	public bool remove_currency(int id, int amount) {
		if (id > currencies.Length || id < 0) {
			return false;
		}
		if (currencies[id].value < amount) {
			return false;
		}
		currencies[id].value -= amount;
		EmitSignal(SignalName.RemoveCurrency, id, amount);
		return true;
	}

	/*
	* Get the balance of currency with id 'id'
	* Returns -1 if id is out of range
	*/
	public int get_currency_balance(int id) {
		if (id > currencies.Length || id < 0) {
			return -1;
		}
		return currencies[id].value;
	}

	/*
	* Get the name of currency with id 'id'
	* Returns NULL if id is out of range
	*/
	public string get_currency_name(int id) {
		if (id > currencies.Length || id < 0) {
			return null;
		}
		return currencies[id].name;
	}
}
