public class KnifePoint : ToolPoint<ISliceable>
{
	protected override void Invoke(ISliceable obj)
	{
		obj.OnSlice();
	}
}
