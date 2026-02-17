namespace Wrap.Services.Core.Interface;

using ViewModels.Production;

public interface IProductionService
{
    /// <summary>
    /// Gets all productions with basic info for display in a list/overview page
    /// </summary>
    /// <returns>IEnumerable<AllProductionsViewModel> collection of productions</returns>
    Task<IEnumerable<AllProductionsViewModel>> GetAllProductionsAsync();
    
    /// <summary>
    /// Gets detailed information about a specific production by its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <returns>DetailsProductionViewModel</returns>
    Task<DetailsProductionViewModel?> GetProductionDetailsAsync(string id);
    
    /// <summary>
    /// Creates a new production based on the provided input model
    /// and returns the ID of the newly created production
    /// </summary>
    /// <param name="inputModel">CreateProductionInputModel</param>
    /// <returns>string: production's ID</returns>
    Task<string> CreateProductionInputModelAsync(CreateProductionInputModel inputModel);
    
    /// <summary>
    /// Gets the current details of a production for editing purposes, based on its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <returns>EditProductionInputModel</returns>
    Task<EditProductionInputModel?> GetEditProductionAsync(string id);
    
    /// <summary>
    /// Updates an existing production with new details provided in the input model, based on its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <param name="inputModel">EditProductionInputModel</param>
    Task UpdateProductionAsync(string id, EditProductionInputModel inputModel);
    
    /// <summary>
    /// Gets the details of a production for confirmation before deletion, based on its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <returns>DeleteProductionViewModel</returns>
    Task<DeleteProductionViewModel?> GetDeleteProductionAsync(string id);
    
    /// <summary>
    /// Deletes a production from the system based on its ID after confirmation
    /// </summary>
    /// <param name="id">string</param>
    Task DeleteProductionAsync(string id);
}