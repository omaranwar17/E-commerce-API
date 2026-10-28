using E_com_Core.Interfases;
using E_com_Core.Model.User;
using Microsoft.AspNetCore.Identity;

public class UnitOfWork : IUintOfWork
{
  
    public ICategoryRepository categoryRepository { get; }
    public IProductRepository productRepository { get; }
    public IPhotoRepository photoRepository { get; }
    public ICustomerBasketRepository  customerBasket { get; }

    public IAuth auth { get; }

    public UnitOfWork(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IPhotoRepository photoRepository,
        ICustomerBasketRepository customerBasket,
        IAuth auth)
    {
        this.categoryRepository = categoryRepository;
        this.productRepository = productRepository;
        this.photoRepository = photoRepository;
        this.customerBasket = customerBasket;
        this.auth = auth;
      
    }
}