public class ScheduledTask
{
    public string Name { get; set; }
    public Action Task { get; set; }
    public long DelayMs { get; set; }
    public Timer Timer { get; set; }
}
