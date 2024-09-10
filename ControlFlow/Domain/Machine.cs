namespace ControlFlow.Domain
{
    class Machine
    {
        public Machine(string name, string role)
        {
            Name = name;
            Role = role;
        }

        public string Name { get; private set; }
        public string Role { get; private set; }
    }
}
