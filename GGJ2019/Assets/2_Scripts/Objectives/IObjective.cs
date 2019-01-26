using System;

public interface IObjective
{
	int Type { get; set; }
	NetworkPlayer Player { get; set; }
	Action<IObjective> OnComplete { get; set; }
}
