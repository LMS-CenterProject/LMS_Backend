using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Command.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICategoryRepository _repo;
        private readonly IUnitOfWork _uow;

        public CreateCategoryHandler(ICategoryRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var exists = await _repo.ExistsByNameAsync(request.Name, ct);
            if (exists)
                throw new Exception("Category already exists");

            var category = LMS.Domain.Entities.Category.Create(request.Name);

            await _repo.AddAsync(category, ct);
            await _uow.SaveChangesAsync(ct);

            return category.Id;
        }
    }
}
