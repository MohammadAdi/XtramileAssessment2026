# Xtramile .NET Assessment

This repository contains two assessment solutions:

1. **Algorithm.Practice** — a C# console application for the algorithm assessment.
2. **Xtramile Weather** — a practical .NET assessment using ASP.NET Core, Onion Architecture, CQRS, EF Core, external API integration, automated testing, and a lightweight frontend.

## Repository Structure

```text
.
├── Algorithm.Practice/
├── Xtramile Weather/
│   └── README.md
├── .gitignore
└── README.md
```

---

## 1. Algorithm.Practice

`Algorithm.Practice` is a C# console application that distributes warriors into dragon teams.

Each warrior belongs to one of four classes:

- Swordsman
- Archer
- Mage
- Healer

Each dragon can carry up to **5 warriors**.

### Team Rules

The implementation follows these rules:

- Prefer one warrior from each available class for every dragon.
- Fill remaining slots using the following priority:

```text
Swordsman -> Archer -> Mage -> Healer
```

- Maximum capacity is 5 warriors per dragon.
- The implementation keeps a maximum of 2 warriors from the same class in one dragon.
- A dragon may contain fewer than 5 warriors if a valid full group cannot be formed.
- Processing continues until all warriors are assigned.

### Implementation Assumption

The assessment statement allows an exception to the two-per-class rule when fewer than five warriors remain.

For this implementation, the **maximum of two warriors per class is kept as a strict constraint for every dragon**. When required, an additional dragon is created rather than exceeding the class limit.

### Run

From the repository root:

```bash
dotnet run --project ./Algorithm.Practice
```

The console requests:

```text
Input the number of swordsmen :
Input the number of archers :
Input the number of mages :
Input the number of healers :
```

### Example

Input:

```text
Swordsmen = 10
Archers   = 6
Mages     = 3
Healers   = 5
```

Output:

```text
Dragon 1 : Swordsmen: 2, Archers: 1, Mages: 1, Healers: 1
Dragon 2 : Swordsmen: 2, Archers: 1, Mages: 1, Healers: 1
Dragon 3 : Swordsmen: 2, Archers: 1, Mages: 1, Healers: 1
Dragon 4 : Swordsmen: 2, Archers: 2, Mages: 0, Healers: 1
Dragon 5 : Swordsmen: 2, Archers: 1, Mages: 0, Healers: 1
```

---

## 2. Xtramile Weather

`Xtramile Weather` contains the practical .NET assessment.

The project covers:

- ASP.NET Core Web API
- Onion Architecture
- CQRS with MediatR
- EF Core persistence
- external weather API integration
- temperature conversion
- frontend workflow
- automated testing
- AI development context/instructions

Detailed architecture, configuration, build, run, and test instructions are documented inside the project:

```text
Xtramile Weather/README.md
```

## Requirements

- .NET 8 SDK
- Git
- Weather API credentials for live Weather integration

## Author

**Mohammad Adi Fadilah**

Senior .NET Software Engineer
