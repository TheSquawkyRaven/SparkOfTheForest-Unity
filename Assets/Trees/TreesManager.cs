using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesManager : MonoBehaviour
{

    public GameObject FirePrefab;

    public void SetTreeOnFire(GameObject treeObj, int firePoints)
    {
        if (!treeObj.TryGetComponent(out Tree tree))
        {
            tree = treeObj.AddComponent<Tree>();
            GameObject fireObj = Instantiate(FirePrefab, tree.transform);
            tree.SetOnFire(fireObj, firePoints);
        }
        else
        {
            tree.AddFirePoints(firePoints);
        }
    }

}
