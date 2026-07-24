using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Commands.GetAllChildren;

public record GetAllChildrenQuery(string rootIdentifier) : IQuery;