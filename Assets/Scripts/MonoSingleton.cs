using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T _instance;

	public static bool HasInstance => _instance != null;

	public static T Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = (T)Object.FindFirstObjectByType(typeof(T));
				if(_instance == null)
				{
					_instance = new GameObject(typeof(MonoSingleton<T>).ToString(), typeof(T)).GetComponent<T>();
				}
			}

			return _instance;
		}
	}

	public virtual void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
