﻿
namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
public record GetProductByCategoryResult(IEnumerable<Product> Products);

public class GetProductByCategoryHandler
    (IDocumentSession session)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
    {
        var product = await session.Query<Product>().
            Where(p => p.Category.Contains(request.Category)).
            ToListAsync(cancellationToken);

        return new GetProductByCategoryResult(product);
    }
}
