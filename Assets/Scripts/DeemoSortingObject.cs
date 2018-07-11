using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeemoSortingObject : MonoBehaviour
{

    public bool isAllowCustom = false;
    public int sortIndex = 0;

    private int curIndex = 0;
    private bool isChild = false;
    private Renderer r = null;

    void Start()
    {
        if (r == null)
            r = this.GetComponent<Renderer>();
        if (r != null)
        {
            r.sortingOrder = sortIndex;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (isAllowCustom)
        {
            SetSortIndex(sortIndex);
        }
        else
        {
            SetSortIndex(curIndex);
        }

        int count = this.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            Transform tran = this.transform.GetChild(i);
            DeemoSortingObject sort = tran.GetComponent<DeemoSortingObject>();
            if (sort == null)
                sort = tran.gameObject.AddComponent<DeemoSortingObject>();

            sort.isChild = true;
            if (sort.isAllowCustom)
            {
                sort.sortIndex = sort.sortIndex;
                sort.curIndex = sort.sortIndex;
            }
            else
            {
                sort.sortIndex = this.sortIndex;
                sort.curIndex = this.sortIndex;
            }
            sort.OnValidate();
        }


    }

    void SetSortIndex(int n_index)
    {
        if (r == null)
            r = this.GetComponent<Renderer>();
        if (r != null)
        {
            this.curIndex = n_index;
            this.sortIndex = n_index;
            r.sortingOrder = n_index;
        }
    }

#endif

}
