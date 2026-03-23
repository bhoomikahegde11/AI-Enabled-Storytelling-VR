using System;

namespace Data
{
    [Serializable]
    public class CustomerPersonality
    {
        public string id;
        public string display_name;
        public string profession;
        public int patience;
        public float desperation;
        public float price_knowledge;
        public float opening_offer_ratio;
        public float concession_step_percent;
        public float walkaway_aggression;
        public string tone_prompt_tag;
    }

    [Serializable]
    public class CustomerPersonalityList
    {
        public CustomerPersonality[] items;
    }
}
