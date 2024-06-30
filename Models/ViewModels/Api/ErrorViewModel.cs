using System.Collections.Generic;
using System.Linq;

namespace Models.ViewModels.Api;

public class ErrorViewModel(params string[] errors)
{
    public List<string> Errors { get; } = errors.Take(1).ToList();
}