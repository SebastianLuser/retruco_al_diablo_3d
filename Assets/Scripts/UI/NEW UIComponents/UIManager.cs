using System;
using Services;
using UnityEngine;

namespace UI
{
    public class UIManager2 : MonoBehaviour
    {
        void Awake()
        {
            ServiceLocator.Register<UIManager2>(this);
        }

        public void ShowComponent(string componentId)
        {
            UIComponentRegistry.ShowComponent(componentId);
        }

        public void HideComponent(string componentId)
        {
            UIComponentRegistry.HideComponent(componentId);
        }

        public void HideAll()
        {
            UIComponentRegistry.HideAll();
        }

        public T GetComponent<T>(string componentId) where T : class, IUIComponent
        {
            return UIComponentRegistry.GetComponent<T>(componentId);
        }

        public void RegisterEventHandler(string eventName, Action<string, object> handler)
        {
            UIComponentRegistry.RegisterEventHandler(eventName, handler);
        }

        public void UnregisterEventHandler(string eventName, Action<string, object> handler)
        {
            UIComponentRegistry.UnregisterEventHandler(eventName, handler);
        }
    }
}