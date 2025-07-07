using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HSM.Core.Event
{
 public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _handlers = new();
        private readonly Queue<IEvent> _eventQueue = new();
        private readonly object _lock = new();

        public void Subscribe<T>(IEventHandler<T> handler) where T : IEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_handlers.ContainsKey(eventType))
                    _handlers[eventType] = new List<object>();

                if (!_handlers[eventType].Contains(handler))
                    _handlers[eventType].Add(handler);
            }
        }

        public void Unsubscribe<T>(IEventHandler<T> handler) where T : IEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                if (_handlers.ContainsKey(eventType))
                    _handlers[eventType].Remove(handler);
            }
        }

        public void Publish<T>(T eventData) where T : IEvent
        {
            lock (_lock)
            {
                _eventQueue.Enqueue(eventData);
            }
        }

        public void ProcessPendingEvents()
        {
            var eventsToProcess = new Queue<IEvent>();
            
            lock (_lock)
            {
                while (_eventQueue.Count > 0)
                    eventsToProcess.Enqueue(_eventQueue.Dequeue());
            }

            while (eventsToProcess.Count > 0)
            {
                var eventData = eventsToProcess.Dequeue();
                ProcessEvent(eventData);
            }
        }

        private void ProcessEvent(IEvent eventData)
        {
            var eventType = eventData.GetType();
            
            if (!_handlers.ContainsKey(eventType))
                return;

            var handlersForType = _handlers[eventType].ToList();
            
            foreach (var handler in handlersForType)
            {
                try
                {
                    var method = handler.GetType().GetMethod("CanHandle");
                    var canHandle = (bool)method?.Invoke(handler, new object[] { eventData });
                    
                    if (canHandle)
                    {
                        var handleMethod = handler.GetType().GetMethod("HandleEvent");
                        handleMethod?.Invoke(handler, new object[] { eventData });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing event {eventType.Name}: {ex.Message}");
                }
            }
        }
    }
}