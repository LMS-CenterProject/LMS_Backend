using LMS.Domain.Interfaces;
using LMS.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Category.Command.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _repo;
        private readonly IUnitOfWork _uow;

        public DeleteCategoryHandler(ICategoryRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await _repo.GetByIdAsync(request.Id, ct);
            if (category is null)
                throw new Exception("Category not found");

            _repo.Remove(category);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
