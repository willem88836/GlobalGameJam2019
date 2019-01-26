using System;

public interface IObjective
{
	int Type { get; set; }
	int Player { get; set; }
	Action<IObjective> OnComplete { get; set; }
}
