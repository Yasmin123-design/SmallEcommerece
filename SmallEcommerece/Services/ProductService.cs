﻿using SmallEcommerece.Models;
using SmallEcommerece.UnitOfWorks;

namespace SmallEcommerece.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _unitOfWork.Products.GetAllAsync();

        public async Task<Product> GetByIdAsync(int id) =>
            await _unitOfWork.Products.GetByIdAsync(id);

        public async Task<Product> AddAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CommitAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.CommitAsync();
            }
        }
    }

}
