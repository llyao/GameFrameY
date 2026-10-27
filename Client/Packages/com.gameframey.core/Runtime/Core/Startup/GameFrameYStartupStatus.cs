using System;

namespace GameFrameY.Core.Startup
{
    /// <summary>
    /// 启动状态快照与转换器。它不依赖 Unity，供启动协调器、流程和测试共享。
    /// </summary>
    public sealed class GameFrameYStartupStatus
    {
        private readonly object _sync = new();
        private GameFrameYStartupState _state = GameFrameYStartupState.Created;
        private Exception _failure;
        private bool _gameReadyRaised;

        public GameFrameYStartupState State
        {
            get
            {
                lock (_sync)
                {
                    return _state;
                }
            }
        }

        public Exception Failure
        {
            get
            {
                lock (_sync)
                {
                    return _failure;
                }
            }
        }

        public bool IsGameReadyRaised
        {
            get
            {
                lock (_sync)
                {
                    return _gameReadyRaised;
                }
            }
        }

        public bool TryTransition(GameFrameYStartupState nextState)
        {
            lock (_sync)
            {
                if (_state == nextState)
                {
                    return true;
                }

                if (!IsValidTransition(_state, nextState))
                {
                    return false;
                }

                _state = nextState;
                if (_state != GameFrameYStartupState.Failed)
                {
                    _failure = null;
                }

                return true;
            }
        }

        public bool TryFail(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            lock (_sync)
            {
                if (_state is GameFrameYStartupState.Ready
                    or GameFrameYStartupState.Stopping
                    or GameFrameYStartupState.Stopped
                    or GameFrameYStartupState.Failed)
                {
                    return false;
                }

                _failure = exception;
                _state = GameFrameYStartupState.Failed;
                return true;
            }
        }

        public bool TryMarkGameReady()
        {
            lock (_sync)
            {
                if (_gameReadyRaised || _state != GameFrameYStartupState.EnteringGame)
                {
                    return false;
                }

                _gameReadyRaised = true;
                _state = GameFrameYStartupState.Ready;
                return true;
            }
        }

        public void Reset()
        {
            lock (_sync)
            {
                _state = GameFrameYStartupState.Created;
                _failure = null;
                _gameReadyRaised = false;
            }
        }

        private static bool IsValidTransition(GameFrameYStartupState current, GameFrameYStartupState next)
        {
            return (current, next) switch
            {
                (GameFrameYStartupState.Created, GameFrameYStartupState.Bootstrapping) => true,
                (GameFrameYStartupState.Bootstrapping, GameFrameYStartupState.Patching) => true,
                (GameFrameYStartupState.Patching, GameFrameYStartupState.LoadingConfig) => true,
                (GameFrameYStartupState.LoadingConfig, GameFrameYStartupState.EnteringGame) => true,
                (GameFrameYStartupState.Failed, GameFrameYStartupState.Bootstrapping) => true,
                (GameFrameYStartupState.Failed, GameFrameYStartupState.Patching) => true,
                (GameFrameYStartupState.Failed, GameFrameYStartupState.LoadingConfig) => true,
                (GameFrameYStartupState.Failed, GameFrameYStartupState.EnteringGame) => true,
                (_, GameFrameYStartupState.Stopping) => current != GameFrameYStartupState.Stopped,
                (GameFrameYStartupState.Stopping, GameFrameYStartupState.Stopped) => true,
                _ => false,
            };
        }
    }
}
