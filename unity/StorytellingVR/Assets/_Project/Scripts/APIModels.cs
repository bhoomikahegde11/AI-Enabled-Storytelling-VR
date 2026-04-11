[System.Serializable]
public class StartResponse
{
    public string session_id;
    public string npc_text;
    public string action;
    public int price;
    public int quantity;
    public bool done;
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
}

[System.Serializable]
public class StepRequest
{
    public string session_id;
    public string player_input;
}