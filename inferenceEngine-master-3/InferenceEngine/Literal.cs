namespace InferenceEngine
{
    public class Literal
    {
        public string Name;
        public bool IsNegated;

        public Literal(string name, bool isNegated)
        {
            Name = name;
            IsNegated = isNegated;
        }

        public Literal GetComplement() => new Literal(Name, !IsNegated);

        public override bool Equals(object obj)
        {
            if (obj is Literal other)
                return Name == other.Name && IsNegated == other.IsNegated;
            return false;
        }

        public override int GetHashCode() => Name.GetHashCode() * (IsNegated ? -1 : 1);

        public override string ToString() => (IsNegated ? "~" : "") + Name;
    }
}
