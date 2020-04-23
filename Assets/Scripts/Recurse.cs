using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Recurse : MonoBehaviour
{
    public GameObject recursePrefab;
    public int recurseIterations = 5;
    public string recursePointName = "RecursePoint";
    public bool recurseNow = false;
    private int totalRecursions = 0;
    private ZoomZone recurseZoomZone;

#if UNITY_EDITOR
    private void Update()
    {
        if (recurseNow == true)
        {
            recurseNow = false;

            if (!Application.isPlaying)
            {
                BeginRecursing();
            }
        }
    }

    private void BeginRecursing()
    {
        for (int child = transform.childCount; child > 0; --child)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        totalRecursions = 0;

        recurseZoomZone = null;

        RecurseAtPoint(recurseIterations, transform);

        Debug.Log("Total recursions: " + totalRecursions);
    }

    private void RecurseAtPoint(int currentIteration, Transform recurseParent)
    {
        if (totalRecursions > 5000)
        {
            Debug.Log("Total recursions greater than 1000, exited out");
        }

        if (currentIteration <= 0)
        {
            return;
        }

        totalRecursions += 1;

        GameObject instantiatedObject = PrefabUtility.InstantiatePrefab(recursePrefab, recurseParent) as GameObject;
        
        if (recurseZoomZone == null)
        {
            recurseZoomZone = instantiatedObject.GetComponent<ZoomZone>();
        }
        else
        {
            ZoomZone childZoomZone = instantiatedObject.GetComponent<ZoomZone>();

            if (childZoomZone != null)
            {
                childZoomZone.linkedZoomZone = recurseZoomZone;
                childZoomZone.priority = recurseZoomZone.priority + (recurseIterations - currentIteration);
            }
        }

        for (int child = 0; child < instantiatedObject.transform.childCount; child++)
        {
            Transform childTransform = instantiatedObject.transform.GetChild(child);

            if (childTransform.name.ToLower().Contains(recursePointName.ToLower()))
            {
                RecurseAtPoint(currentIteration - 1, childTransform);
            }
        }
    }
#endif
}
