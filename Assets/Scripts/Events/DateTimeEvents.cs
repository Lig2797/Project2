using System;
using UnityEngine;

public class DateTimeEvents
{
    public event Action<DateTime> onDateChanged;
    public void DateChanged(DateTime dateTime)
    {
        onDateChanged?.Invoke(dateTime);
    }
}
