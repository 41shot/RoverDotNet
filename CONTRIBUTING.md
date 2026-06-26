# Contributing to RoverDotNet

## Development Guidelines

### 1. Project Documentation

- **Always consult the root-level README.md** for the high-level plan and project overview
- **Update the README.md** whenever you implement a new feature or make significant changes
  - Add newly ported features to the "Currently ported" section
  - Remove items from "To be ported" as they are completed

### 2. Language and Spelling

- **Use British English** throughout the codebase for:
  - Comments
  - Documentation
  - String literals (where appropriate)
  - Variable and method names (where appropriate)

- **Use US English** when it makes more sense, particularly for:
  - Names derived from referenced types

### 3. Rover Source Reference

When referencing or porting functionality from Apollo Rover:

- **Official repository**: [apollographql/rover](https://github.com/apollographql/rover) on GitHub

Cross-reference the original Rover implementation to check parity when adding/modifying features.

### 4. Platform Support

- **Primary target**: Windows (must work)
- **Secondary targets**: Linux and macOS (nice to have, but optional)

When implementing features:
- Ensure Windows compatibility is fully tested
- Use cross-platform APIs where possible (e.g., `Path.Combine`, `Environment.NewLine`)
- If platform-specific code is required, clearly document Windows-only functionality
- Linux/Mac support should not compromise Windows functionality

### 5. Project Structure

The solution is organised into:
- `src/RoverDotNet.Config` - Rover config (e.g. whoami, auth) operations.
- `src/RoverDotNet.Client` - Apollo API client.
- `src/RoverDotNet.Dev` - Rover dev operation.
- `src/RoverDotNet.Core` - Shared interfaces, models, functionality, etc.
- `tests/` - Unit tests for each project

### 6. Testing

- Write unit tests for all new functionality
- Ensure tests pass before submitting changes
- Follow the existing test structure in the `tests/` directory

### 7. Code Style

- Add XML documentation comments for public APIs

## Getting Started

Prerequisites:
1. Requires rover.exe with path environment variable set.

RoverDotNet solution:
1. Clone the repository
2. Open `RoverDotNet.slnx` in Visual Studio
3. Build the solution to restore packages
4. Run tests to ensure everything works
5. Run demo project (optional)
6. Start coding!