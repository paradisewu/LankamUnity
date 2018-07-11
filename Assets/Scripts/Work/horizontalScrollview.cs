
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class horizontalScrollview : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //将子物体拉到中心位置时的速度
    public float centerSpeed = 9f;

    //注册该事件获取当拖动结束时位于中心位置的子物体
    public delegate void OnCenterHandler(GameObject centerChild);
    public event OnCenterHandler onCenter;

    private ScrollRect _scrollView;
    private Transform _container;

    public List<float> _childrenPos = new List<float>();
    private float _targetPos;
    public bool _centering = false;
    public int currentIndex = 0;
    public GameObject RightButton;
    public GameObject LeftButton;

    private void Start()
    {
        RightButton = transform.Find("RightButton").gameObject;
        LeftButton = transform.Find("LeftButton").gameObject;
    }
    public void Init(int childCount)
    {
        _scrollView = GetComponent<ScrollRect>();
        if (_scrollView == null)
        {
            Debug.LogError("CenterOnChild: No ScrollRect");
            return;
        }
        _container = _scrollView.content;


        GridLayoutGroup grid;
        grid = _container.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            Debug.LogError("CenterOnChild: No GridLayoutGroup on the ScrollRect's content");
            return;
        }

        _scrollView.movementType = ScrollRect.MovementType.Clamped;
        _childrenPos.Clear();
        //计算第一个子物体位于中心时的位置
        float childPosX = _scrollView.GetComponent<RectTransform>().rect.width * 0.5f - grid.cellSize.x * 0.5f;
        _childrenPos.Add(childPosX);
        //缓存所有子物体位于中心时的位置
        for (int i = 0; i < childCount - 1/*_container.childCount - 1*/; i++)
        {
            childPosX -= grid.cellSize.x + grid.spacing.x;
            _childrenPos.Add(childPosX);
        }


        _centering = true;
        _targetPos = FindClosestPos(_childrenPos[0], out currentIndex);
    }

    void Update()
    {
        if (_centering)
        {
            Vector3 v = _container.localPosition;
            v.x = Mathf.Lerp(_container.localPosition.x, _targetPos, centerSpeed * Time.deltaTime);
            _container.localPosition = v;
            if (Mathf.Abs(_container.localPosition.x - _targetPos) < 0.01f)
            {
                _centering = false;
            }
        }
    }

    public void ClickToNext()
    {
        _centering = true;
        _targetPos = FindClosestPos(_childrenPos[currentIndex + 1], out currentIndex);
    }

    public void ClickToPrevious()
    {
        _centering = true;
        _targetPos = FindClosestPos(_childrenPos[currentIndex - 1], out currentIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _centering = true;
        _targetPos = FindClosestPos(_container.localPosition.x, out currentIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _centering = false;
    }

    private float FindClosestPos(float currentPos, out int index)
    {
        int childIndex = 0;
        float closest = 0;
        float distance = Mathf.Infinity;

        for (int i = 0; i < _childrenPos.Count; i++)
        {
            float p = _childrenPos[i];
            float d = Mathf.Abs(p - currentPos);
            if (d < distance)
            {
                distance = d;
                closest = p;
                childIndex = i;
            }
        }

        GameObject centerChild = _container.GetChild(childIndex).gameObject;
        if (onCenter != null)
            onCenter(centerChild);
        index = childIndex;

        if (index == _childrenPos.Count - 1)
        {
            RightButton.SetActive(false);
            if (_childrenPos.Count > 1)
            {
                LeftButton.SetActive(true);
            }
            else
            {
                LeftButton.SetActive(false);
            }
        }
        else if (index == 0)
        {
            LeftButton.SetActive(false);
            if (_childrenPos.Count > 1)
            {
                RightButton.SetActive(true);
            }
            else
            {
                RightButton.SetActive(false);
            }
        }
        else
        {
            RightButton.SetActive(true);
            LeftButton.SetActive(true);
        }
        return closest;
    }
}

