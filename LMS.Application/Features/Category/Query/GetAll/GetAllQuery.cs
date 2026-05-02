using LMS.Application.DTOs.Category;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Query.GetAll
{
    public record GetAllCategoriesQuery() : IRequest<List<CategoryDto>>;
}
