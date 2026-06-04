using System.Windows.Input;
using DirectoryService.Domain.Locations.ValueObjects;
using ICommand = DirectoryService.Application.Abstraction.ICommand;

namespace DirectoryService.Application.Locations.Failures;

public record GetByIdLocationCommand(LocationId LocationId) : ICommand;