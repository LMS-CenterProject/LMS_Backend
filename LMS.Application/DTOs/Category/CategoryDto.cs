using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
