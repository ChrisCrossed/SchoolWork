using UnityEngine;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
    private new Transform transform = null;

    public static void Add(SpawnPoint _newSpawnPoint)
    {
        spawnPoints.Add(_newSpawnPoint);
    }
    public static void Remove(SpawnPoint _newSpawnPoint)
    {
        spawnPoints.Remove(_newSpawnPoint);
    }
    public static SpawnPoint GetRandom()
    {
        int _randIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[_randIndex];
    }
    public static SpawnPoint GetBest(int _team)
    {
        if (GameManager.instance.gameOver)
            return null;

        if (PlayerManager.playersInWorld == 0)
            return GetRandom();

        List<float> _highestValues = new List<float>() { float.NegativeInfinity };
        List<SpawnPoint> _bestSpawnPoints = new List<SpawnPoint>() { null };

        //float _highestValue = float.NegativeInfinity;
        //SpawnPoint _bestSpawnPoint = null;
        foreach (SpawnPoint _spawnPoint in spawnPoints)
        {
            if (_spawnPoint == null)
                continue;

            float _totalValue = 0;
            Vector3 _spawnPointPosition = _spawnPoint.transform.position;
            Vector3 _spawnPointForward = _spawnPoint.transform.forward;

            foreach (Player _player in PlayerManager.players)
            {
                if (_player == null || _player.team == _team)
                    continue;

                Vector3 _vecToPlayer = (_player.transform.position + Vector3.up) - (_spawnPointPosition + Vector3.up);
                Vector3 _vecToSpawnPoint = (_spawnPointPosition + Vector3.up) - (_player.transform.position + Vector3.up);
                float _distance = (_player.transform.position - _spawnPointPosition).magnitude;
                _totalValue += PlayerManager.instance.spawnDistanceValueCurve.Evaluate(_distance / 100) * PlayerManager.instance.spawnDistanceInfluence;

                float _firstAngle = Vector3.Angle(_player.transform.forward, _vecToSpawnPoint);
                float _firstAngleRatio = PlayerManager.instance.spawnLookValueCurve.Evaluate(_firstAngle / 180) * PlayerManager.instance.spawnAngleInfluence;
                _totalValue += _firstAngleRatio;

                float _secondAngle = Vector3.Angle(_spawnPointForward, _vecToPlayer);
                float _secondAngleRatio = PlayerManager.instance.spawnLookValueCurve.Evaluate(_secondAngle / 180) * PlayerManager.instance.spawnAngleInfluence;
                _totalValue += _secondAngleRatio * _firstAngleRatio;

                if (Physics.CheckSphere(_spawnPointPosition + _spawnPoint.transform.up, 0.75f))
                    _totalValue -= 100;

                if ((_firstAngle < 60 || _secondAngle < 60) && !Physics.Raycast(_player.transform.position + Vector3.up, _vecToSpawnPoint, _distance, 1 << 11, QueryTriggerInteraction.Ignore))
                {
                    _totalValue -= PlayerManager.instance.spawnSightInfluence;
                }
            }

            for (int i = _highestValues.Count - 1; i >= 0; i--)
            {
                if (_totalValue > _highestValues[i])
                {
                    _highestValues.Insert(i, _totalValue);
                    _bestSpawnPoints.Insert(i, _spawnPoint);

                    if (_highestValues.Count > PlayerManager.instance.maxSpawnValueCount)
                    {
                        _highestValues.RemoveAt(_highestValues.Count - 1); //remove the lowest value
                        _bestSpawnPoints.RemoveAt(_bestSpawnPoints.Count - 1); //remove the lowest value
                    }
                    break;
                }
            }
        }

        float _highestValuesTotal = 0;
        string _logString = "";
        for (int i = 0; i < _highestValues.Count; i++)
        {
            _highestValuesTotal += _highestValues[i];
            _logString += _highestValues[i];

            if (i != _highestValues.Count - 1)
                _logString += ", ";
        }

        float _randomValue = Random.Range(_highestValues[_highestValues.Count - 1], _highestValuesTotal);

        for (int i = 0; i < _highestValues.Count - 1; i++)
        {
            if (_randomValue > _highestValues[i + 1])
                return _bestSpawnPoints[i];
        }

        return _bestSpawnPoints[_bestSpawnPoints.Count - 1];
    }

    void Awake()
    {
        transform = base.transform;
        spawnPoints.Add(this);
    }

    // Use this for initialization
    void Start()
    {
        transform = base.transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}