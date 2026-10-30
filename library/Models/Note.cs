using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace library.Models;

public class Note
{
    public long NoteId { get; set; }         
    public string Title { get; set; } = "";  
    public string? Body { get; set; }

    [BindNever, ValidateNever]
    public DateOnly CreatedOn { get; set; }
}