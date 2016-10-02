using NUnit.Framework;
using NSubstitute;
using Gemserk.Lockstep;
using NSubstitute.ReturnsExtensions;
using System.Collections.Generic;

namespace Gemserk.Lockstep.Tests
{
	public interface ReplayPlayerControls
	{
		bool IsPaused();

		float GetPlaybackSpeed();

		void Play(float speed);

		void Pause();

		void Seek(float time);

		float GetPlaybackTime();

		float GetTotalTime();

		void Restart();

		void Update (float dt);
	}

	public interface GameStateLoader
	{
		void Load(GameState gameState);
	}

	public interface GameUpdater
	{
		void Update(float dt);
	}

	// TODO: separate playback timeline from replay commands and stuff logic

	public class MyReplayPlayer : ReplayPlayerControls
	{
		Replay _replay;
		GameStateLoader _gameStateLoader;
		GameUpdater _gameUpdater;

		bool _paused;
		float _playbackSpeed;
		float _playbackTime;

		public MyReplayPlayer(Replay replay, GameStateLoader gameStateLoader, GameUpdater gameUpdater)
		{
			_replay = replay;
			_gameStateLoader = gameStateLoader;
			_gameUpdater = gameUpdater;
		}

		#region IReplayPlayer implementation

		public bool IsPaused ()
		{
			return _paused;
		}

		public float GetPlaybackSpeed ()
		{
			return _playbackSpeed;
		}

		public void Play (float speed)
		{
			_playbackSpeed = speed;
			_paused = false;	
		}

		public void Pause ()
		{
			_paused = true;
		}

		public void Seek (float time)
		{
			throw new System.NotImplementedException ();
		}

		public float GetPlaybackTime ()
		{
			return _playbackTime;
		}

		public float GetTotalTime ()
		{
			throw new System.NotImplementedException ();
		}

		public void Restart ()
		{
			_gameStateLoader.Load (_replay.GetInitialGameState ());
		}

		public void Update (float dt)
		{
			if (_paused)
				return;
			_playbackTime += dt;
		}
		#endregion
		
	}

	public class TestReplayImplementationPlayer
	{

		[Test]
		public void TestRestartShouldLoadInitialGameStateFromReplay()
		{
			var replay = NSubstitute.Substitute.For<Replay> ();
			// var gameStateProvider = NSubstitute.Substitute.For<GameStateProvider> ();
			var gameStateLoader = NSubstitute.Substitute.For<GameStateLoader> ();
			var gameUpdater = NSubstitute.Substitute.For<GameUpdater> ();

			GameState customGameState = NSubstitute.Substitute.For<GameState> ();

			replay.GetInitialGameState ().Returns (customGameState);

			var replayPlayer = new MyReplayPlayer (replay, gameStateLoader, gameUpdater);

			replayPlayer.Restart ();

			gameStateLoader.Received ().Load (customGameState);
		}

		[Test]
		public void TestReplayPlayerBasicAPI()
		{
			var replay = NSubstitute.Substitute.For<Replay> ();

			var gameStateLoader = NSubstitute.Substitute.For<GameStateLoader> ();
			var gameUpdater = NSubstitute.Substitute.For<GameUpdater> ();

			var replayPlayer = new MyReplayPlayer (replay, gameStateLoader, gameUpdater);

			replayPlayer.Pause ();
			Assert.That (replayPlayer.IsPaused (), Is.True);

			replayPlayer.Play(1.0f);
			Assert.That (replayPlayer.IsPaused (), Is.False);

			Assert.That (replayPlayer.GetPlaybackSpeed (), Is.EqualTo (1.0f));

			replayPlayer.Play(2.0f);
			Assert.That (replayPlayer.GetPlaybackSpeed (), Is.EqualTo (2.0f));
		}

		[Test]
		public void TestGetPlaybackTimeWhenUpdateCalledAndNotPaused()
		{
			var replay = NSubstitute.Substitute.For<Replay> ();

			var gameStateLoader = NSubstitute.Substitute.For<GameStateLoader> ();
			var gameUpdater = NSubstitute.Substitute.For<GameUpdater> ();

			var replayPlayer = new MyReplayPlayer (replay, gameStateLoader, gameUpdater);

			replayPlayer.Play(1.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (0.0f));

			replayPlayer.Update (5.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (5.0f));

			replayPlayer.Update (1.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (6.0f));
		}


		[Test]
		public void TestReplayDontUpdateGameIfPaused()
		{
			var replay = NSubstitute.Substitute.For<Replay> ();
			// var gameStateProvider = NSubstitute.Substitute.For<GameStateProvider> ();
			var gameStateLoader = NSubstitute.Substitute.For<GameStateLoader> ();
			var gameUpdater = NSubstitute.Substitute.For<GameUpdater> ();

			var replayPlayer = new MyReplayPlayer (replay, gameStateLoader, gameUpdater);

			replayPlayer.Pause ();
			Assert.That (replayPlayer.IsPaused (), Is.True);

			replayPlayer.Update (1.0f);

			Assert.That (replayPlayer.GetPlaybackTime (), Is.EqualTo (0.0f));

			gameUpdater.DidNotReceiveWithAnyArgs ().Update (Arg.Any<float> ());
		}
	}
	
}
