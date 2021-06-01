using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _horizontal;

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_horizontal.GetComponent<RectTransform>());
    }
}
