# astm-lib

ASTM library for laboratory information systems

## Quick Start

1. **Read CLAUDE.md first** - Contains essential rules for Claude Code
2. Follow the pre-task compliance checklist before starting any work
3. Use proper module structure under `src/`
4. Commit after every completed task

## Simple Project Structure

This project uses a simple structure suitable for basic C# libraries:

```
astm-lib/
├── CLAUDE.md              # Essential rules for Claude Code
├── README.md              # Project documentation
├── .gitignore             # Git ignore patterns
├── src/                   # Source code (NEVER put files in root)
│   ├── AstmLib.cs         # Main library classes
│   └── Utils.cs           # Utility functions
├── tests/                 # Test files
│   └── AstmLibTests.cs    # Basic tests
├── docs/                  # Documentation
└── output/                # Generated output files
```

## Development Guidelines

- **Always search first** before creating new files
- **Extend existing** functionality rather than duplicating  
- **Use Task agents** for operations >30 seconds
- **Single source of truth** for all functionality
- **C# conventions** - follow established C# naming and coding standards
- **Scalable** - start simple, grow as needed