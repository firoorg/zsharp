namespace Zsharp.Elysium
{
    public sealed class Property
    {
        public Property(PropertyId id, TokenType type)
        {
            this.Id = id;
            this.Type = type;
        }

        public PropertyId Id { get; }

        public TokenType Type { get; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            return ((Property)obj).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
