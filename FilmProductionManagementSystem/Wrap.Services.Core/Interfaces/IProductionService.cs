namespace Wrap.Services.Core.Interfaces;

using Models.Production;

// using ViewModels.Production;

public interface IProductionService
{
    /// <summary>
    /// Gets all productions with basic info for display in a list/overview page
    /// </summary>
    /// <returns>IReadOnlyCollection<ProductionDto> collection of productions</returns>
    Task<IReadOnlyCollection<ProductionDto>> GetAllProductionsAsync();
    
    /// <summary>
    /// Gets detailed information about a specific production by its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <returns>DetailsProductionDto?</returns>
    Task<DetailsProductionDto?> GetProductionDetailsAsync(string? id);
    
    /// <summary>
    /// Creates a new production based on the provided dto
    /// and returns the ID of the newly created production
    /// </summary>
    /// <param name="dto">CreateProductionDto</param>
    /// <returns>string: production's ID</returns>
    Task<string> CreateProductionAsync(CreateProductionDto dto);
    
    /// <summary>
    /// Gets the current details of a production for editing purposes, based on its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <returns>EditProductionDto?</returns>
    Task<EditProductionDto?> GetEditProductionAsync(string? id);
    
    /// <summary>
    /// Updates an existing production with new details provided in the dto, based on its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <param name="dto">EditProductionDto</param>
    Task<bool> UpdateProductionAsync(string? id, EditProductionDto dto);
    
    /// <summary>
    /// Gets the details of a production for confirmation before deletion, based on its ID
    /// </summary>
    /// <param name="id">string</param>
    /// <returns>DeleteProductionDto?</returns>
    Task<DeleteProductionDto?> GetDeleteProductionAsync(string? id);
    
    /// <summary>
    /// Deletes a production from the system based on its ID after confirmation
    /// </summary>
    /// <param name="id">string</param>
    Task<bool> DeleteProductionAsync(string? id);
}