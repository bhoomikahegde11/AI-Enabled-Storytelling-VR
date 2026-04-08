public class CustomerState
{
    public string id; // personality id for reference
    public string name;
    public string item;
    public int quantity;

    // Pricing and negotiation values (all amounts are in Varahas)
    public float fairTotalPrice; // selected_good.cost_price_per_unit * quantity * 1.3f
    public float customerMaxAccept;
    public float currentCustomerOffer;

    public int patience;
    public int effectivePatience;

    public int lastOffer;

    public int roundCount;

    public Data.TradeGood good;
    public Data.CustomerPersonality personality;
}