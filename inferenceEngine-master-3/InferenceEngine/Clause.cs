namespace InferenceEngine
{
    public class Clause
    {
        public HashSet<Literal> Literals;

        public Clause(IEnumerable<Literal> literals)
        {
            Literals = new HashSet<Literal>(literals);
        }

        public bool IsEmpty() => Literals.Count == 0;

        public override bool Equals(object obj)
        {
            if (obj is Clause other)
                return Literals.SetEquals(other.Literals);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var lit in Literals.OrderBy(l => l.ToString()))
                hash = hash * 31 + lit.GetHashCode();
            return hash;
        }

        public override string ToString() =>
            Literals.Count == 0 ? "{}" : "{" + string.Join(", ", Literals.Select(l => l.ToString())) + "}";

        // Extract clauses from an expression in CNF.
        public static List<Clause> ExtractClauses(Expr expr)
        {
            List<Clause> clauses = new List<Clause>();
            if (expr is And andExpr)
            {
                foreach (Expr conjunct in andExpr.Conjuncts)
                    clauses.AddRange(ExtractClauses(conjunct));
            }
            else
            {
                clauses.Add(new Clause(ExtractLiterals(expr)));
            }
            return clauses;
        }

        private static List<Literal> ExtractLiterals(Expr expr)
        {
            List<Literal> literals = new List<Literal>();
            if (expr is Or orExpr)
            {
                foreach (Expr disj in orExpr.Disjuncts)
                    literals.AddRange(ExtractLiterals(disj));
            }
            else if (expr is Not notExpr && notExpr.Operand is Symbol sym)
            {
                var lit = new Literal(sym.Name, true);
                literals.Add(lit);
            }
            else if (expr is Symbol symbol)
            {
                var lit = new Literal(symbol.Name, false);  // FIX: false for positive symbol
                literals.Add(lit);
            }
            else
            {
                throw new Exception("Unexpected expression in literal extraction: " + expr.ToString());
            }
            return literals;
        }

        public static List<Clause> Resolve(Clause c1, Clause c2)
        {
            List<Clause> resolvents = new List<Clause>();
            foreach (var lit in c1.Literals)
            {
                Literal complementary = lit.GetComplement();
                if (c2.Literals.Contains(complementary))
                {
                    // Create a new clause by taking the union and then removing the complementary literals.
                    HashSet<Literal> newLiterals = new HashSet<Literal>(c1.Literals);
                    newLiterals.UnionWith(c2.Literals);
                    newLiterals.Remove(lit);
                    newLiterals.Remove(complementary);

                    Clause resolvent = new Clause(newLiterals);

                    // Check if the new clause is a tautology.
                    if (!resolvent.IsTautology())
                    {
                        resolvents.Add(resolvent);
                    }
                }
            }
            return resolvents;
        }


        public bool IsTautology()
        {
            foreach (var literal in Literals)
            {
                if (Literals.Contains(literal.GetComplement()))
                    return true;
            }
            return false;
        }

    }
}
