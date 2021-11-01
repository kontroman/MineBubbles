using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HistoryHolder
{
    private List<GameState> _history = new List<GameState>();

    private GunController _gunController;
    private GridController _gridController;
    private GameObject _ballDummy;
    private ScoreController _scoreController;

    public void Setup(GunController gunController, GameObject grid)
    {
        _scoreController = ScoreController.Instance;
        _gunController = gunController;
        _gridController = grid.GetComponent<GridController>();

    }

    public void SaveState()
    {
        var gunState = _gunController.SaveState();
        var queueState = _gunController._QueueController.SaveState();
        var gridState = _gridController.SaveState();
        var scoreState = _scoreController.SaveState();

        var state = new GameState(gunState, gridState, queueState, scoreState);
        _history.Add(state);
    }

    public void RestoreLastState()
    {
        _ballDummy = _gunController._QueueController._dummy;

        if (_history.Any())
        {
            var state = _history[_history.Count - 1];
            _history.RemoveAt(_history.Count - 1);

            _gunController.RestoreFrom(state.gunState);
            _gunController._QueueController.RestoreFrom(state.queueState);
            _gridController.RestoreFrom(state.gridState, _ballDummy);
            _scoreController.RestoreFrom(state.scoreState);

            GunDisabler.Instance.RemoveAction("Reloading");

        }
    }
}
