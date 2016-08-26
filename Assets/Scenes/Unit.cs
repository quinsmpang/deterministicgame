﻿using UnityEngine;

public class UnitView
{
	float _lastTime;

	Vector2 _p0;
	Vector2 _p1;
	Vector2 _currentPosition;

	float _accumulatedDt;

	float _totalTime;

	public void SetPosition (float time, Vector2 position)
	{
		_lastTime = time;
		_p0 = position;
		_p1 = position;
	}

	public void UpdatePosition(float time, Vector2 position) 
	{
		_p0 = _currentPosition;
		_p1 = position;

		// _accumulatedDt -= time;
		_accumulatedDt = 0;
		_totalTime = time;
	}

	public Vector2 GetCurrentPosition(float dt)
	{
		_accumulatedDt += dt;
		_currentPosition = Vector2.Lerp (_p0, _p1, _accumulatedDt / _totalTime);
		return _currentPosition;
	}

}

public class Unit : MonoBehaviour {

	Vector2 _gamePosition;

	Vector2 _destination;

	Vector2 _debugLastGamePosition;

	public float speed = 1.0f;

	bool _moving = false;

	UnitView unitView;

	void Awake()
	{
		_gamePosition = transform.position;
		_debugLastGamePosition = _gamePosition;

		unitView = new UnitView ();
		unitView.SetPosition (0, _gamePosition);
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

		_debugLastGamePosition = _gamePosition;

		Vector2 direction = (_destination - _gamePosition).normalized;

		float realSpeed = speed * dt;

		Vector2 newPosition = _gamePosition + direction * realSpeed;
	
		if ((_destination - newPosition).SqrMagnitude() < realSpeed * realSpeed) {
			newPosition = _destination;
			_moving = false;
		}

		_gamePosition = newPosition;

		unitView.UpdatePosition (dt, _gamePosition);
	}

//	const float timePrecision = 0.0001f;

	void LateUpdate()
	{
		transform.position = unitView.GetCurrentPosition (Time.deltaTime);

//		t += Time.deltaTime;
//
//		if (Mathf.Abs(_t1 - _t0) < timePrecision) {
//			transform.position = _gamePosition;
//			return;
//		}
//
//		float tdiff = _t1 - _t0;
//
//		Debug.Log (string.Format("t:{0} t0:{1} t1:{2} tdiff:{3}", t, _t0, _t1, tdiff));
//
//		transform.position = Vector3.Lerp (_lastGamePosition, _gamePosition, t / tdiff);
//
//		lerp += interpolationRate;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (_debugLastGamePosition, 0.2f);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (_gamePosition, 0.2f);
	}

}
