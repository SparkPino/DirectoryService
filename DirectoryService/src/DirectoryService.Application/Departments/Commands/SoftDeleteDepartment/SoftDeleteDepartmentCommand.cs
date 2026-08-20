using Core;
using DirectoryService.Domain.Departments.ValueObjects;


namespace DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(DepartmentId Id) : ICommand;