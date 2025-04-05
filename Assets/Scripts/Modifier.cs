
using System;
using System.Collections.Generic;

[Serializable]
public class Modifier
{
	public int add = 0;
	public int mul = 1;
}

[Serializable]
public class Modifiable
{
	public float baseValue;

	List<Modifier> modifiers = new();

	public void Apply(Modifier mod) => modifiers.Add(mod);

	public void Remove(Modifier mod) => modifiers.Remove(mod);

	public float Base() => baseValue;

	public float Modified()
	{
		float result = baseValue;

		foreach (var mul in modifiers)
			result *= mul.mul;

		foreach (var add in modifiers)
			result += add.add;

		return result;
	}
}