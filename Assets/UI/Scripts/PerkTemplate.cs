using UnityEngine;
using System.Collections.Generic;

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
	public float maxHealthMultiplier = 0; // implemented
	public float maxHealthAdditive = 0; // implemented

	public float damage = 0; // implemented
	public float damageMultiplier = 0; // implemented
	public float aoeDamage = 0;
	public float aoeRadius = 0;
	public float movementSpeed = 0; // implemented
	public float manaRegen = 0; // implemented
	public float healthRegen = 0; // implemented
	public float maxManaAdditive = 0; // implemented
	public float damageReduction = 0;
	public float castSpeed = 0; // implemented
	public float manaCost = 0; // implemented
	public float homingSpeed = 0;
	public float lifesteal = 0;
	public float cascadeChance = 0;
	public float XPIncrease = 0;
	public float shieldCooldown = 0;
	public float madnessGained = 0;
	public float instakill = 0;
	public int perksGained = 0;
	public float retaliate = 0;

    public bool equality = false;
    public bool shield = false;
	public bool rampage = false;
	public bool panic = false;

	public float madnessThreshold = 0;
	public float rollWeight = 0;


    public List<PerkTemplate> nextTier;
}
