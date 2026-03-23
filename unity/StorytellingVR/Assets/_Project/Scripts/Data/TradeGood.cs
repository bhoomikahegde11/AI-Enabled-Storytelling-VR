namespace Data
{
    [System.Serializable]
    public class TradeGood
    {
        public string id;
        public string name;
        public string unit;
        public string category;
        public float cost_price_per_unit;
        public float price_variance_percent;
        public string description_for_ai;
        // Currency used for this good. e.g. "varaha", "fanam", "kasu"
        // If blank, defaults to "varaha"
        public string currency;

        public string GetCurrency()
        {
            return string.IsNullOrEmpty(currency) ? "varaha" : currency;
        }
    }

    [System.Serializable]
    public class TradeGoodList
    {
        public TradeGood[] items;
    }
}