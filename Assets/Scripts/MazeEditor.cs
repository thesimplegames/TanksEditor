using UnityEngine;
using System.Collections;

public class MazeEditor : MonoBehaviour {
	
	
	static EditorController.BrushStyle[,] map = new 
					EditorController.BrushStyle [EditorController.mapWidth,EditorController.mapHeight];
	static int[] line1,line2;
	static int length = EditorController.mapWidth;
	static int now=0;
	static System.IO.StreamWriter file;
	static void AddLine(){
		string de1= "",de2="";
		for (int i=0; i<length;i++){
			de1+=line1[i].ToString()+" ";
			de2+=line2[i].ToString()+" ";
			switch (line1[i]) {
			case -1:
				map[i,now]=EditorController.BrushStyle.Wall;
			break;
			default:
				map[i,now]=EditorController.BrushStyle.None;
			break;
			}
			switch (line2[i]) {
			case -1:
				map[i,now+1]=EditorController.BrushStyle.Wall;
			break;
			default:
				map[i,now+1]=EditorController.BrushStyle.None;
			break;
			}
		}
		file.WriteLine(de1);
		file.WriteLine(de2);
	
		now+=2;
	}
	
	public static EditorController.BrushStyle[,] Randomize(){
		
		 file = new System.IO.StreamWriter("_.txt");
		line1 = new int [length];
		line2 = new int [length];
		//Add first line
		for (int i=0; i<length;i+=2){
			line1[i]=i;
			line2[i]=-10;
			if (i+1<length) line2[i+1] = -1;
			if (i+1<length) line1[i+1]=-10;
		}
		for (int i=0; i<length-2;i+=2){
			int toDel=line1[i+2];
			 if (line1[i]!=line1[i+2]) if (Random.value<=0.6f) line1[i+1]=-1; 
				else {
					for (int q = 0; q<length;q+=2) if (line1[q]==toDel) line1[q]=line1[i];
				}
			
		}
		
		for (int i=0; i<length;i+=2){
			int count=0;
			for (int q = 0;q<length;q+=2) if (line1[i]==line1[q]) count++;
			for (int q = 0;q<length;q+=2) if (line1[i]==line1[q]) if (count>1){
				if (Random.value<0.6f) {
					line2[q]=-1;
					count--;
				}
			}
		}
		AddLine();
		// Add lines
		while (now+4<EditorController.mapHeight){
			
			for (int i=0; i<length;i++){
				if (line1[i]==-1) line1[i]=-10;
				if (i%2==0) if (line2[i]==-1) {
					line1[i]=-10;
					line2[i]=-10;
				}
			}
			for (int i=0; i<length;i+=2){
				if (line1[i]==-10) line1[i]=i+length*now;
			}
			
			for (int i=0; i<length-2;i+=2){
				int toDel=line1[i+2];
				 if (line1[i]!=line1[i+2]) if (Random.value<=0.6f) line1[i+1]=-1; 
					else {
						for (int q = 0; q<length;q+=2) if (line1[q]==toDel) line1[q]=line1[i];
					}
				
			}
			
			for (int i=0; i<length;i+=2){
				int count=0;
				for (int q = 0;q<length;q+=2) if (line1[i]==line1[q]) if (line2[q]!=-1) count++;
				for (int q = 0;q<length;q+=2) if (line1[i]==line1[q]) if (count>1){
					if (Random.value<0.6f) {
						line2[q]=-1;
						count--;
					}
				}
			}
			AddLine();
		}
			
		//Add last line
			for (int i=0; i<length-2;i+=2){
				line2[i]=-1;
				int toDel=line1[i+2];
				 if (line1[i]!=line1[i+2]) {
						line1[i+1]=-10; 
						for (int q = 0; q<length;q+=2) if (line1[q]==toDel) line1[q]=line1[i];
					}
				
			}
			
			AddLine();
		
		
		file.Close();
		
		return map;
	}
	
	void Start () {
		
	}
}
