using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Command.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _repo;
        private readonly IUnitOfWork _uow;

        public UpdateCategoryHandler(ICategoryRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await _repo.GetByIdAsync(request.Id, ct);
            if (category is null)
                throw new Exception("Category not found");
            _repo.Update(category);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
