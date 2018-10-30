using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Keith.Towers;
using UnityEngine.UI;
using System;

[CreateAssetMenu]
public class Tower : ScriptableObject {

    [SerializeField] string id;
    public string ID { get { return id; } }
    public string TowerName;
    public TowerStats Damage;
    public TowerStats FireRate;
    public TowerStats Range;
    public TowerStats PowerCooldown;
    public TowerStats ProcChance;
    public LayerMask targetableLayers;
    public GameObject prefab;
    public TargetType targettype;
    public GameObject TargetTower = null;
    public GameObject AestheticMesh;
    public MonoBehaviour TowerBehavior;
    public Sprite preview;

    [TextAreaAttribute(15,20)]
    public string Description;

    private void OnValidate() {
        
        // string path = AssetDatabase.GetAssetPath(this);
        id = Guid.NewGuid().ToString(); //AssetDatabase.AssetPathToGUID(path);
    }

    public enum TargetType
    {
        Closest,
        Lowest,
        Furthest,
        Around,
        Multi,
    }
}
