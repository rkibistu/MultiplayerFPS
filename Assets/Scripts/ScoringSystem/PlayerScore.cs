using Fusion;
using System;
using System.Diagnostics;
using UnityEngine.Events;
public class PlayerScore
{
    // PUBLIC MEMBERS

    public Action OnKillsChanged;
    public Action OnDeathsChanged;
    public Action OnAssistsChanged;

    public int Kills { get => _kills; private set { } }
    public int Deaths { get => _deaths; private set { } }
    public int Assists { get => _assists; private set { } }

    // PRIVATE MEMBERS

    private int _kills;
    private int _deaths;
    private int _assists;

    // PUBLIC METHODS

    public PlayerScore() {

        ResetScore();
    }


    public int IncrementKills() {

        _kills++;
        OnKillsChanged?.Invoke();

        return _kills;
    }
    public int IncrementDeaths() {

        _deaths++;
        OnDeathsChanged?.Invoke();

        return _deaths;
    }
    public int IncrementAssists() {

        _assists++;
        OnAssistsChanged?.Invoke();

        return _assists;
    }

    public void ResetScore() {

        _kills = 0;
        _deaths = 0;
        _assists = 0;

        OnKillsChanged?.Invoke();
        OnDeathsChanged?.Invoke();
        OnAssistsChanged?.Invoke();
    }

    public String Print() {

        return new String($"Kills: {_kills}. Deaths: {_deaths}. Assists: {_assists}");
    }
}
