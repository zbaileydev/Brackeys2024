using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tool", menuName = "Items/Tool")]
public class LootTool : Loot
{
    [System.Serializable]
    public struct ToolStats
    {
    }

    [HideInInspector]
    public new LootType Type { get => LootType.Tool; }
    public new string name = " ";
    public Sprite sprite;
    public ToolStats stats;
    public AnimationClip attackAnimation;
    public AnimationClip useAnimation;
}
