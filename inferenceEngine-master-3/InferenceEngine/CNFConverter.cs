using InferenceEngine;

public static class CNFConverter
{
    public static Expr Convert(Expr expr)
    {
        expr = MoveNotInward(expr);
        expr = DistributeOrOverAnd(expr);
        return expr;
    }

    private static Expr MoveNotInward(Expr expr)
    {
        if (expr is Not notExpr)
        {

            if (notExpr.Operand is Not inner)
            {
                return MoveNotInward(inner.Operand);
            }
            else if (notExpr.Operand is And andExpr)
            {
                // ~(A & B) = ~A || ~B.
                return new Or(andExpr.Conjuncts.Select(c => MoveNotInward(new Not(c))).ToList());
            }
            else if (notExpr.Operand is Or orExpr)
            {
                // ~(A || B) = ~A & ~B.
                return new And(orExpr.Disjuncts.Select(d => MoveNotInward(new Not(d))).ToList());
            }
            else
            {
                return new Not(MoveNotInward(notExpr.Operand));
            }
        }
        else if (expr is And andEx)
        {
            return new And(andEx.Conjuncts.Select(MoveNotInward).ToList());
        }
        else if (expr is Or orEx)
        {
            return new Or(orEx.Disjuncts.Select(MoveNotInward).ToList());
        }
        else
        {
            return expr;
        }
    }

    private static Expr DistributeOrOverAnd(Expr expr)
    {
        if (expr is Or orExpr)
        {
            // If any disjunct is an And, distribute OR over AND.
            if (orExpr.Disjuncts.Any(e => e is And))
            {
                Expr andPart = orExpr.Disjuncts.First(e => e is And);
                And andExpr = (And)andPart;
                List<Expr> others = orExpr.Disjuncts.Where(e => e != andPart).ToList();
                List<Expr> distributed = new List<Expr>();
                foreach (Expr conjunct in andExpr.Conjuncts)
                {
                    Expr newOr = new Or(new List<Expr> { conjunct }.Concat(others).ToList());
                    distributed.Add(DistributeOrOverAnd(newOr));
                }
                return DistributeOrOverAnd(new And(distributed));
            }
            else
            {
                return new Or(orExpr.Disjuncts.Select(DistributeOrOverAnd).ToList());
            }
        }
        else if (expr is And andExpr)
        {
            return new And(andExpr.Conjuncts.Select(DistributeOrOverAnd).ToList());
        }
        return expr;
    }
}
