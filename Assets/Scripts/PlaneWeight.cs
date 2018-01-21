using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWeight : MonoBehaviour {
	public int weight;
	public int type;
    
//	 <-type-> <-weight-> <-weight changed->
//	start : 0 	0   2
//	single: 1 	2
//	curve : 2 	2
//  T :     3 	3
//	allway: 4 	4
//	stuck : 5 	1   2
//	end :   6 	2   2

    public int getWeight()
    {
        return weight;
    }

    public void decreaseWeight()
    {
        weight--;
    }
}
