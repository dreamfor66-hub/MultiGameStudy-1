using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Rogue.Ingame.Event
{
    public class MessageMediator<S>
    {
        private readonly Dictionary<Type, List<Delegate>> typeToHandlers = new Dictionary<Type, List<Delegate>>();
        private readonly Queue<Action> actionQueue = new Queue<Action>();
        private bool sendNow = false;
        private readonly Thread mainThread = Thread.CurrentThread;

        public void Listen<T>(Action<T> handler) where T : S
        {
            if (!sendNow)
            {
                ListenCore(handler);
            }
            else
            {
                AddListenToQueue(handler);
            }
        }

        public void Remove<T>(Action<T> handler) where T : S
        {
            if (!sendNow)
            {
                RemoveCore(handler);
            }
            else
            {
                AddRemoveToQueue(handler);
            }
        }

        public void Send<T>(T evt) where T : S
        {
            if (!CheckMainThread())
                return;

            if (sendNow)
            {
                AddSendToQueue(evt);
                return;
            }


            sendNow = true;
            SendCore(evt);
            var loop = 0;

            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                action.Invoke();

                loop++;
                if (loop > 1000)
                {
                    actionQueue.Clear();
                    Debug.LogError("loop count error. maybe infinite send chain");
                    break;
                }
            }


            sendNow = false;
        }

        private void ListenCore<T>(Action<T> handler) where T : S
        {
            var handlers = GetHandler<T>();
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
            else
            {
                Debug.LogError("already added handler : " + handler);
            }
        }

        private void RemoveCore<T>(Action<T> handler) where T : S
        {
            var handlers = GetHandler<T>();
            handlers.Remove(handler);
        }

        private void SendCore<T>(T evt) where T : S
        {
            var handlers = GetHandler<T>();
            foreach (var handler in handlers)
            {
                try
                {
                    ((Action<T>)handler)(evt);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private void AddSendToQueue<T>(T evt) where T : S
        {
            actionQueue.Enqueue(() => { SendCore(evt); });
        }

        private void AddListenToQueue<T>(Action<T> handler) where T : S
        {
            actionQueue.Enqueue(() => { ListenCore(handler); });
        }

        private void AddRemoveToQueue<T>(Action<T> handler) where T : S
        {
            actionQueue.Enqueue(() => { RemoveCore(handler); });
        }

        private bool CheckMainThread()
        {
            if (Thread.CurrentThread == mainThread)
                return true;

            Debug.LogError($"current thread is not main thread");
            return false;
        }

        private List<Delegate> GetHandler<T>() where T : S
        {
            var type = typeof(T);
            if (!typeToHandlers.ContainsKey(type))
            {
                typeToHandlers.Add(type, new List<Delegate>());
            }
            return typeToHandlers[type];
        }
    }
}