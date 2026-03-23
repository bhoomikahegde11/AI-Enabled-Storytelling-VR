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
    }

    [System.Serializable]
    public class TradeGoodList
    {
        public TradeGood[] items;
    }
}