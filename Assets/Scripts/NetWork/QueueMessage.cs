using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class QueueMessage
{
    private static QueueMessage instance;
    public static QueueMessage GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new QueueMessage();
            }
            return instance;
        }
    }

    public static readonly object m_lockObject = new object();
    static Queue<KeyValuePair<string, string>> mEvents = new Queue<KeyValuePair<string, string>>();


    public void AddEvent(string _event, string data)
    {
        lock (m_lockObject)
        {
            mEvents.Enqueue(new KeyValuePair<string, string>(_event, data));
        }
    }

    public void Update()
    {
        if (mEvents.Count > 0)
        {
            while (mEvents.Count > 0)
            {
                KeyValuePair<string, string> _event = mEvents.Dequeue();
                //GameServerMgr.GetInstance().RequsterNotifier(12, _event.Key, _event.Value);
            }
        }
    }
}
