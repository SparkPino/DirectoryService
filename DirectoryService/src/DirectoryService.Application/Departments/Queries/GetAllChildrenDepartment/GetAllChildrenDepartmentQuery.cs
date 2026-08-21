using Core;

namespace DirectoryService.Application.Departments.Commands.GetAllChildren;

public record GetAllChildrenQuery(string rootIdentifier) : IQuery;