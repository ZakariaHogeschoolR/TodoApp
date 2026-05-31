public class Users : IHasId<int>, IComparable<Users>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
    private Users _currentUser { get; set; }
    public string role { get; set; }
    public bool changed { get; set; }

    public Users()
    {
        _currentUser = this;
    }

    public int CompareTo(Users? other)
    {
        if(other == null )
            return 1;

        return this.Id.CompareTo(other.Id);
    }
}