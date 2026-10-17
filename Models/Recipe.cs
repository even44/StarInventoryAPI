using System.ComponentModel.DataAnnotations;

public class Recipe : IValidatableObject
{
    public required int Id {get; set;}
    [Required(ErrorMessage = "Name must not be empty.")]
    public required string Name {get; set;}
    public required int[] UexItemIds {get; set;}
    public required int[] ItemAmounts {get; set;}

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UexItemIds == null || UexItemIds.Length == 0)
        {
            yield return new ValidationResult("A recipe must contain at least one item.", [nameof(UexItemIds)]);
        }
        if (ItemAmounts == null || UexItemIds == null || ItemAmounts.Length != UexItemIds.Length)
        {
            yield return new ValidationResult("ItemAmounts must have the same length as UexItemIds.", [nameof(ItemAmounts)]);
        }
        if (ItemAmounts != null && ItemAmounts.Any(amount => amount <= 0))
        {
            yield return new ValidationResult("Every item amount must be greater than 0.", [nameof(ItemAmounts)]);
        }
    }
}
