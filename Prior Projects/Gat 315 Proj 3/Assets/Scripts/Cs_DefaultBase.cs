using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridQuadrant
{
    Center,
    Top,
    Bottom,
    Left,
    Right
}

public enum Colors
{
    Default,
    Blue,
    Red,
    Green,
    Purple,
    Yellow,
    Orange,
    SemiTransparent
}

public class Cs_DefaultBase : MonoBehaviour
{
    int i_Health;
    int i_Health_Max;
    bool b_IsDead;
    Material mat_Color; // Color is the object color (Red, Blue, Green, etc...)
    Material mat_Color_Base; // Base is the black backdrop
    float f_FireTimer;
    float f_FireTimer_Max;
    BoxCollider col_BaseCollider;
    CapsuleCollider col_RadiusCollider;
    List<GameObject> EnemyList;
    GameObject go_GridObject_Parent;

    public Material testMat;

    // Just a test comment. Means nothing
    int element_Color = -1; // Color is the object color (Red, Blue, Green, etc...)
    int element_Base = -1; // Base is the black backdrop

    virtual public void Initialize(int i_Health_Max_, float f_FireTimer_Max_, GameObject go_ParentBoxCollider_, BoxCollider col_BaseCollider_ = null, CapsuleCollider col_RadiusCollider_ = null)
    {
        i_Health = i_Health_Max_;
        i_Health_Max = i_Health_Max_;
        go_GridObject_Parent = go_ParentBoxCollider_;

        b_IsDead = false;

        // Material newMat = Resources.Load("DEV_Orange", typeof(Material)) as Material;
        SetMaterialElements();
        SetNewMaterialColor(Colors.Default);

        f_FireTimer = 0;
        f_FireTimer_Max = f_FireTimer_Max_;

        if (col_BaseCollider_ != null) col_BaseCollider = col_BaseCollider_; else
        {
            if (gameObject.transform.Find("Col_BaseCollider")) col_BaseCollider = gameObject.transform.Find("Col_BaseCollider").GetComponent<BoxCollider>();
        }

        if(col_RadiusCollider_ != null) col_RadiusCollider = col_RadiusCollider_; else
        {
            if (gameObject.transform.Find("Col_Radius")) col_RadiusCollider = gameObject.transform.Find("Col_Radius").GetComponent<CapsuleCollider>();
        }

        EnemyList = new List<GameObject>();
    }

    // Gathers the default element positions for the Material backdrops
    void SetMaterialElements()
    {
        Material matColor = Resources.Load("Color_Base", typeof(Material)) as Material; // (Red, Blue, etc...)
        Material matColorBase = Resources.Load("Mat_BASE", typeof(Material)) as Material; // Black Backdrop
        var tempMatList = gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterials;
        
        for (int i = 0; i < tempMatList.Length; ++i)
        {
            // renderer.sharedMaterial
            if      (tempMatList[i] == matColor)        element_Color = i;
            else if (tempMatList[i] == matColorBase)    element_Base = i;
        }
    }

    virtual public void SetHealth(int i_Health_, int i_Health_Max_ = -1)
    {
        i_Health = i_Health_;

        if (i_Health_Max_ != -1) i_Health_Max = i_Health_Max_;
    }

    virtual public void ApplyDamage(int i_Damage)
    {
        i_Health -= i_Damage;

        if(i_Health <= 0)
        {
            // Destroy GameObject
            go_GridObject_Parent.GetComponent<Cs_GridObjectLogic>().KillTower();

            go_GridObject_Parent.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
        }
    }

    public void SetNewMaterialColor(Colors newColor_)
    {
        if (newColor_ == Colors.Default) mat_Color = Resources.Load("Color_Base", typeof(Material)) as Material; // Black
        else if (newColor_ == Colors.Red) mat_Color = Resources.Load("Mat_RED", typeof(Material)) as Material;
        else if (newColor_ == Colors.Blue) mat_Color = Resources.Load("Mat_BLUE", typeof(Material)) as Material;
        else if (newColor_ == Colors.Green) mat_Color = Resources.Load("Mat_GREEN", typeof(Material)) as Material;
        else if (newColor_ == Colors.Purple) mat_Color = Resources.Load("Mat_PURPLE", typeof(Material)) as Material;
        else if (newColor_ == Colors.Yellow) mat_Color = Resources.Load("Mat_YELLOW", typeof(Material)) as Material;
        else if (newColor_ == Colors.Orange) mat_Color = Resources.Load("Mat_ORANGE", typeof(Material)) as Material;
        else if (newColor_ == Colors.SemiTransparent) mat_Color = Resources.Load("Mat_TRANSPARENT", typeof(Material)) as Material;

        if(newColor_ != Colors.SemiTransparent)
        {
            mat_Color_Base = Resources.Load("Mat_BASE", typeof(Material)) as Material;

            var tempMatList = gameObject.GetComponentInChildren<MeshRenderer>().materials;
            tempMatList[element_Base] = mat_Color_Base;
            tempMatList[element_Color] = mat_Color;
            gameObject.GetComponentInChildren<MeshRenderer>().materials = tempMatList;
        }
        else // Turn semi-transparent for a short while
        {
            var tempTransList = gameObject.GetComponentInChildren<MeshRenderer>().materials;
            for(int i = 0; i < tempTransList.Length; ++i)
            {
                // tempTransList[i] = mat_Color;
                var tempMatList = gameObject.GetComponentInChildren<MeshRenderer>().materials;
                tempTransList[element_Base] = Resources.Load("Color_Base", typeof(Material)) as Material;
                tempTransList[element_Color] = mat_Color;
            }
            gameObject.GetComponentInChildren<MeshRenderer>().materials = tempTransList;
        }
    }
}
