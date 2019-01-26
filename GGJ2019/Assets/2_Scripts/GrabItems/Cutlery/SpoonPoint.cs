public class SpoonPoint : ToolPoint<ISpoonable>
{
	protected override void Invoke(ISpoonable obj)
	{
		obj.OnSpoon();
	}
}
