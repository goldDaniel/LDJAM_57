using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Game : MonoBehaviour
{
	public static Game Instance;

	public PlayerController player;

	public Texture2D cursorTexture;

	public Level level;

	public List<PerkTemplate> currentPerkPool;

	public List<PerkTemplate> madnessPerkPool;

	public List<Pickup> pickups;

	
	public Wave currentWave;
	private float _waveTimer;
	private int _currentWaveCount = 0;

	public List<Wave> allWaves; // sorted by #. 0 == first wave, 1 == second wave, etc...
	Bounds levelBounds = new();

	public Cultist cultistPrefab;
	public BrainCrab brainCrabPrefab;
	public Eyeball eyeballPrefab;

	public CanvasRenderer waveCompleteText;

	public CanvasRenderer loseUI;

	public bool IsPaused
	{
		get => !player.enabled;
		set => player.enabled = !value;
	}

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Debug.LogError("Cannot instantiate multiple games. Something is wrong!");
			Destroy(this.gameObject);
		}
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);

		levelBounds.min = new Vector3(-level.arenaWidth / 2 + level.wallWidth, -level.arenaHeight / 2 + level.wallWidth, 0);
		levelBounds.max = new Vector3(level.arenaWidth / 2 - level.wallWidth, level.arenaHeight / 2 - level.wallWidth, 0);

		currentWave = SelectNextWave();
	}

	void Update()
	{
		if (IsPaused)
			Pause();
		else
			UnPause();

		HandleWaveUpdates();
	}

	public void Win()
	{
		StartCoroutine(EndInternal(true));
	}

	public void Lose()
	{
		StartCoroutine(EndInternal(false));
	}

	private IEnumerator EndInternal(bool win)
	{
		loseUI.gameObject.SetActive(true);
		loseUI.transform.Find("Lose Text").GetComponent<TextMeshProUGUI>().text = win ? "You Win" : "You Lose";
		loseUI.transform.Find("Score Text").GetComponent<TextMeshProUGUI>().text = $"Final Score: {player.score}";
		loseUI.SetAlpha(0);
		float timer = 0;
		float time = 2f;

		while(timer < time)
		{
			float t = timer / time;
			loseUI.SetAlpha(t);
			timer += Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(5f);
		SceneTransitions.Instance.LoadScene("MainMenu", SceneTransition.FadeOut);
		AudioManager.Instance.SwitchToMainMenu();
	}

	void HandleWaveUpdates()
	{
		if (currentWave == null)
			return;

		if (currentWave.WaveComplete() && Enemy.instances.Count == 0)
		{
			// select next wave, or game complete
			currentWave = null;
			StartCoroutine(SetupNextWave());
		}
		else
		{
			_waveTimer += Time.deltaTime;
			while (_waveTimer >= currentWave.spawnInterval)
			{
				_waveTimer -= currentWave.spawnInterval;

				Vector2 spawnPosition = PickSpawnLocation();

				bool picked = currentWave.WaveComplete();
				while (!picked)
				{
					int type = Random.Range(0, 2);
					if(currentWave.eyeballCount > 0)
					{
                        picked = true;
                        currentWave.eyeballCount--;
                        var eye = Instantiate(eyeballPrefab);
                        eye.transform.position = spawnPosition;
                        eye.Health *= currentWave.healthMulti;
                        eye.hitDamage *= currentWave.damageMulti;
                        eye.xpGain *= currentWave.xpMulti;
                        eye.attackDamage *= currentWave.damageMulti;
                    } else {
						switch (type)
						{
							case 0: // brain crab
								if (currentWave.brainWaveCount > 0)
								{
									picked = true;
									currentWave.brainWaveCount--;
									for (int i = 0; i < currentWave.brainPackCount; ++i)
									{
										var crab = Instantiate(brainCrabPrefab);
										crab.transform.position = spawnPosition + Random.insideUnitCircle * 5f;
										crab.Health *= currentWave.healthMulti;
										crab.hitDamage *= currentWave.damageMulti;
										crab.xpGain *= currentWave.xpMulti;
									}
								}
								break;
							case 1: // cultist
								if (currentWave.cultistCount > 0)
								{
									picked = true;
									currentWave.cultistCount--;
									var cultist = Instantiate(cultistPrefab);
									cultist.transform.position = spawnPosition;
									cultist.Health *= currentWave.healthMulti;
									cultist.hitDamage *= currentWave.damageMulti;
									cultist.xpGain *= currentWave.xpMulti;
								}
								break;
						}
					}
				}

			}
		}
	}

	Wave SelectNextWave()
	{
		if(allWaves.Count > 0)
		{
			var result = Instantiate(allWaves[0]);
			allWaves.RemoveAt(0);
			_currentWaveCount++;
			return result;
		}

		return null;
	}

	private IEnumerator SetupNextWave()
	{
		var text = waveCompleteText.GetComponent<TextMeshProUGUI>();
		text.text = $"WAVE {_currentWaveCount} COMPLETE";

		waveCompleteText.SetAlpha(0);
		waveCompleteText.gameObject.SetActive(true);
		float t = 0;
		while(t < 1)
		{
			t += Time.deltaTime;
			t = Mathf.Clamp01(t);
			waveCompleteText.SetAlpha(t);
			yield return null;
		}
		while (t > 0)
		{
			t -= Time.deltaTime;
			t = Mathf.Clamp01(t);
			waveCompleteText.SetAlpha(t);
			yield return null;
		}

		var nextWave = SelectNextWave();
		if (nextWave == null)
		{
			yield return EndInternal(true);
		}
		else 
		{
			text.text = $"WAVE {_currentWaveCount} START";
			t = 0;
			while (t < 1)
			{
				t += Time.deltaTime;
				t = Mathf.Clamp01(t);
				waveCompleteText.SetAlpha(t);
				yield return null;
			}

			t = 1;
			while (t > 0)
			{
				t -= Time.deltaTime;
				t = Mathf.Clamp01(t);
				waveCompleteText.SetAlpha(t);
				yield return null;
			}

			currentWave = nextWave;
			waveCompleteText.gameObject.SetActive(false);
		}
	}

	private Vector2 PickSpawnLocation(float distanceFromCamera = 5)
	{
		float distanceOutsideCameraWS = 5;

		float halfHeight = Camera.main.orthographicSize;

		float halfWidth = halfHeight * Camera.main.aspect;
		float top = Camera.main.transform.position.y + halfHeight + distanceFromCamera;
		float bottom = Camera.main.transform.position.y - halfHeight - distanceFromCamera;
		float right = Camera.main.transform.position.x + halfWidth + distanceFromCamera;
		float left = Camera.main.transform.position.x - halfWidth - distanceFromCamera;

		bool horizontal = Random.value > 0.5f;
		if (horizontal && left <= levelBounds.min.x + distanceOutsideCameraWS)
			horizontal = false;
		else if (!horizontal && right >= levelBounds.max.x - distanceOutsideCameraWS)
			horizontal = true;

		float posX;
		if(horizontal)  // left side
			posX = Random.Range(left - distanceOutsideCameraWS, left);
		else // right side
			posX = Random.Range(right, right + distanceOutsideCameraWS);

		bool vertical = Random.value > 0.5f;
		if (vertical && bottom <= levelBounds.min.y + distanceOutsideCameraWS)
			vertical = false;
		else if (!vertical && top >= levelBounds.max.y - distanceOutsideCameraWS)
			vertical = true;

		float posY;
		if (vertical)  // bottom side
			posY = Random.Range(bottom - distanceOutsideCameraWS, bottom);
		else // top side
			posY = Random.Range(top, top + distanceOutsideCameraWS);

		return new Vector2(posX, posY);
	}

	private void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public IEnumerable<Enemy> GetNearbyEnemies(Vector2 position, float radius)
	{
		List<Enemy> result = new();
        foreach (var item in Physics2D.OverlapCircleAll(position, radius, LayerMask.NameToLayer("Enemy")))
        {
			if(item.TryGetComponent(out Enemy enemy))
				result.Add(enemy);
        }
		return result;
	}

	public List<PerkTemplate> SelectPerks(int count)
	{
		List<PerkTemplate> result = new(count);
		

		for(int i = 0; i < count; ++i)
		{
			float madnessRoll = Random.Range(0f,1f);

			List<PerkTemplate> perkPool;

			if(madnessRoll > Game.Instance.player.madness * 0.75f || Game.Instance.player.madness < 0.01f)
			{
				perkPool = currentPerkPool;
			} else
			{
				perkPool = madnessPerkPool;
			}

            int totalWeights = 0;
            int currentWeight;
            for (int j = 0; j < perkPool.Count; ++j)
            {
                totalWeights += 100 + perkPool[j].rollWeight;
            }

            int roll;
			int index = 0;
			do
			{
                roll = Random.Range(0, totalWeights);
				currentWeight = roll;
				for(int j = 0;j < perkPool.Count;++j)
				{
					currentWeight -= 100 + perkPool[j].rollWeight;
					if(currentWeight <= 0)
					{
						index = j;
						break;
					}
				}
            }
			while (result.Contains(perkPool[index]) || perkPool[index].madnessThreshold > Game.Instance.player.madness);
			result.Add(perkPool[index]);
		}

		return result;
	}

	public void RemovePerkFromPool(PerkTemplate perk)
	{
		List<PerkTemplate> perkPool;
		if (perk.madnessThreshold > 0f)
		{
			perkPool = madnessPerkPool;
		} else
		{
			perkPool = currentPerkPool;
		}

        perkPool.RemoveSwapBack(perk);
		if (perk.nextTier != null)
            perkPool.AddRange(perk.nextTier);
	}
	
	private void Pause()
	{
		foreach (var enemy in Enemy.instances)
			enemy.enabled = false;

		foreach (var fireball in Fireball.instances)
			fireball.enabled = false;

		foreach (var explosion in Explosion.instances)
			explosion.enabled = false;

		foreach (var attack in CultistAttack.instances)
			attack.enabled = false;
	}

	private void UnPause()
	{
		foreach (var enemy in Enemy.instances)
			enemy.enabled = true;

		foreach (var fireball in Fireball.instances)
			fireball.enabled = true;

		foreach (var explosion in Explosion.instances)
			explosion.enabled = true;

		foreach (var attack in CultistAttack.instances)
			attack.enabled = true;
	}
}
