using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Attributes;
using UnityEngine;
using UnityEngine.Serialization;
public class UIAttributeUpdater : MonoBehaviour 
{
    [SerializeField] private AttributeSet attributeSet = default;
    [SerializeField] private string attributeType = default;
    [SerializeField] private string maxAttributeType = default;
    [SerializeField] private float lerpSpeed = default;

    [SerializeField] private RectTransform attributeBar = default;

    private Attribute attribute;
    private Attribute maxAttribute;
    private float maxWidth;

    void Start() 
    {
        attribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == attributeType);
        maxAttribute = attributeSet.Attributes.FirstOrDefault(x => x.AttributeType == maxAttributeType);
        maxWidth = attributeBar.rect.width;
    }

    void Update() 
    {
        Rect rect = attributeBar.rect;
        float width = (attribute.CurrentValue / maxAttribute.CurrentValue) * maxWidth;
        float lerpedWidth = Mathf.Lerp(rect.width, width, Time.deltaTime * lerpSpeed);
        lerpedWidth = Mathf.Min(lerpedWidth, maxWidth);
        lerpedWidth = Mathf.Max(lerpedWidth, 0);
        attributeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lerpedWidth);
        attributeBar.ForceUpdateRectTransforms();
    }
}
