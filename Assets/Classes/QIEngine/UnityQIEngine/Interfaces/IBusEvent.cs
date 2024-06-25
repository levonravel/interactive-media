using System;

public interface IBusEvent
{
    public Action<IBusEvent> NotificationEvent { get; set; }

    public void Invoke();
}
