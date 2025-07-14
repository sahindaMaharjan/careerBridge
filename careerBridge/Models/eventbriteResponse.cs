public class eventbriteResponse
{
    public List<EventItem> events { get; set; }
}

public class EventItem
{
    public Name name { get; set; }
    public Description description { get; set; }
    public string url { get; set; }
}

public class Name
{
    public string text { get; set; }
}

public class Description
{
    public string text { get; set; }
}
