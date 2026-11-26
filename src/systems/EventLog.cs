namespace zombieroad.src.systems;

public class Event
{
    public enum EventType
    {
        MOVEMENT,
        COMBAT,
        MISC,
        EVENT_TYPE_COUNT
    };
    public EventType type;

    public string message;

    public Event(EventType type, string message)
    {
        this.type = type;
    }
}

public class EventLog
{
    private string[] messageLog;
    
    public EventLog()
    {
        messageLog = new string[256];
    }

    public void addEvent(Event e)
    {
        messageLog.Append(e.message);
    }
}