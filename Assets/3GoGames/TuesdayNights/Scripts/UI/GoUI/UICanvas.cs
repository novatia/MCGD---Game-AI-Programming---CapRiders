using UnityEngine;
using System.Collections;

namespace GoUI
{

public class UICanvas : MonoBehaviour 
{
	void Start () 
    {
	
	}
	
	void Update () 
    {
        for (int child1Index = 0; child1Index < transform.childCount - 1; ++child1Index)
        {
            for (int child2Index = child1Index + 1; child2Index < transform.childCount; ++child2Index)
            {
                Transform child1 = transform.GetChild(child1Index);
                Transform child2 = transform.GetChild(child2Index);

                if (child1.position.z < child2.position.z)
                {
                    child1.SetSiblingIndex(child2Index);
                    child2.SetSiblingIndex(child1Index);
                }
            }
        }
	}
}

}