using UnityEngine;

namespace Assets._2_Scripts
{
	public class Foo : MonoBehaviour
	{
		public Observer observer;
		public ObjectiveManager objectiveManager;

		private void Start()
		{
			for (int i = 0; i < 4; i++)
			{
				objectiveManager.AddPlayer(i);
			}
		}
	}
}
