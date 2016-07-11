using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Creates a tile snapped to grid at mouse position, undo will delete that tile
/// </summary>
public class AddTileAction : GridAction {

	GameObject obj;
	Vector3 mousePos;
	float width;
	float height;
	GameObject prefab;
	bool redo = false;
	Event copy;


	public override void InitAction(Event e) {
		if (!redo) {
			prefab = TileHelper.SelectedTile;

			if (prefab == null || !prefab.GetComponent<Tile>()) 
				return;
			Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
			mousePos = r.origin;
			this.width = TileHelper.Width;
			this.height = TileHelper.Height;
			mousePos.x = Mathf.FloorToInt(mousePos.x/width)*width + width/2f;
			mousePos.y = Mathf.FloorToInt(mousePos.y/height)*height + height/2f;
			copy = new Event(e);
		}
			
//		obj = MonoBehaviour.Instantiate(prefab) as GameObject;
		obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
		obj.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
		obj.GetComponent<Tile>().AddedBy = this;

		DoAction();
	}

	public void ReinitAction() {
		redo = true;
		InitAction(copy);
		redo = false;
	}

	public override void ForceRemove() {
		if (obj != null)
			obj.GetComponent<Tile>().AddedBy = null;
		TileHelper._instance.ForceRemove(this);
	}

	public override void DoAction () {
		TileHelper._instance.AddAction(this, redo);
	}

	public override void UndoAction () {
		MonoBehaviour.DestroyImmediate(obj);
	}

	public override string ActionToString (){
		if (obj != null)
			return "Create Tile: " + obj.name;
		else return "Create Tile: Null";
	}

	public void FocusObject() {
		Selection.activeObject = obj;
	}
}
