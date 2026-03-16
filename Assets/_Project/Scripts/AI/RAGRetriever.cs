using UnityEngine;

public class RAGRetriever : MonoBehaviour
{
    string knowledge;

    void Awake()
    {
        TextAsset data = Resources.Load<TextAsset>("vijayanagar_knowledge");
        knowledge = data.text;
    }

    public string RetrieveContext(string item)
    {
        string[] lines = knowledge.Split('\n');
        string result = "";

        foreach(string line in lines)
        {
            if(line.ToLower().Contains(item.ToLower()))
            {
                result += line + "\n";
            }
        }

        if(result == "")
            result = knowledge.Substring(0, Mathf.Min(300, knowledge.Length));

        return result;
    }
}