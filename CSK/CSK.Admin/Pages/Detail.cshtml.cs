using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSK.Admin.Pages
{
    public class DetailModel : PageModel
    {
        public string Id { get; set; }
        public void OnGet(string id)
        {
            Id = id;
        }
    }
}