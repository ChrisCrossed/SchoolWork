using UnityEngine;
using System.Collections;

public class Cs_GridLogic : MonoBehaviour
{
    int i_sizeOfGrid = 9; // STARTING size of grid
    Object[] gridList;

    bool b_Test = true;

    int i_CurrNumTowers;
    int i_TowerLevel = 3; // This is an int claming the number of towers per row/column.

    // Use this for initialization
    void Start()
    {
        gridList = new GameObject[i_sizeOfGrid * i_sizeOfGrid];

        PreloadGridObjects();
    }

    void PreloadGridObjects()
    {
        int midPoint = i_sizeOfGrid / 2;

        for (int y = 0; y < i_sizeOfGrid; ++y)
        {
            for (int x = 0; x < i_sizeOfGrid; ++x)
            {
                Vector3 pos = new Vector3(x - (i_sizeOfGrid / 2), 0, y - (i_sizeOfGrid / 2));
                Quaternion quat = new Quaternion();

                GameObject temp = Instantiate(Resources.Load("GridObject", typeof(GameObject)), pos, quat) as GameObject;

                // Only activate the middle points
                int i_Size = 2;
                if (y >= (midPoint - i_Size) && y <= (midPoint + i_Size) && x >= (midPoint - i_Size) && x <= (midPoint + i_Size))
                {
                    temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
                }
                else
                {
                    temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(false);
                }

                gridList[(y * i_sizeOfGrid) + x] = temp;
            }
        }
    }

    public void IncrementNumberOfTowers(bool b_Incremented = true)
    {
        // If the function call claims a tower was added, increment the towers. Otherwise, this is meant to decrement it.
        if (b_Incremented) ++i_CurrNumTowers; else --i_CurrNumTowers;

        // i_CurrLevel is a misnomer and represents
        if((float)i_CurrNumTowers / (float)(i_TowerLevel * i_TowerLevel) > 0.75f)
        {
            IncrementWidthOfTowerArray();
        }
    }

    void IncrementWidthOfTowerArray()
    {
        i_TowerLevel += 2; // Brings the tower level to 5x5, 7x7, etc...

        #region Tower Levels
        if(i_TowerLevel == 5)
        {
            // Top Row
            int y_ = 2;

            #region Top Row
            int x_ = 2;
            GameObject temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Bot row
            y_ = 6;

            #region Bot Row
            x_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Left Wall
            x_ = 2;

            #region Left Wall
            y_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            x_ = 6;

            #region Right Wall
            y_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion
        }
        else if(i_TowerLevel == 7)
        {
            // Top Row
            int y_ = 1;

            #region Top Row
            int x_ = 1;
            GameObject temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 7;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Bot row
            y_ = 7;

            #region Bot Row
            x_ = 1;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 7;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Left Wall
            x_ = 1;

            #region Left Wall
            y_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Right Wall
            x_ = 7;

            #region Right Wall
            y_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion
        }
        else if (i_TowerLevel == 9)
        {
            // Top Row
            int y_ = 0;

            #region Top Row
            int x_ = 0;
            GameObject temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 1;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 7;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 8;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Bot row
            y_ = 8;

            #region Bot Row
            x_ = 0;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 1;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 7;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            x_ = 8;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Left Wall
            x_ = 0;

            #region Left Wall
            y_ = 1;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 7;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion

            // Right Wall
            x_ = 8;

            #region Right Wall
            y_ = 1;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 2;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 3;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 4;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 5;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 6;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);

            y_ = 7;
            temp = gridList[(y_ * i_sizeOfGrid) + x_] as GameObject;
            temp.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(true);
            #endregion
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            b_Test = !b_Test;

            for (int i = 0; i < gridList.Length; ++i)
            {
                var tower = gridList[i] as GameObject;

                tower.GetComponent<Cs_GridObjectLogic>().SetGridObjectState(b_Test);
            }
        }
        #endif

    }
}
