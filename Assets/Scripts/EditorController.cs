using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditorController : MonoBehaviour {
	
	private bool editorMode = false;	//	true - we edit, false - settings on start
	private int mapWidth = 15;
	private int mapHeight = 15;
	private int minSize = 10;
	private int maxSize = 30;
	private int numberEnemies=0;
	private BrushStyle[,] map;
	private enum BrushStyle {None = 0, Wall = 1, Water = 2, Grass = 3, Arol = 4, Enemy = 5, Tank1 = 6, Tank2 = 7, SuperWall = 8};
	private BrushStyle brush = BrushStyle.None;
	private GameObject elementPrefab;
	private List<GameObject> elements = new List<GameObject>();
	
	
	// Use this for initialization
	void Start () {
		elementPrefab = Resources.Load("Prefabs/Element") as GameObject;
	}
	
	bool MouseOnFrame (int x, int y) {
		return x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1;
	}
	
	string MapToString () {
		string strMap = numberEnemies.ToString()+ ":" + mapWidth.ToString() + ":" + mapHeight.ToString() + ":";
		/*
		for (int i = 0; i <= mapWidth; i--) {
			for (int j = 0; j < mapHeight; j++) {
				strMap += ((int)map[i, j]).ToString();
			}
		}*/
		
		
		
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapHeight; j++) {
				strMap += ((int)map[i, mapHeight - j - 1]).ToString();
			}
		}
		
		Debug.Log(strMap);
		return(strMap);
	}
	/*
	BrushStyle[,] LoadFromFile(string path) {
		string strMap;
		System.IO.StreamReader file = new System.IO.StreamReader(path);
	    strMap = file.ReadLine();
		file.Close();	
		//int[,] map;
		
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
				map[i, j] = (BrushStyle)System.Convert.ToInt32(strMap[mapPosition].ToString());
				mapPosition++;
			}
		}	
		return map;
	}	
	*/
	
	int[,] LoadFromFile(string path) {
		string strMap;
		System.IO.StreamReader file = new System.IO.StreamReader(path);
	    strMap = file.ReadLine();
		file.Close();	
		int[,] map;
		
		int mWidth = 0;
		int mHeight = 0;
		int wh = 0;
		string sInt = "";
		int mapPosition = 0;
		
		//	Loading map size
		for (int i = 0; i < strMap.Length; i++) {
			if (strMap[i] == ':') {
				if (wh==0) {
					wh = 1;
					numberEnemies = System.Convert.ToInt32(sInt);
					sInt = "";
					continue;
				}
				if (wh==1) {
					wh = 2;
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
		
		map = new int[mWidth, mHeight];
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapWidth; j++) {
				map[i, j] = System.Convert.ToInt32(strMap[mapPosition].ToString());
				mapPosition++;
			}
		}	
		return map;
	}

	void SaveToFile(string path) {
		string strMap = MapToString();
		System.IO.StreamWriter file = new System.IO.StreamWriter(path);
		file.WriteLine(strMap);
		file.Close();
	}	
	
	// Update is called once per frame
	void Update () {
		if (editorMode) {
			Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if (Input.GetMouseButton(0)) {
				if (Physics.Raycast(ray, out hit) && !MouseOnFrame((int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1)) {
					Debug.Log(hit.transform.position);
					hit.transform.renderer.material.mainTexture = Resources.Load("Png/" + brush.ToString()) as Texture;
					map[(int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1] = brush;
				}
			}
			
			if (Input.GetMouseButton(1)) {
				if (Physics.Raycast(ray, out hit) && !MouseOnFrame((int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1)) {
					Debug.Log(hit.transform.position);
					hit.transform.renderer.material.mainTexture = Resources.Load("Png/None") as Texture;
					map[(int)hit.transform.position.x, mapHeight - (int)hit.transform.position.y - 1] = BrushStyle.None;
				}
			}			
		}
	}
	
	void DestroyElements () {
		for (int i = 0; i < elements.Count; i++) {
			Destroy(elements[i]);
		}
	}
	
	void StartEdit () {
		Camera.mainCamera.transform.position = new Vector3(mapWidth / 2, mapHeight / 2, Camera.mainCamera.transform.position.z);
		Camera.mainCamera.orthographicSize = 2 + (mapWidth > mapHeight ? (mapWidth / 2 + 1) : (mapHeight / 2 + 1));
		map = new BrushStyle[mapWidth, mapHeight];
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapHeight; j++) {
				GameObject newElement = Instantiate(elementPrefab) as GameObject;
				newElement.transform.position = new Vector3(i, j, 0);
				
				if (i == 0 || j == 0 || i == mapWidth - 1 || j == mapHeight - 1) {
					map[i, j] = BrushStyle.SuperWall;
					newElement.renderer.material.mainTexture = Resources.Load("Png/SuperWall") as Texture;
				} else {
					map[i, j] = BrushStyle.None;
					newElement.renderer.material.mainTexture = Resources.Load("Png/None") as Texture;
				}
				
				newElement.transform.position = new Vector3(i, j, 0);
				newElement.name = "Element_x" + i.ToString() + "y" + j.ToString();
				elements.Add(newElement);
			}
		}
	}
	
	void OnGUI () {
		if (!editorMode) {
			GUI.Button(new Rect(Screen.width / 2 - 100, 100, 200, 50), "Set map settings");

			GUI.Button(new Rect(Screen.width / 2 - 100, 160, 100, 50), "Width: " + mapWidth.ToString());
			if (GUI.Button(new Rect(Screen.width / 2,160, 50, 50), "-")) 
				if (mapWidth > minSize) 
					mapWidth--;
			if (GUI.Button(new Rect(Screen.width / 2 + 50, 160, 50, 50), "+")) 
				if (mapWidth < maxSize) 
					mapWidth++;

			GUI.Button(new Rect(Screen.width / 2 - 100, 220, 100, 50), "Height: " + mapHeight.ToString());
			if (GUI.Button(new Rect(Screen.width / 2, 220, 50, 50), "-")) 
				if (mapHeight > minSize) 
					mapHeight--;
			if (GUI.Button(new Rect(Screen.width / 2 + 50, 220, 50, 50), "+")) 
				if (mapHeight < maxSize) 
					mapHeight++;

			if (GUI.Button(new Rect(Screen.width / 2 -100, 280, 200, 50), "Start editor")) {
				editorMode = true;
				StartEdit();
			}
			
		} else {
			if (GUI.Button(new Rect(0, 0, 100, 50), "New map")) {
				DestroyElements();
				editorMode = false;
			}
			if (GUI.Button(new Rect(0, 100, 100, 50), "Save map")) {
				SaveToFile("map_" + ((int)(Time.time)).ToString() + ".map");
			}     
			
			if (GUI.Button(new Rect(0, 200, 100, 50), "Load map")) {
				LoadFromFile("1.map");
			}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
//{None = 0, Wall = 1, Water = 2, Grass = 3, Arol = 4, Enemy = 5, Tank1 = 6, Tank2 = 7, SuperWall = 8};			
			if (GUI.Button(new Rect(150, 0, 60, 50), "None")) 
				brush = BrushStyle.None;
			if (GUI.Button(new Rect(210, 0, 60, 50), "Wall")) 
				brush = BrushStyle.Wall;
			if (GUI.Button(new Rect(270, 0, 60, 50), "Tank1")) 
				brush = BrushStyle.Tank1;
			if (GUI.Button(new Rect(330, 0, 60, 50), "Tank2")) 
				brush = BrushStyle.Tank2;
			if (GUI.Button(new Rect(390, 0, 60, 50), "Enemy")) 
				brush = BrushStyle.Enemy;
			if (GUI.Button(new Rect(450, 0, 60, 50), "Grass")) 
				brush = BrushStyle.Grass;
			if (GUI.Button(new Rect(510, 0, 60, 50), "Water")) 
				brush = BrushStyle.Water;
			if (GUI.Button(new Rect(570, 0, 60, 50), "Arel")) 
				brush = BrushStyle.Arol;			
			if (GUI.Button(new Rect(630, 0, 60, 50), "Rock")) 
				brush = BrushStyle.SuperWall;
			
			GUI.Button(new Rect(720, 0, 220, 50), "Number of enemies: "+numberEnemies.ToString()); 
			if (GUI.Button(new Rect(940, 0, 50, 50), "-")) {
				if (numberEnemies>0) numberEnemies--;
			}
			if (GUI.Button(new Rect(990, 0, 50, 50), "+")) {
				if (numberEnemies<1828) numberEnemies++;
			}
		}
	}
}
