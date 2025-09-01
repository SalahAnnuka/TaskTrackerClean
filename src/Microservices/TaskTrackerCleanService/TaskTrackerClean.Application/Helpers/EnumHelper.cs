using TaskTrackerClean.Domain.Enums;
namespace TaskTrackerClean.Application.Helpers;

public static class EnumHelper
{
    public static string GetPriorityText(Priority priority)
    {
        return priority switch
        {
            Priority.LOW => "Low",
            Priority.MEDIUM => "Medium",
            Priority.HIGH => "High",
            _ => "Unknown"
        };
    }

    public static string GetStatusText(Status status)
    {
        return status switch
        {
            Status.NEW => "New",
            Status.IN_PROGRESS => "In Progress",
            Status.COMPLETE => "Complete",
            _ => "Unknown"
        };
    }

    public static Priority? GetPriority(string priorityText)
    {
        priorityText = priorityText.ToLower();
        return priorityText switch
        {
            "low" => Priority.LOW,
            "medium" => Priority.MEDIUM,
            "high" => Priority.HIGH,
            _ => null
        };
    }

    public static Status? GetStatus(string statusText)
    {
        statusText = statusText.ToLower();
        return statusText switch
        {
            "new" => Status.NEW,
            "in progress" => Status.IN_PROGRESS,
            "complete" => Status.COMPLETE,
            _ => null
        };
    }

}