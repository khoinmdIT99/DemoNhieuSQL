using System;
using System.Collections.Generic;
using System.Threading;

using Demo.Data;
using Demo.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Demo.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(ApplicationDbContext applicationDbContext) =>
            ApplicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));

        public ApplicationDbContext ApplicationDbContext { get; }

        IAsyncEnumerable<Product> Products { get; set }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
        {

        }
    }
}
