Inference Engine for Propositional Logic
An extensible C# inference engine that evaluates propositional logic statements using multiple reasoning strategies. Built for learning, experimentation, and automation in logical reasoning systems.

Features

Four inference methods:

📊 Truth Table (TT): Exhaustive evaluation across all models.
🔀 Forward Chaining (FC): Iteratively derives new facts until query is reached.
🔁 Backward Chaining (BC): Works backward from query through supporting rules.
📐 Resolution (RES): Converts to CNF and applies resolution-based theorem proving.
Supports both Horn-clause and general propositional knowledge bases
Handles standard logical connectives:
Negation ~
Conjunction &
Disjunction ||
Implication =>
Biconditional <=>
Command-line interface for batch testing with different reasoning methods.
Modular C# design → easy to extend with new inference strategies.


Project Structure:

The project consists of 11 C# source files, each handling a distinct part of the inference process:
InferenceEngine.cs → Entry point, parses CLI args, selects reasoning method.
TruthTableMethod.cs → Exhaustive truth table evaluation.
ForwardChainingMethod.cs → Iterative fact inference.
BackwardChainingMethod.cs → Recursive rule tracing.
ResolutionMethod.cs → CNF conversion + resolution prover.
Rule.cs → Encapsulates logical rules (premises + conclusion).
CNFConverter.cs → Converts expressions into CNF.
Clause.cs → Models CNF clauses.
Expr.cs → Abstract base class for logical expressions.
Literal.cs → Handles propositional literals & negations.
Parser.cs → Transforms input into structured logical representations.


Data Structures:

Knowledge Base → List<string>
Inferred Facts → HashSet<string>
Agenda (queue) → Queue<string>
Truth Table Models → Dictionary<string, bool>
Resolution Clauses → List<Clause>
Rule Graphs → Used in forward/backward chaining


Getting Started:

1. Clone the Repository
git clone https://github.com/yourusername/inference-engine.git
cd inference-engine
2. Build the Project
dotnet build
3. Run the Program
dotnet run <method> <path-to-input-file>
Where <method> is one of:
TT → Truth Table
FC → Forward Chaining
BC → Backward Chaining
RES → Resolution
Example:
dotnet run TT ./examples/example1.txt


Input File Format:

Each input file must contain two sections:
TELL
P => Q
Q => R
P

ASK
R
TELL → Knowledge base (facts & rules)
ASK → Query

Example Output:

YES: 12 models support the query
or
NO
Depending on reasoning method used.


Research Enhancements:


Extended Truth Table to support general propositional logic (not limited to Horn clauses).
Added Resolution Theorem Prover, widely used in automated reasoning.
Validated correctness through extensive testing across Horn and non-Horn knowledge bases.


Outcomes:

Fully functional inference engine supporting four reasoning methods
Strong collaboration workflow via GitHub + Discord
Deeper understanding of propositional logic and AI reasoning systems
