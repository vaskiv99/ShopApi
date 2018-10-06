﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using ShopService.Data.Repositories.Shop;
using ShopService.Services.Adapters;
using ShopService.Services.Commands;
using ShopService.Services.Responses;

namespace ShopService.Services.Handlers
{
    public class CategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>,
        IRequestHandler<UpdateCategoryCommand,CategoryResponse>,
        IRequestHandler<DeleteCategoryCommand,bool>
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryCommandHandler(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var model = request.ToModel();

            _categoryRepository.Create(model);
            await _categoryRepository.SaveAsync().ConfigureAwait(false);

            return model.ToResponse();
        }

        public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = (await _categoryRepository.FindAsync(c => c.Id == request.Id && !c.IsDeleting)).Data;

            category.Name = request.Name;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveAsync();

            return category.ToResponse();
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = (await _categoryRepository.GetWithIncludeAsync(c => c.Id == request.Id && !c.IsDeleting)).Data;

            if (category.GoodsCategories.Any())
            {
                category.IsDeleting = true;
                _categoryRepository.Update(category);
            }

            else
            {
                _categoryRepository.Delete(category);
            }

            await _categoryRepository.SaveAsync();

            return true;
        }

    }
}
