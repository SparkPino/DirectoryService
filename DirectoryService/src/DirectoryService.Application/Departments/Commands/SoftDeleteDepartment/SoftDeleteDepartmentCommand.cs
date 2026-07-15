using System.Windows.Input;
using DirectoryService.Domain.Departments.ValueObjects;
using ICommand = DirectoryService.Application.Abstraction.ICommand;

namespace DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(DepartmentId Id) : ICommand;