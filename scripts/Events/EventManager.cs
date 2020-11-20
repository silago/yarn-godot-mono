using System.Collections.Generic;
using System;
namespace Events{

    public static class ObjectExt
    {
        public static void Subscribe<T>(this object _, Action<T> cb) where T : Message
        {
            EventManager<T>.Subscribe(cb);
        }
        
        public static void Unsubscribe<T>(this object _, Action<T> cb) where T : Message
        {
            EventManager<T>.Unsubscribe(cb);
        }
        
        public static void SendEvent<T>(this object _, T data) where T : Message
        {
            EventManager<T>.Invoke(data);
        }
        
        public static void SubscribeOnce<T>(this object @this, Action<T> action) where T : Message
        {
            void WrapperAction(T data)
            {
                action?.Invoke(data);
                EventManager<T>.Unsubscribe(WrapperAction);
            }
            EventManager<T>.Subscribe(WrapperAction);
        }
        
        public static void Unsubscribe<TMessage,TKey>(this object _, Action<TMessage> action, TKey key) where TMessage : Message
        {
            TopicEventManager<TMessage, TKey>.Unsubscribe(action, key);
        }
        public static void Subscribe<TMessage,TKey>(this object _, Action<TMessage> action, TKey key) where TMessage : Message
        {
            TopicEventManager<TMessage, TKey>.Subscribe(action, key);
        }
        public static void SendEvent<TMessage,TKey>(this object _, TMessage data, TKey key) where TMessage : Message
        {
            TopicEventManager<TMessage, TKey>.Invoke(data, key);
        }
    }

    public class Message {}

    class EventContainer<T>
    {
        public event Action<T> Event;

        public void Invoke(T msg)
        {
            Event?.Invoke(msg);
        }
    }

    static class TopicEventManager<TMessage, TKey> where TMessage : Message
    {
        static readonly Dictionary<TKey, EventContainer<TMessage>> _events = new Dictionary<TKey, EventContainer<TMessage>>();

        public static void Subscribe(Action<TMessage> action, TKey topic)
        {
            if (_events.TryGetValue(topic, out var container) == false)
            {
                _events[topic] = container = new EventContainer<TMessage>();
            }

            container.Event += action;
        }

        public static void Unsubscribe(Action<TMessage> action, TKey topic)
        {
            if (_events.TryGetValue(topic, out var container))
            {
                container.Event -= action;
            }
        }

        public static void Invoke(TMessage message, TKey key)
        {
            if (_events.TryGetValue(key, out var container))
            {
                container.Invoke(message);
            }
        }
    }
    
    static class EventManager<T> where T : Message {
        public static event Action<T> Event; 
        public static void Subscribe(Action<T> cb)  {       
            Event+=cb;
        }

        public static void Unsubscribe(Action<T> cb) {
            Event-=cb;
        }

        public static void Invoke(T data = null) {
                Event?.Invoke(data);
        }
    }

    
}
