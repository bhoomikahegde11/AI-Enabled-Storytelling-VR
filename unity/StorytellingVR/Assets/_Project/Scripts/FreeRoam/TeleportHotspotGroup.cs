using Oculus.Interaction.Locomotion;
using UnityEngine;

[System.Serializable]
public class TeleportHotspotGroup
{
    public string groupName;

    [Tooltip("All teleport hotspots belonging to this story group.")]
    public TeleportInteractable[] hotspots;
}