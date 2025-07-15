# Azuro Enterprise Library

A comprehensive .NET framework library providing enterprise-grade components for data access, caching, logging, configuration, messaging, and Windows services. This is a legacy codebase open-sourced for educational purposes.

## Overview

The Azuro Enterprise Library is a collection of utility libraries originally developed for enterprise .NET applications. It demonstrates classic enterprise patterns and architectural approaches that were common in .NET Framework applications.

## Key Components

### Core Libraries

- **Azuro.Common** - Foundation library with data abstractions, configuration management, caching, validation, and utilities
- **Azuro.Data** - Data access layer implementing DAO patterns with ORM-like functionality
- **Azuro.Logging** - Comprehensive logging framework with multiple output destinations
- **Azuro.Caching** - Pluggable caching implementations (HTTP, in-memory)

### Specialized Libraries

- **Azuro.MSMQ** - Message queue processing utilities
- **Azuro.ActiveDirectory** - Active Directory integration helpers
- **Azuro.Common.WindowsService** - Windows service utilities and installers
- **Azuro.Common.Security** - Security and cryptography utilities

## Architecture Highlights

This library demonstrates several enterprise patterns:

- **Layered Architecture** with clear separation of concerns
- **Active Record Pattern** for data entities with attribute-based ORM
- **Plugin Architecture** with configurable implementations
- **Enterprise Integration Patterns** for messaging and data access
- **Configuration-Driven Design** with extensive customization options

## Technology Stack

- .NET Framework 4.5.2
- NLog for logging
- MSTest for unit testing
- MSMQ for messaging
- SQL Server integration
- WCF support

## Building and Development

### Prerequisites
- Visual Studio 2013 or later
- .NET Framework 4.5.2
- NuGet Package Manager

### Build Commands
```bash
# Build entire solution
msbuild Azuro.EnterpriseLibrary.sln /p:Configuration=Release

# Run tests
mstest /testcontainer:Unit.Test.EnterpriseLibrary/bin/Debug/Unit.Test.EnterpriseLibrary.dll

# Create NuGet packages
./Azuro.Common/nugetpush.bat
```

## Educational Value

This codebase serves as an example of:

- **Enterprise .NET Architecture** from the mid-2010s era
- **Custom ORM Implementation** predating modern ORMs like Entity Framework Core
- **Configuration Management** patterns in .NET Framework
- **Logging Framework Design** with pluggable destinations
- **Windows Service Development** patterns and utilities
- **Message Queue Processing** with MSMQ
- **Caching Strategies** and implementations

## License

This project is open-sourced for educational purposes. See the license information for details.

## Historical Context

This library represents .NET development practices and patterns from the 2010-2015 era, before many modern .NET technologies became mainstream. It provides insight into how enterprise applications were structured and how common problems were solved during the .NET Framework era.

## Note for Modern Development

While this code demonstrates valuable architectural concepts, modern .NET development has evolved significantly. For new projects, consider using:

- .NET 5+ instead of .NET Framework
- Entity Framework Core for data access
- Built-in logging abstractions (Microsoft.Extensions.Logging)
- Built-in dependency injection
- Modern caching abstractions (Microsoft.Extensions.Caching)

This codebase is valuable for understanding historical approaches and learning from established patterns.