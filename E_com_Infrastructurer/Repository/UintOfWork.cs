using E_com_Core.Interfases;

public class UnitOfWork : IUintOfWork
{
    public ICategoryRepository categoryRepository { get; }
    public IProductRepository productRepository { get; }
    public IPhotoRepository photoRepository { get; }
    public ICustomerBasketRepository  customerBasket { get; }

    public UnitOfWork(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IPhotoRepository photoRepository,
        ICustomerBasketRepository customerBasket)
    {
        this.categoryRepository = categoryRepository;
        this.productRepository = productRepository;
        this.photoRepository = photoRepository;
        this.customerBasket = customerBasket;
    }
}