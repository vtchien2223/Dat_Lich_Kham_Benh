using Microsoft.AspNetCore.Mvc;
using nhom4_quanlyadmin.Models;
using nhom4_quanlyadmin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nhom4_quanlyadmin.Controllers
{
    public class PostApiController : Controller
    {
        private readonly ApiService _apiService;

        public PostApiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _apiService.GetAsync<List<Post>>("PostApi");
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Post());  
        }
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;

                var response = await _apiService.PostAsync("PostApi", post);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Post created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Error creating post.";
            }
            return View(post);  
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _apiService.GetAsync<Post>($"PostApi/{id}");
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.PutAsync($"PostApi/{post.Id}", post);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Post updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Error updating post.";
            }
            return View(post);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"PostApi/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Post deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting post.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}