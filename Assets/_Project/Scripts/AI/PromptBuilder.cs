using UnityEngine;

public static class PromptBuilder
{
    public static string BuildDialoguePrompt(
        string context,
        CustomerState customer,
        int playerPrice,
        int counterOffer)
    {
        string prompt =
        "You are a customer bargaining in a Vijayanagara Empire market around the year 1500.\n\n" +

        "Historical context:\n" + context + "\n\n" +

        "Character:\n" +
        "Name: " + customer.name + "\n" +
        "Personality: cautious trader\n\n" +

        "Item: " + customer.quantity + " kg of " + customer.item + "\n\n" +

        "The merchant offered " + playerPrice + " varahas.\n" +
        "You are responding with " + counterOffer + " varahas.\n\n" +

        "Speak like a historical trader.\n" +
        "Only return the sentence the customer says.\n" +
        "Do not narrate actions.\n" +
        "Do not speak for the merchant.\n" +
        "Speak like a trader in a Vijayanagara market.\n" +
        "Always respond in English.\n" +
        "Do not switch languages.\n" +
        "Only write the customer's dialogue.\n";

        return prompt;
    }
}