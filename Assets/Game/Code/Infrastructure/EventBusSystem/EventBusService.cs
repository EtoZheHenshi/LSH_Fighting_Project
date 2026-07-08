using System;
using System.Collections.Generic;

namespace Code.Infrastructure.EventBusSystem
{
    public sealed class EventBusService : Singleton<EventBusService>
    {
        private Dictionary<Type, Delegate> _events;

        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(gameObject);
            
            _events = new Dictionary<Type, Delegate>();
        }

        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            Type type = typeof(T);

            if (_events.ContainsKey(type))
            {
                _events[type] = Delegate.Combine(_events[type], callback);
            }
            else
            {
                _events.Add(type, callback);
            }
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            Type type = typeof(T);

            if (_events.TryGetValue(type, out Delegate action))
            {
                action = Delegate.Remove(action, callback);

                if (action == null)
                {
                    _events.Remove(type);
                }
                else
                {
                    _events[type] = action;
                }
            }
        }

        public void Publish<T>(T @event) where T : IEvent
        {
            Type type = typeof(T);

            if (_events.TryGetValue(type, out Delegate action))
            {
                ((Action<T>)action)?.Invoke(@event);
            }
        }
    }
}