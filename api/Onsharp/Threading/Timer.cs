using System;
using System.Collections.Generic;
using Onsharp.Native;

namespace Onsharp.Threading
{
    /// <summary>
    /// The timer class offers some kind of multi threading without breaking the server.
    /// </summary>
    public class Timer : IDisposable
    {
        private static readonly Dictionary<string, Action> DelayCallbacks = new Dictionary<string, Action>();
        private static readonly Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

        /// <summary>
        /// Delays the given callback the given amount of milliseconds.
        /// </summary>
        /// <param name="millis">The delaying milliseconds value</param>
        /// <param name="callback">The callback which gets called after the millis</param>
        public static void Delay(long millis, Action callback)
        {
            lock (DelayCallbacks)
            {
                string id;
                while (DelayCallbacks.ContainsKey(id = Guid.NewGuid().ToString())) ;
                DelayCallbacks.Add(id, callback);
                Onset.Delay(id, millis);
            }
        }

        /// <summary>
        /// Creates a timer to manage how it should function.
        /// </summary>
        /// <param name="callback">The callback which gets called on execute of the timer</param>
        /// <param name="interval">The interval of the timer</param>
        /// <returns>The wrapped timer object</returns>
        public static Timer Create(Action callback, double interval)
        {
            lock (Timers)
            {
                string id;
                while (Timers.ContainsKey(id = Guid.NewGuid().ToString())) ;
                Timer timer = new Timer(id, callback, Onset.CreateTimer(id, interval));
                Timers.Add(id, timer);
                return timer;
            }
        }

        private readonly string _callbackId;
        private readonly Action _callback;
        private readonly int _timerId;

        /// <summary>
        /// The remaining time of the interval when the callback gets executed next.
        /// </summary>
        public double RemainingInterval => Onset.GetTimerRemainingTime(_timerId);

        /// <summary>
        /// Whether the underlying timer is valid or not.
        /// </summary>
        public bool IsValid => Onset.IsTimerValid(_timerId);
        
        private Timer(string callbackId, Action callback, int timerId)
        {
            _callbackId = callbackId;
            _callback = callback;
            _timerId = timerId;
        }

        /// <summary>
        /// Destroys the timer.
        /// </summary>
        public void Destroy()
        {
            lock (Timers)
                Timers.Remove(_callbackId);
            Onset.DestroyTimer(_timerId);
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            Onset.PauseTimer(_timerId);
        }

        /// <summary>
        /// Unpauses the timer.
        /// </summary>
        public void Resume()
        {
            Onset.UnpauseTimer(_timerId);
        }

        /// <summary>
        /// Disposes the timer and destroys it.
        /// </summary>
        public void Dispose()
        {
            Destroy();
        }

        internal static void CallTimer(string id)
        {
            lock (Timers)
            {
                if(!Timers.ContainsKey(id)) return;
                Timers[id]._callback.Invoke();
            }
        }
        
        internal static void CallDelay(string id)
        {
            lock (DelayCallbacks)
            {
                if(!DelayCallbacks.ContainsKey(id)) return;
                DelayCallbacks[id].Invoke();
                DelayCallbacks.Remove(id);
            }
        }
    }
}