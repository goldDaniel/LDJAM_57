
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Wave", menuName = "Wave")]
public class Wave : ScriptableObject
{
	public float spawnInterval;

	public int brainPackCount; // # of brains in a pack
	
	public int brainWaveCount; // # of brains in a wave
	public int cultistCount;
	public int eyeballCount;
	public int bossCount;
	public float healthMulti;
	public float damageMulti;
	public float xpMulti;

	public bool WaveComplete()
	{
		bool result = true;
		result &= brainWaveCount == 0;
		result &= cultistCount == 0;
		result &= eyeballCount == 0;
		result &= bossCount == 0;
		return result;
	}
}
