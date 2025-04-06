using UnityEngine;

[CreateAssetMenu(fileName = "PerkTemplate", menuName = "Scriptable Objects/Perk")]
public class PerkTemplate : ScriptableObject
{
	public Sprite icon;
	public string title;
	[Multiline]
	public string description;
	public Color borderColor;
	public bool showDecoration;
}
