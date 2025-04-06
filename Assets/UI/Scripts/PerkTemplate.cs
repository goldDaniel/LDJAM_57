using UnityEngine;

[CreateAssetMenu(fileName = "PerkTemplate", menuName = "Perk")]
public class PerkTemplate : ScriptableObject
{
	public Sprite icon;
	public string title;
	[Multiline]
	public string description;
	public Color borderColor;
	public bool showDecoration;


	// 10% increase = 1.1f, 22% = 1.22f, etc...
	public float maxHealthMultiplier;
	public float maxHealthAdditive;

	public PerkTemplate nextTier;
}
