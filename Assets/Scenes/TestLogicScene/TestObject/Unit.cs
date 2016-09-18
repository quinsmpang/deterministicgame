using UnityEngine;
using Gemserk.Lockstep;

public class Unit : GameLogic, GameStateProvider
{
	Vector2 _gamePosition;

	Vector2 _destination;

	Vector2 _lastGamePosition;

	float speed = 1.0f;

	bool _moving = false;

	readonly PositionInterpolator positionInterpolator;

	public Vector2 Position {
		get {
			return _gamePosition;
		}
	}

	public Vector2 PreviousPosition {
		get {
			return _lastGamePosition;
		}
	}

	public PositionInterpolator UnitView {
		get {
			return positionInterpolator;
		}
	}

	public float Speed {
		get {
			return speed;
		}
		set {
			speed = value;
		}
	}

	#region GameStateProvider implementation

	public void SaveState (GameState gameState)
	{
		gameState.SetFloat ("position.x", _gamePosition.x);
		gameState.SetFloat ("position.y", _gamePosition.y);
		gameState.SetFloat ("speed", speed);
		gameState.SetBool ("moving", _moving);
		gameState.SetFloat ("destination.x", _destination.x);
		gameState.SetFloat ("destination.y", _destination.y);
	}

	#endregion

	public Unit(Vector2 position)
	{
		_gamePosition = position;

		positionInterpolator = new PositionInterpolator ();
		positionInterpolator.SetPosition (0, _gamePosition);

		_destination = _gamePosition;
		_moving = false;
	}

	public void SetPosition(Vector2 position)
	{
		_gamePosition = position;
		_lastGamePosition = _gamePosition;

		positionInterpolator.SetPosition (0, _gamePosition);

		_destination = _gamePosition;
		_moving = false;
	}

	public void MoveTo(Vector2 destination)
	{
		_destination = destination;
		_moving = true;
	}

	public void GameUpdate(float dt, int frame)
	{
		if (!_moving)
			return;

		_lastGamePosition = _gamePosition;

		Vector2 direction = (_destination - _gamePosition).normalized;

		float realSpeed = speed * dt;

		Vector2 newPosition = _gamePosition + direction * realSpeed;

		if ((_destination - newPosition).SqrMagnitude() < realSpeed * realSpeed) {
			//			newPosition = _destination;
			_moving = false;
		}

		_gamePosition = newPosition;

		positionInterpolator.UpdatePosition (dt, _gamePosition);
	}

}
