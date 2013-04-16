using UnityEngine;
using System.Collections;

public class LabrintEditor : MonoBehaviour {
	
	
	private EditorController.BrushStyle[,] map = new 
					EditorController.BrushStyle [EditorController.mapWidth,EditorController.mapHeight];
	private int[] line1,line2;
	private int length = EditorController.mapWidth;
	private int now=0;
	private void AddLine(){
		for (int i=0; i<length;i++){
			switch (line1[i]) {
			case 1:
				map[i,now]=EditorController.BrushStyle.Wall;
			break;
			default:
				map[i,now]=EditorController.BrushStyle.None;
			break;
			}
			switch (line2[i]) {
			case 1:
				map[i,now+1]=EditorController.BrushStyle.Wall;
			break;
			default:
				map[i,now+1]=EditorController.BrushStyle.None;
			break;
			}
		}
		now+=2;
	}
	void Start () {
		line1 = new int [length];
		line2 = new int [length];
		for (int i=0; i<length;i+=2){
			line[i]=i;
			if (i+1<length) line[i+1]=-10;
		}
		for (int i=0; i<length-2;i+=2){
			int toDel=line[i+1];
			 if (line[i]!=line[i+1]) if (Random.value<=0.5f) line[i+1]=-1; 
				else {
					for (int q = 0; q<length;q+=2) if (line[q]==toDel) line[q]=line[i];
				}
			
		}
		AddLine();
	}
}
