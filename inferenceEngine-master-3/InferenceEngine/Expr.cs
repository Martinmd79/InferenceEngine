namespace InferenceEngine
{
    public abstract class Expr
    {
        public abstract override string ToString();
    }

    public class Symbol : Expr
    {
        public string Name;
        public Symbol(string name) { Name = name; }
        public override string ToString() { return Name; }
    }

    public class Not : Expr
    {
        public Expr Operand;
        public Not(Expr operand) { Operand = operand; }
        public override string ToString() { return "~" + Operand.ToString(); }
    }

    public class And : Expr
    {
        public List<Expr> Conjuncts;
        public And(List<Expr> conjuncts) { Conjuncts = conjuncts; }
        public override string ToString() { return "(" + string.Join(" & ", Conjuncts.Select(c => c.ToString())) + ")"; }
    }

    public class Or : Expr
    {
        public List<Expr> Disjuncts;
        public Or(List<Expr> disjuncts) { Disjuncts = disjuncts; }
        public override string ToString() { return "(" + string.Join(" || ", Disjuncts.Select(d => d.ToString())) + ")"; }
    }
}
