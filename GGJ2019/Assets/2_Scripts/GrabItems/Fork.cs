using UnityEngine;

public class Fork : GrabObject
{
	[HideInInspector] public bool Grabbed; // Mayhaps just replace this to GrabObject?

	public override void Grab()
	{
		base.Grab();

		Grabbed = true;
	}

	public override void Release(Vector3 velocity)
	{
		base.Release(velocity);

		Grabbed = false;
	}
}
