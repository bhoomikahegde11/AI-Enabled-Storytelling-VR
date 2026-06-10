[System.Serializable]
public class CurrentTrade
{
    public string spice;
    public string quantity;
    public int npc_offer;
    public int market_value;
}

[System.Serializable]
public class StartResponse
{
    public string session_id;
    public string npc_text;
    public string action;
    public int price;
    public int quantity;
    public bool done;
    public string audio_url;
    public int reputation;
    public int total_varahas;
    public int reputation_delta;
    
    // Backend-driven HUD data
    public int player_reputation;
    public int player_money;
    public string buyer_name;
    public string buyer_origin;
    public string spice_name;
    public string spice_quantity;
    public CurrentTrade current_trade;
}

[System.Serializable]
public class TransactionSummary
{
    public string item;
    public string quantity;
    public int earned;
    public int profit;
    public int respect_change;
    public string buyer_name;
    public string buyer_origin;
}

[System.Serializable]
public class StepResponse
{
    public string session_id;
    public string npc_text;
    public string action;
    public int price;
    public int quantity;
    public bool done;
    public string audio_url;
    public int reputation;
    public int total_varahas;
    public int reputation_delta;
    public TransactionSummary transaction;

    // Backend-driven HUD data
    public int player_reputation;
    public int player_money;
    public string buyer_name;
    public string buyer_origin;
    public string spice_name;
    public string spice_quantity;
    public CurrentTrade current_trade;
}

[System.Serializable]
public class StepRequest
{
    public string session_id;
    public string player_input;
}