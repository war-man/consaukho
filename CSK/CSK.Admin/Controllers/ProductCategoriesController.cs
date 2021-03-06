﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CSK.Data;
using CSK.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TNT.Core.Helpers.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSK.Admin.Controllers
{
    [Route("api/product-categories")]
    [ApiController]
    public class ProductCategoriesController : BaseController
    {
        public ProductCategoriesController(CSKContext context) : base(context)
        {
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create([FromForm]CreateCategoryViewModel model)
        {
            try
            {
                
                var cate = new ProductCategories()
                {
                    Active = true,
                    Description = model.description,
                    Id = Guid.NewGuid().ToString(),
                    Name = model.name,
                    ImageUrl = model.image_url,
                    CategoriesOfProducts = null
                };
                _context.ProductCategories.Add(cate);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception e)
            {
                return Error(new
                {
                    message = "Có lỗi xảy ra. Vui lòng thử lại.",
                    data = e,
                });
            }
        }

        [Authorize]
        [HttpPost("{id}")]
        public IActionResult Edit(string id,
            [FromForm]EditCategoryViewModel model)
        {
            try
            {
                var entity = _context.ProductCategories.FirstOrDefault(c => c.Id == id);
                if (entity == null)
                    return NotFound(new
                    {
                        message = "Không tìm thấy"
                    });

                entity.Name = model.name;
                entity.Description = model.description;
                entity.ImageUrl = model.image_url;

                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception e)
            {
                return Error(new
                {
                    message = "Có lỗi xảy ra. Vui lòng thử lại.",
                    data = e,
                });
            }
        }

        [HttpGet("")]
        public IActionResult Get([FromQuery]string[] fields,
            [FromQuery]string[] ids)
        {
            try
            {
                var query = _context.ProductCategories.Where(p => p.Active == true);
                if (ids != null && ids.Length > 0)
                    query = query.Where(c => ids.Contains(c.Id));
                var cates = query.ToList();
                if (fields == null || fields.Length == 0)
                    fields = new string[] { "info" };
                var list = new List<IDictionary<string, object>>();
                foreach (var c in cates)
                {
                    var obj = new Dictionary<string, object>();
                    list.Add(obj);

                    foreach (var f in fields)
                    {
                        switch (f)
                        {
                            case "info":
                                obj["active"] = c.Active;
                                obj["description"] = c.Description;
                                obj["id"] = c.Id;
                                obj["image_url"] = c.ImageUrl;
                                obj["name"] = c.Name;
                                break;
                            case "pcount":
                                obj["pcount"] = c.CategoriesOfProducts.Where(p => p.Product.Active == true).Count();
                                break;
                        }
                    }

                }

                return Ok(list);
            }
            catch (Exception e)
            {
                return Error(new
                {
                    message = "Có lỗi xảy ra. Vui lòng thử lại.",
                    data = e
                });
            }

        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                var cate = _context.ProductCategories
                    .FirstOrDefault(c => c.Id == id && c.Active == true);
                if (cate == null)
                    return NotFound(new
                    {
                        message = "Không tìm thấy"
                    });

                return Ok(new
                {
                    id = cate.Id,
                    image_url = cate.ImageUrl,
                    name = cate.Name,
                    description = cate.Description,
                    active = cate.Active
                });
            }
            catch (Exception e)
            {
                return Error(new
                {
                    message = "Có lỗi xảy ra. Vui lòng thử lại.",
                    data = e
                });
            }

        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var cate = _context.ProductCategories.FirstOrDefault(c => c.Id == id);
                if (cate == null)
                    return NotFound(new { message = "Không tìm thấy" });

                cate.Active = false;
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return Error(new
                {
                    message = "Có lỗi xảy ra. Vui lòng thử lại.",
                    data = e
                });
            }
        }

    }

    public class CreateCategoryViewModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
    }


    public class EditCategoryViewModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public string image_url { get; set; }
    }

}
