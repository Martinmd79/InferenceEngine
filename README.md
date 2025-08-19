# Inference Engine for Propositional Logic
[![.NET](https://img.shields.io/badge/.NET-6%2B-512BD4)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23-blue)](#)
[![Status](https://img.shields.io/badge/Status-Active-success)](#)

A C# project for automated reasoning over propositional logic. It supports multiple inference strategies, works with Horn-clause and general propositional knowledge bases, and exposes a simple command-line interface for experimentation and batch runs.

## Table of Contents
- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Data Structures](#data-structures)
- [Requirements](#requirements)
- [Quick Start](#quick-start)
- [Usage](#usage)
- [Input File Format](#input-file-format)
- [Example Output](#example-output)
- [Research & Enhancements](#research--enhancements)
- [Outcomes](#outcomes)

## Features
- Four reasoning backends:
  - Truth Table (**TT**) — exhaustive model checking across all assignments.
  - Forward Chaining (**FC**) — iteratively derives consequences from known facts.
  - Backward Chaining (**BC**) — proves a query by recursively satisfying rule antecedents.
  - Resolution (**RES**) — CNF conversion and clause resolution to refute ¬query.
- Accepts **Horn** and **general propositional** KBs.
- Supports standard connectives: `~` (negation), `&` (conjunction), `||` (disjunction), `=>` (implication), `<=>` (biconditional).
- CLI-first workflow for repeatable testing.
- Modular design so additional strategies can be dropped in with minimal friction.

## Architecture
The engine parses an input file into a knowledge base (KB) and a query, then routes execution to the selected method. Truth-table evaluation works over arbitrary propositional formulas; chaining methods operate naturally on Horn-style rules; resolution converts the KB ∪ {¬query} into CNF and searches for the empty clause.

```mermaid
flowchart LR
  A[CLI Args<br/>(method, file)] --> B[Parser<br/>TELL/ASK]
  B --> C[Knowledge Base]
  B --> D[Query]
  C --> E[TT]
  C --> F[FC]
  C --> G[BC]
  C --> H[RES (CNF)]
  D --> E
  D --> F
  D --> G
  D --> H
  E --> I[Answer]
  F --> I
  G --> I
  H --> I
```

**Methods at a glance**

| Method | Works With | Strengths | Notes |
|------:|:-----------|:----------|:-----|
| TT | General propositional KBs | Sound & complete; simple to validate | Exponential in symbols |
| FC | Horn rules + facts | Efficient for deriving many facts | Goal-agnostic; can explore broadly |
| BC | Horn rules + facts | Efficient for single-goal queries | Depth-first; needs cycle guards |
| RES | General propositional KBs | Uniform proof procedure; good for automation | Requires CNF; may need clause management |

## Project Structure
```text
/src
  InferenceEngine.cs          # Entry point: parse args, dispatch method
  TruthTableMethod.cs         # Exhaustive model checking
  ForwardChainingMethod.cs    # Data-driven inference
  BackwardChainingMethod.cs   # Goal-driven inference
  ResolutionMethod.cs         # CNF + resolution refutation
  Rule.cs                     # Premises + conclusion representation
  CNFConverter.cs             # AST → CNF transformations
  Clause.cs                   # Clause and literal set utilities
  Expr.cs                     # Abstract expression base
  Literal.cs                  # Propositional literal (with negation)
  Parser.cs                   # TELL/ASK + expression parser
```

> Folder name (`/src`) is illustrative; align this section with your actual repo layout.

## Data Structures
- **Knowledge Base**: `List<string>` — raw statements from `TELL`.
- **Inferred Facts**: `HashSet<string>` — deduped facts during FC/BC.
- **Agenda**: `Queue<string>` — propositions awaiting processing (FC).
- **Models**: `Dictionary<string, bool>` — symbol → truth value (TT).
- **Clauses**: `List<Clause>` — CNF clauses for the resolution loop.
- **Rule Graphs**: rule/premise relationships to support FC/BC traversal.

## Requirements
- .NET SDK 6.0 or newer
- Windows, macOS, or Linux

## Quick Start
```bash
git clone https://github.com/yourusername/inference-engine.git
cd inference-engine
dotnet build
```

## Usage
```bash
# General form
dotnet run <method> <path-to-input-file>

# Methods
#  TT  → Truth Table
#  FC  → Forward Chaining
#  BC  → Backward Chaining
#  RES → Resolution

# Example
dotnet run TT ./examples/example1.txt
```

## Input File Format
Each input must include a `TELL` block for the KB and an `ASK` block for the query.

```text
TELL
P => Q
Q => R
P

ASK
R
```

- `TELL` — facts and rules (Horn or general propositional formulas).
- `ASK` — single query to test for entailment.

## Example Output
```text
YES: 12 models support the query
```
or
```text
NO
```
(Output details depend on the selected method.)

## Research & Enhancements
- Extended **Truth Table** to evaluate **general propositional** KBs (not just Horn).
- Implemented a **Resolution** theorem prover:
  - Convert KB and `¬query` to CNF.
  - Apply binary resolution; derive the empty clause to prove entailment.
- Validated across Horn and non-Horn inputs to ensure correctness.

## Outcomes
- Fully working inference engine with **TT**, **FC**, **BC**, and **RES** backends.
- Clean separation of parsing, data structures, and reasoning strategies.
- Improved understanding and a solid base for future extensions (e.g., heuristics, DPLL/SAT integration, proof tracing).
