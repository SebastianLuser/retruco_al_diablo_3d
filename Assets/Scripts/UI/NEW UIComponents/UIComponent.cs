using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface IUIComponent
    {
        string ComponentId { get; }
        void Show();
        void Hide();
        void Initialize();
        void Cleanup();
        bool IsVisible { get; }
    }

    public interface IUIEventComponent : IUIComponent
    {
        event Action<string, object> OnUIEvent;
    }

    public abstract class BaseUIComponent : MonoBehaviour, IUIEventComponent
    {
        [SerializeField] protected GameObject rootPanel;
        [SerializeField] protected string componentId;

        public string ComponentId => componentId;
        public bool IsVisible => rootPanel != null && rootPanel.activeInHierarchy;

        public event Action<string, object> OnUIEvent;

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(componentId))
                componentId = GetType().Name;
        }

        public virtual void Initialize()
        {
            UIComponentRegistry.Register(this);
            SetupComponent();
            Hide();
        }

        public virtual void Show()
        {
            if (rootPanel != null)
                rootPanel.SetActive(true);
        }

        public virtual void Hide()
        {
            if (rootPanel != null)
                rootPanel.SetActive(false);
        }

        public virtual void Cleanup()
        {
            OnUIEvent = null;
        }

        protected abstract void SetupComponent();

        protected void EmitEvent(string eventName, object data = null)
        {
            OnUIEvent?.Invoke(eventName, data);
        }

        void OnDestroy()
        {
            UIComponentRegistry.Unregister(this);
            Cleanup();
        }
    }

    public static class UIComponentRegistry
    {
        private static Dictionary<string, IUIComponent> components = new Dictionary<string, IUIComponent>();
        private static Dictionary<string, Action<string, object>> eventHandlers = new Dictionary<string, Action<string, object>>();

        public static void Register(IUIComponent component)
        {
            components[component.ComponentId] = component;
            
            if (component is IUIEventComponent eventComponent)
            {
                eventComponent.OnUIEvent += HandleUIEvent;
            }

            Debug.Log($"UI Component registered: {component.ComponentId}");
        }

        public static void Unregister(IUIComponent component)
        {
            if (components.ContainsKey(component.ComponentId))
            {
                components.Remove(component.ComponentId);
                
                if (component is IUIEventComponent eventComponent)
                {
                    eventComponent.OnUIEvent -= HandleUIEvent;
                }
            }
        }

        public static T GetComponent<T>(string componentId) where T : class, IUIComponent
        {
            return components.TryGetValue(componentId, out var component) ? component as T : null;
        }

        public static void ShowComponent(string componentId)
        {
            if (components.TryGetValue(componentId, out var component))
                component.Show();
        }

        public static void HideComponent(string componentId)
        {
            if (components.TryGetValue(componentId, out var component))
                component.Hide();
        }

        public static void HideAll()
        {
            foreach (var component in components.Values)
                component.Hide();
        }

        public static void RegisterEventHandler(string eventName, Action<string, object> handler)
        {
            if (!eventHandlers.ContainsKey(eventName))
                eventHandlers[eventName] = handler;
            else
                eventHandlers[eventName] += handler;
        }

        public static void UnregisterEventHandler(string eventName, Action<string, object> handler)
        {
            if (eventHandlers.ContainsKey(eventName))
                eventHandlers[eventName] -= handler;
        }

        private static void HandleUIEvent(string eventName, object data)
        {
            if (eventHandlers.TryGetValue(eventName, out var handler))
                handler?.Invoke(eventName, data);
        }
    }
}