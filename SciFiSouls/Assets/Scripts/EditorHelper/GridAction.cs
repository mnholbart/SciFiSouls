using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class GridAction {
	
	public abstract void InitAction(Event e);
	public abstract void ForceRemove();
	public abstract void DoAction();
	public abstract void UndoAction();

	public abstract string ActionToString();
}
