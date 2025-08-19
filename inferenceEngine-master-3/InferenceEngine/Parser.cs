namespace InferenceEngine
{
    public class Parser
    {
        private string input;
        private int pos;

        public Parser(string input) { this.input = input; pos = 0; }

        private void SkipWhitespace()
        {
            while (pos < input.Length && char.IsWhiteSpace(input[pos]))
                pos++;
        }

        public Expr ParseExpression()
        {
            SkipWhitespace();
            return ParseBiconditional();
        }

        private Expr ParseBiconditional()
        {
            Expr left = ParseImplication();
            SkipWhitespace();
            while (Match("<=>"))
            {
                Expr right = ParseImplication();
                // Replace A <=> B with (~A || B) & (~B || A)
                left = new And(new List<Expr> {
                    new Or(new List<Expr> { new Not(left), right }),
                    new Or(new List<Expr> { new Not(right), left })
                });
                SkipWhitespace();
            }
            return left;
        }

        private Expr ParseImplication()
        {
            Expr left = ParseOr();
            SkipWhitespace();
            while (Match("=>"))
            {
                Expr right = ParseOr();
                // Replace A => B with ~A || B.
                left = new Or(new List<Expr> { new Not(left), right });
                SkipWhitespace();
            }
            return left;
        }

        private Expr ParseOr()
        {
            Expr left = ParseAnd();
            SkipWhitespace();
            while (Match("||"))
            {
                Expr right = ParseAnd();
                if (left is Or orLeft)
                {
                    orLeft.Disjuncts.Add(right);
                }
                else
                {
                    left = new Or(new List<Expr> { left, right });
                }
                SkipWhitespace();
            }
            return left;
        }

        private Expr ParseAnd()
        {
            Expr left = ParseUnary();
            SkipWhitespace();
            while (Match("&"))
            {
                Expr right = ParseUnary();
                if (left is And andLeft)
                {
                    andLeft.Conjuncts.Add(right);
                }
                else
                {
                    left = new And(new List<Expr> { left, right });
                }
                SkipWhitespace();
            }
            return left;
        }

        private Expr ParseUnary()
        {
            SkipWhitespace();
            if (Match("~"))
                return new Not(ParseUnary());
            else if (Match("("))
            {
                Expr expr = ParseExpression();
                SkipWhitespace();
                if (!Match(")"))
                    throw new Exception("Expected ')' at position " + pos);
                return expr;
            }
            else
                return ParseSymbol();
        }

        private Expr ParseSymbol()
        {
            SkipWhitespace();
            int start = pos;
            while (pos < input.Length && !char.IsWhiteSpace(input[pos]) && !"()~&|<=>".Contains(input[pos]))
                pos++;
            if (start == pos)
                throw new Exception("Expected symbol at position " + pos);
            string name = input.Substring(start, pos - start).Trim();
            return new Symbol(name);
        }

        private bool Match(string token)
        {
            SkipWhitespace();
            if (pos + token.Length <= input.Length && input.Substring(pos, token.Length) == token)
            {
                pos += token.Length;
                return true;
            }
            return false;
        }
    }
}
