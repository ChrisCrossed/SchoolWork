using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public int startingPlayerCount = 1;
    public bool screenTest = false;

    public enum SplitScreenStyle { Vertical, Horizontal }
    public SplitScreenStyle splitScreenStyle = SplitScreenStyle.Horizontal;
    public bool splitHorizontally { get { return splitScreenStyle == SplitScreenStyle.Horizontal; } }

    public GameObject basePlayer;
    public Color[] playerColors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow};
    
    public Material primaryMaterial;
    public Material secondaryMaterial;

    public Color primaryColorOverride = new Color(1, 0, 1, 1);
    public Color secondaryOverride = new Color(1, 0, 1, 1);
    public Vector3 primaryColorOffset;
    public Vector3 secondaryColorOffset;
    public bool primaryOffsetUsesHSV = false;
    public bool secondaryOffsetUsesHSV = false;

    public static Player[] players = new Player[4];
    public static int playersInWorld;
    public AnimationCurve spawnLookValueCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve spawnDistanceValueCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public int maxSpawnValueCount;
    
    public float spawnDistanceInfluence;
    public float spawnAngleInfluence;
    public float spawnSightInfluence;

    private List<List<Rect>> rectGrid = new List<List<Rect>>();
    public static PlayerManager instance;
    public static Color[] PlayerColors { get { return instance.playerColors; } }

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        if(startingPlayerCount > playerColors.Length)
        {
            Color[] _newPlayerColors = new Color[startingPlayerCount];
            playerColors.CopyTo(_newPlayerColors, 0);
            playerColors = _newPlayerColors;
        }

        if (startingPlayerCount < 1) startingPlayerCount = 1;
        SetScreenDimensions(startingPlayerCount);

        for (int i = 0; i < startingPlayerCount; i++)
        {
            CreatePlayer(i);
        }
    }

    public void AddPlayer()
    {

    }

    public void RemovePlayer()
    {

    }

    public static void SpawnPlayer(int _index, float _delay = 0)
    {
        instance.StartCoroutine(instance.SpawnPlayerAfterDelay(_index, _delay));
    }

    private System.Collections.IEnumerator SpawnPlayerAfterDelay(int _index, float _delay)
    {
        if(_delay > 0)
            yield return new WaitForSeconds(_delay);

        if (!GameManager.instance.gameOver)
            CreatePlayer(_index);
    }
    private Player CreatePlayer(int _index)
    {
        ++PlayerManager.playersInWorld;

        Transform _spawnPoint = SpawnPoint.GetBest(_index).transform; //you need to come up with a way of doing teams when it becomes relevant (if it does)
        GameObject _playerObject = (GameObject)Instantiate(basePlayer, _spawnPoint.position, _spawnPoint.rotation);
        Player _player = _playerObject.GetComponent<Player>();
        _player.index = _index;
        _player.color = GetPlayerColor(_index);
        _player.team = _index % 2;

        players[_index] = _player;

        Camera[] _cameras = _player.GetComponentsInChildren<Camera>();
        for (int i = 0; i < _cameras.Length; i++)
            _cameras[i].rect = new Rect(GetScreenDimensions(_index));

        Events.PlayerSpawned.Send(_player);

        return _player;
    }

    Color GetPlayerColor(int _index)
    {
        if (_index < startingPlayerCount)
            return playerColors[_index];

        return Color.white;
    }
    
    void SetScreenDimensions(int _playerCount)
    {
        rectGrid.Add(new List<Rect>());

        int j = 1;
        for (int i = 0; i < _playerCount; i++)
        {
            if ((i == 1 && splitHorizontally) || rectGrid[0].Count > rectGrid.Count)
            {
                rectGrid.Add(new List<Rect>());

                if (i == 1)
                {
                    rectGrid[rectGrid.Count - j].Add(new Rect());
                    continue;
                }
                else
                {
                    for (int k = 0; k < rectGrid.Count - 1; k++)
                    {
                        rectGrid[k].RemoveAt(rectGrid[k].Count - 1);
                        rectGrid[rectGrid.Count - j].Add(new Rect());
                    }
                }
            }

            rectGrid[rectGrid.Count - j].Add(new Rect());

            if (j == rectGrid.Count)
                j = 0;

            ++j;
        }

        for (int _row = 0; _row < rectGrid.Count; _row++)
        {
            int _rows = rectGrid.Count;
            int _columns = rectGrid[_row].Count;

            for (int _column = 0; _column < rectGrid[_row].Count; _column++)
            {
                Rect _rect = rectGrid[_row][_column];

                _rect.width = 1f / _columns;
                _rect.height = 1f / _rows;
                _rect.x = _rect.width * _column;
                _rect.y = _rect.height * (_rows - _row - 1);

                rectGrid[_row][_column] = _rect;
            }
        }
    }

    Rect GetScreenDimensions(int _index)
    {
        if(screenTest)
        {
            if(_index == 0)
            {
                return new Rect(0, 0.5f, 0.65f, 0.5f);
            }
            else
            {
                return new Rect(0.35f, 0, 0.65f, 0.5f);
            }
        }

        int _total = 0;
        for (int _row = 0; _row < rectGrid.Count; _row++)
        {
            _total += rectGrid[_row].Count;
            if (_total > _index)
            {
                int _column = _index - (_total - rectGrid[_row].Count);
                return rectGrid[_row][_column];
            }
        }
        return new Rect();
    }
}
