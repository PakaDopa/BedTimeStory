using System;
using System.Collections.Generic;
using UnityEngine;

public enum MEventType
{
    //Weapon
    OnShoot,
    ChangeArmo,
    ReloadingArmo,

    //Projectile
    EnemyHitted,
}
[Serializable]
public class TransformEventArgs : EventArgs
{
    public Transform transform;
    public object[] value;
    public TransformEventArgs(Transform transform, params object[] value)
    {
        this.transform = transform;
        this.value = value;
    }
}

namespace Manager
{
    public class EventManager : Singleton<EventManager>
    {
        protected EventManager() { }

        // �븮�� ����
        public delegate void OnEvent(MEventType MEventType, Component Sender, EventArgs args = null);
        private Dictionary<MEventType, List<OnEvent>> Listeners = new Dictionary<MEventType, List<OnEvent>>();

        public void AddListener(MEventType MEventType, OnEvent Listener)
        {
            List<OnEvent> ListenList = null;

            if (Listeners.TryGetValue(MEventType, out ListenList))
            {
                ListenList.Add(Listener);
                return;
            }

            ListenList = new List<OnEvent>
            {
                Listener
            };
            Listeners.Add(MEventType, ListenList);
        }

        public void PostNotification(MEventType MEventType, Component Sender, EventArgs args = null)
        {
            List<OnEvent> ListenList = null;

            if (!Listeners.TryGetValue(MEventType, out ListenList))
                return;

            for (int i = 0; i < ListenList.Count; i++)
            {
                if (ListenList[i].Target.ToString() == "null")
                    continue;
                ListenList[i](MEventType, Sender, args);
            }
        }

        public void RemoveListener(MEventType MEventType, object target)
        {
            if (Listeners.ContainsKey(MEventType) == false)
                return;

            foreach (OnEvent ev in Listeners[MEventType])
            {
                if (target == ev.Target)
                {
                    Listeners[MEventType].Remove(ev);
                    return;
                }
            }
            return;
        }
        public void RemoveEvent(MEventType MEventType) => Listeners.Remove(MEventType);
        public void RemoveRedundancies()
        {
            Dictionary<MEventType, List<OnEvent>> newListeners = new Dictionary<MEventType, List<OnEvent>>();

            foreach (KeyValuePair<MEventType, List<OnEvent>> Item in Listeners)
            {
                for (int i = Item.Value.Count - 1; i >= 0; i--)
                {
                    if (Item.Value[i].Equals(null))
                        Item.Value.RemoveAt(i);
                }

                if (Item.Value.Count > 0)
                    newListeners.Add(Item.Key, Item.Value);
            }

            Listeners = newListeners;
        }

        void OnLevelWasLoaded()
        {
            RemoveRedundancies();
        }

        public override void Init()
        {
            Debug.Log("EventManager Init Complete!");
        }
    }
}