namespace Model
{
    public enum statusProgression
    {
        None = 0,
        ToDo = 1,
        InProgress = 2,
        Done = 3
    }
    public class TaskItem
    {
        public int Id { get; set; }
        public int showId { get; set; }
        public required string Description { get; set; }
        public bool Completed { get; set; }
        public statusProgression Status { get; set; }
        public int Priority { get; set; }
        public Users[] TeamMembersArray { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool changed { get; set; }

    }
}