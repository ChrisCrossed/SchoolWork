using UnityEngine;
using System.Collections;

class GridPosition
{
    public int x { get; set; }
    public int y { get; set; }
    public GridQuadrant gridQuadrant { get; set; }

    public void Initialize(int x_, int y_, GridQuadrant gridQuadrant_)
    {
        x = x_;
        y = y_;
        gridQuadrant = gridQuadrant_;
    }    
}

public enum GridObjectState
{
    Off,
    On,
    Active
}

public class Cs_GridObjectLogic : MonoBehaviour
{
    // GridPosition testGridObject = new GridPosition { x = 0, y = 0, gridQuadrant = GridPosition.GridQuadrant.Center };
    GameObject go_CurrentGameObject;

    // State of the GridObject
    bool b_IsEnabled;
    GridObjectState gridObjectState;
    PurchaseObjects gridObjectType;

    // Prototype information. Remove later.
    int i_CurrTestPos;

    public GridObjectState Get_GridObjectState()
    {
        return gridObjectState;
    }

    public PurchaseObjects Get_GridObjectType()
    {
        return gridObjectType;
    }

    public void Set_GridObjectType(PurchaseObjects purchaseObjects_)
    {
        gridObjectType = purchaseObjects_;

        ToggleGameObjects(0, gridObjectType);
    }

    public void KillTower()
    {
        // Kills the tower
        ToggleGameObjects(8);
    }

    // Changes through the colors of the walls when clicked on
    public void ToggleGameObjects(int i_NewTowerPos_ = -1, PurchaseObjects purchaseObjects_ = PurchaseObjects.Wall)
    {
        if (i_NewTowerPos_ != -1) i_CurrTestPos = i_NewTowerPos_;

        // No Game Object, Instantiate it
        if(i_CurrTestPos == 0)
        {
            gridObjectState = GridObjectState.Active;

            GameObject.Find("GridObject List").GetComponent<Cs_GridLogic>().IncrementNumberOfTowers();

            if(purchaseObjects_ == PurchaseObjects.Wall) go_CurrentGameObject = Instantiate(Resources.Load("GO_Wall")) as GameObject;
            else if(purchaseObjects_ == PurchaseObjects.Tree) go_CurrentGameObject = Instantiate(Resources.Load("GO_Tree")) as GameObject;
            else if(purchaseObjects_ == PurchaseObjects.Bush) go_CurrentGameObject = Instantiate(Resources.Load("GO_Bush")) as GameObject;
            else if (purchaseObjects_ == PurchaseObjects.Halfwall) go_CurrentGameObject = Instantiate(Resources.Load("GO_Halfwall")) as GameObject;
            else if (purchaseObjects_ == PurchaseObjects.Halfwall_90) go_CurrentGameObject = Instantiate(Resources.Load("GO_Halfwall_90")) as GameObject;
            else if (purchaseObjects_ == PurchaseObjects.Corner) go_CurrentGameObject = Instantiate(Resources.Load("GO_Corner")) as GameObject;


            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().Initialize(4, 10, gameObject);

            Vector3 newPos = gameObject.transform.position;
            newPos.y = 0.5f;
            go_CurrentGameObject.transform.position = newPos;
        }
        // Run through three colors on the tower (Blue)
        else if(i_CurrTestPos == 1)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(-1);

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.Blue);
        }

        // Run through three colors on the tower (Red)
        else if (i_CurrTestPos == 2)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(-1);

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.Red);
        }

        // Run through three colors on the tower (Green)
        else if (i_CurrTestPos == 3)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(-1);

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.Green);
        }

        // Run through three colors on the tower (Yellow)
        else if (i_CurrTestPos == 4)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(-1);

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.Yellow);
        }

        // Run through three colors on the tower (Purple)
        else if (i_CurrTestPos == 5)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(-1);

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.Purple);
        }

        // Run through three colors on the tower (Green)
        else if (i_CurrTestPos == 6)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().ApplyDamage(-1);

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.Orange);
        }

        // Turn Tower Semi-transparent
        else if (i_CurrTestPos == 7)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX();

            go_CurrentGameObject.GetComponent<Cs_WallTowerLogic>().SetNewMaterialColor(Colors.SemiTransparent);
        }

        // Destroy the Tower
        else if (i_CurrTestPos == 8)
        {
            GameObject.Find("Main Camera").GetComponent<Cs_CameraLogic>().PlaySFX(false);

            gridObjectState = GridObjectState.On;
            
            GameObject.Find("GridObject List").GetComponent<Cs_GridLogic>().IncrementNumberOfTowers(false);

            // Destroy tower
            GameObject.Destroy(go_CurrentGameObject);
        }

        if (i_CurrTestPos < 8) ++i_CurrTestPos;

        // Set/Reset Counter
        if(i_CurrTestPos > 8) i_CurrTestPos = 0;
    }

    public void SetGridObjectState(bool b_IsEnabled_)
    {
        b_IsEnabled = b_IsEnabled_;

        // Set the Mouse Collider
        gameObject.GetComponent<BoxCollider>().enabled = b_IsEnabled;

        // Set the Mesh Renderer
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = b_IsEnabled;

        // Set the state of the GridObject
        if(b_IsEnabled_) gridObjectState = GridObjectState.On; else gridObjectState = GridObjectState.Off;
    }

    // Use this for initialization
    void Start ()
    {
        // gridObjectState = GridObjectState.Off;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
