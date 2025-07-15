# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is the Azuro Enterprise Library, a comprehensive .NET framework library providing enterprise-grade components for data access, caching, logging, configuration, messaging, and Windows services. The library follows classic enterprise patterns and is designed for maintainability, configurability, and performance in .NET applications.

## Build and Development Commands

### Building the Solution
```bash
# Build entire solution (Debug configuration)
msbuild Azuro.EnterpriseLibrary.sln /p:Configuration=Debug

# Build entire solution (Release configuration)  
msbuild Azuro.EnterpriseLibrary.sln /p:Configuration=Release

# Build specific project
msbuild Azuro.Common/Azuro.Common.csproj /p:Configuration=Release
```

### Running Tests
```bash
# Run all unit tests using MSTest
mstest /testcontainer:Unit.Test.EnterpriseLibrary/bin/Debug/Unit.Test.EnterpriseLibrary.dll

# Build and run tests in one command
msbuild Unit.Test.EnterpriseLibrary/Unit.Test.EnterpriseLibrary.csproj /p:Configuration=Debug && mstest /testcontainer:Unit.Test.EnterpriseLibrary/bin/Debug/Unit.Test.EnterpriseLibrary.dll
```

### NuGet Package Management
```bash
# Create NuGet packages for all main libraries
./Azuro.Common/nugetpush.bat

# Individual package creation (from respective directories)
nuget pack Azuro.Common.csproj -Prop Configuration=Release -Symbols
```

### Package Restoration
```bash
# Restore NuGet packages for all projects
nuget restore Azuro.EnterpriseLibrary.sln
```

## Architecture Overview

### Core Libraries and Responsibilities

**Azuro.Common** - Foundation library containing:
- Data layer abstraction (`ADataEntity`, `ADataEntityCache`)
- Configuration management (`ConfigurationHelper`, various config sections)
- Caching framework (`CacheManager`, `ICacheManager`)
- Validation framework (attribute-based validators)
- Cryptography utilities
- Reflection and serialization helpers
- WCF proxy utilities

**Azuro.Data** - Data access layer providing:
- `DataObject` class implementing DAO pattern
- Bulk data operations (`SqlBulkInserter`)
- Entity data reader functionality
- Configuration-driven data access

**Azuro.Logging** - Comprehensive logging framework:
- Multiple log sinks (EventLog, File, Database, Console)
- Configurable log routing and categories
- Structured logging with metadata

**Azuro.Caching** - Caching implementations:
- HTTP caching wrapper
- In-memory caching
- Configuration-driven cache management

**Azuro.MSMQ** - Message queue processing:
- MSMQ processor with configuration support
- Queue helper utilities

**Azuro.ActiveDirectory** - Active Directory integration:
- AD helper classes for user/group management

**Windows Service Libraries**:
- `Azuro.Common.WindowsService` - Service utilities and installers
- `Azuro.Common.MSMQ` - MSMQ helper for services

### Key Architectural Patterns

1. **Layered Architecture**: Clear separation between data, business, and infrastructure layers
2. **Plugin Architecture**: Configurable implementations using interfaces and factories
3. **Attribute-Driven Design**: Extensive use of attributes for metadata and configuration
4. **Active Record Pattern**: Data entities contain persistence logic via stored procedure attributes
5. **Enterprise Patterns**: Singleton, DAO, Strategy, Observer patterns throughout

### Configuration System

The library uses a sophisticated configuration system that:
- Supports assembly-specific configuration files
- Provides fallback mechanisms for missing configurations
- Uses custom configuration sections for each component
- Integrates with .NET's built-in configuration framework

### Data Access Patterns

The data layer implements:
- ORM-like functionality with attribute-based column mapping
- Stored procedure-based CRUD operations
- Automatic dirty tracking and change detection
- Configurable fill depth for related entities
- Built-in caching for performance optimization

## Dependencies and Technologies

- **.NET Framework 4.5.2** - Target framework
- **NLog 4.4.12** - Primary logging framework
- **Microsoft Visual Studio Unit Testing** - Test framework
- **System.ServiceModel** - WCF integration
- **System.Data** - Database connectivity
- **System.Configuration** - Configuration management

## Development Guidelines

### Project Structure
- Each library has its own project with clear separation of concerns
- Shared utilities are in `Azuro.Common`
- Data access patterns are centralized in `Azuro.Data`
- Infrastructure concerns (logging, caching) have dedicated projects

### Coding Patterns
- Use attribute-based configuration where possible
- Implement interfaces for pluggable components
- Follow established patterns for data entities (inherit from `ADataEntity`)
- Use the centralized logging framework via `LogStub`
- Leverage the caching framework for performance-critical operations

### Testing
- Unit tests are in `Unit.Test.EnterpriseLibrary`
- Tests use MSTest framework
- Focus on testing public APIs and integration points

### NuGet Packaging
- Each major component has its own NuGet package
- Use the provided batch script for consistent package creation
- Packages include symbols for debugging support