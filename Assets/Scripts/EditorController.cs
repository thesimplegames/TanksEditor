using UnityEngine;
using System.Collections;

public class EditorController : MonoBehaviour {
	
	private bool editorMode = false;	//	true - we edit, false - settings on start
	private int mapWidth = 10;
	private int mapHeight = 10;
	private int min = 10;
	private int max = 30;
	private BrushStyle[,] map;
	private enum BrushStyle {None, SuperWall = 1, Wall, Tank1, Tank2, EnemyTank, Grass, Water, Arol};
	private BrushStyle brush = BrushStyle.None;
	private GameObject elementPrefab;
	
	
	// Use this for initialization
	void Start () {
		elementPrefab = Resources.Load("Prefabs/Element") as GameObject;
	}
	
	bool MouseOnFrame (int x, int y) {
		return x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1;
	}
	
	string MapToString () {
		string strMap = mapWidth.ToString() + ":" + mapHeight.ToString() + ":";
		
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapWidth; j++) {
				strMap += ((int)map[i, j]).ToString();
			}
		}
		Debug.Log(strMap);
		return(strMap);
	}
	
	void LoadFromFile(string path) {
		string strMap = System.IO.File.ReadAllLines(path, System.Text.Encoding.UTF8)[0];
		int mWidth = 0;
		int mHeight = 0;
		bool wh = true;
		string sInt = "";
		int mapPosition = 0;
		//	Loading map size
		for (int i = 0; i < strMap.Length; i++) {
			if (strMap[i] == ':') {
				if (wh) {
					wh = false;
					mWidth = System.Convert.ToInt32(sInt);
					sInt = "";
				} else {
					mHeight = System.Convert.ToInt32(sInt);
					mapPosition = i + 1;
					break;
				}
			} else {
				sInt += strMap[i];
			}			
		}
		
		map = new BrushStyle[mWidth, mHeight];
		
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapWidth; j++) {
				map[i, j] = (BrushStyle)System.Convert.ToInt32(strMap[mapPosition]);
				mapPosition++;
			}
		}		
	}

	void SaveToFile(string path) {
		string[] strMap = new string[] {MapToString()};
		System.IO.File.Create(path);
		System.IO.File.WriteAllLines(path, strMap, System.Text.Encoding.UTF8);
	}	
	
	// Update is called once per frame
	void Update () {
		if (editorMode) {
			Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Input.GetMouseButton(0)) {
				if (Physics.Raycast(ray, out hit) && !MouseOnFrame((int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1)) {
					Debug.Log(hit.transform.position);
					hit.transform.renderer.material.mainTexture = Resources.Load("Png/" + ((int)brush).ToString()) as Texture;
					map[(int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1] = brush;
				}
			}
			
			if (Input.GetMouseButton(1)) {
				if (Physics.Raycast(ray, out hit) && !MouseOnFrame((int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1)) {
					Debug.Log(hit.transform.position);
					hit.transform.renderer.material.mainTexture = Resources.Load("Png/0") as Texture;
					map[(int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1] = BrushStyle.None;
				}
			}			
		}
	}
	
	void StartEdit () {
		Camera.mainCamera.transform.position = new Vector3(mapWidth / 2, mapHeight / 2, Camera.mainCamera.transform.position.z);
		map = new BrushStyle[mapWidth, mapHeight];
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapHeight; j++) {
				GameObject newElement = Instantiate(elementPrefab) as GameObject;
				newElement.transform.position = new Vector3(i, j, 0);
				
				if (i == 0 || j == 0 || i == mapWidth - 1 || j == mapHeight - 1) {
					map[i, j] = BrushStyle.SuperWall;
					newElement.renderer.material.mainTexture = Resources.Load("Png/1") as Texture;
				} else {
					map[i, j] = BrushStyle.None;
					newElement.renderer.material.mainTexture = Resources.Load("Png/0") as Texture;
				}
				
				newElement.transform.position = new Vector3(i, j, 0);
				newElement.name = "Element_x" + i.ToString() + "y" + j.ToString();
			}
		}
	}
	
	void OnGUI () {
		if (!editorMode) {
			GUI.Button(new Rect(Screen.width / 2 - 100, 100, 200, 50), "Set map settings");

			GUI.Button(new Rect(Screen.width / 2 - 100, 160, 100, 50), "Width: " + mapWidth.ToString());
			if (GUI.Button(new Rect(Screen.width / 2,160, 50, 50), "-")) 
				if (mapWidth > min) 
					mapWidth--;
			if (GUI.Button(new Rect(Screen.width / 2 + 50, 160, 50, 50), "+")) 
				if (mapWidth < max) 
					mapWidth++;

			GUI.Button(new Rect(Screen.width / 2 - 100, 220, 100, 50), "Height: " + mapHeight.ToString());
			if (GUI.Button(new Rect(Screen.width / 2, 220, 50, 50), "-")) 
				if (mapHeight > min) 
					mapHeight--;
			if (GUI.Button(new Rect(Screen.width / 2 + 50, 220, 50, 50), "+")) 
				if (mapHeight < max) 
					mapHeight++;

			if (GUI.Button(new Rect(Screen.width / 2 -100, 280, 200, 50), "Start editor")) {
				editorMode = true;
				StartEdit();
			}
			
		} else {
			if (GUI.Button(new Rect(0, 0, 100, 50), "New map")) 
				editorMode = false;
			if (GUI.Button(new Rect(0, 100, 100, 50), "Save map")) {
				SaveToFile("map" + ((int)(Time.time)).ToString());
			}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
			
			if (GUI.Button(new Rect(150, 0, 60, 50), "None")) 
				brush = BrushStyle.None;
			if (GUI.Button(new Rect(210, 0, 60, 50), "Wall")) 
				brush = BrushStyle.Wall;
			if (GUI.Button(new Rect(270, 0, 60, 50), "Tank1")) 
				brush = BrushStyle.Tank1;
			if (GUI.Button(new Rect(330, 0, 60, 50), "Tank2")) 
				brush = BrushStyle.Tank2;
			if (GUI.Button(new Rect(390, 0, 60, 50), "Enemy")) 
				brush = BrushStyle.EnemyTank;
			if (GUI.Button(new Rect(450, 0, 60, 50), "Grass")) 
				brush = BrushStyle.Grass;
			if (GUI.Button(new Rect(510, 0, 60, 50), "Water")) 
				brush = BrushStyle.Water;
			if (GUI.Button(new Rect(570, 0, 60, 50), "Arel")) 
				brush = BrushStyle.Arol;			
			if (GUI.Button(new Rect(630, 0, 60, 50), "Rock")) 
				brush = BrushStyle.SuperWall;
		}
	}
}
