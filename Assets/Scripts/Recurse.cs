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
    public bool dontRemoveZoomZones = false;
    public string recursePointName = "RecursePoint";
    public bool recurseNow = false;
    public string sortingLayer;
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

        GameObject instantiatedObject = PrefabUtility.InstantiatePrefab(recursePrefab, recurseParent) as GameObject;
        
        // Remove zoom zones from children of the initial prefab
        if (totalRecursions > 0 && dontRemoveZoomZones == false)
        {
            foreach (ZoomZone zoomZone in instantiatedObject.GetComponentsInChildren<ZoomZone>())
            {
                DestroyImmediate(zoomZone);
            }

            foreach (BoxCollider boxCollider in instantiatedObject.GetComponentsInChildren<BoxCollider>())
            {
                DestroyImmediate(boxCollider);
            }

            foreach (Rigidbody rigidbody in instantiatedObject.GetComponentsInChildren<Rigidbody>())
            {
                DestroyImmediate(rigidbody);
            }
        }

        totalRecursions += 1;

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

        foreach (SpriteRenderer spriteRenderer in instantiatedObject.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.sortingOrder = spriteRenderer.sortingOrder + (recurseIterations - currentIteration);
            if (sortingLayer != "")
            {
                spriteRenderer.sortingLayerName = sortingLayer;
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
