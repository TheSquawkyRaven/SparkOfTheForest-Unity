using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesManager : MonoBehaviour
{

    public GameObject FirePrefab;
    public GameObject FlamePrefab;
    public GameObject OrbPrefab;

    public Transform FlameContainer;
    public Transform OrbContainer;

    public Sprite AshSprite;
    public int FirePointsToMidBurn;
    public int FirePointsToFinalBurn;
    public int FirePointsToAsh;

    [System.NonSerialized] public List<Collider2D> TreeColliders = new();
    public ContactFilter2D TreeCF;

    public float SpreadFlameTime;

    //UPGRADES
    public float FirePointsIncreaseTime;
    public float FireSpreadChance;
    public float FireSpreadRadius;


    public void SetTreeOnFire(GameObject treeObj, int firePoints)
    {
        if (!treeObj.TryGetComponent(out Tree tree))
        {
            tree = treeObj.AddComponent<Tree>();
            AnimAdv fireAnimAdv = Instantiate(FirePrefab, tree.transform).GetComponent<AnimAdv>();
            tree.SetOnFire(this, fireAnimAdv, firePoints);
        }
        else
        {
            tree.AddFirePoints(firePoints);
        }
    }

    public void FireSpreadToTree(Tree source, Collider2D treeCL, int firePoints)
    {
        //use source to spread flame
        StartCoroutine(SetTreeOnFireDelayed(treeCL, firePoints, SpreadFlameTime));

        Flame flame = Instantiate(FlamePrefab, FlameContainer).GetComponent<Flame>();
        flame.SetFromTo(source.transform.localPosition, treeCL.transform.localPosition, SpreadFlameTime);
    }

    private IEnumerator SetTreeOnFireDelayed(Collider2D treeCL, int firePoints, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTreeOnFire(treeCL.gameObject, firePoints);
    }

    public void DropFireOrb(Tree tree)
    {
        Vector3 pos = tree.transform.localPosition;
        Instantiate(OrbPrefab, pos, Quaternion.identity, OrbContainer);
    }

}
