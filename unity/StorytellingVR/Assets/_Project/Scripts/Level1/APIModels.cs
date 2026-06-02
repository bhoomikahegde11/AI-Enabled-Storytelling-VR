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
    public TransactionSummary transaction;
}

[System.Serializable]
public class StepRequest
{
    public string session_id;
    public string player_input;
}