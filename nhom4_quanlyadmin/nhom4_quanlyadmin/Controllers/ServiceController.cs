using Microsoft.AspNetCore.Mvc;
using nhom4_quanlyadmin.Models;
using nhom4_quanlyadmin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nhom4_quanlyadmin.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ApiService _apiService;

        public ServiceController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var services = await _apiService.GetAsync<List<ServiceModel>>("Service");
            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                
                service.Price = Math.Round(service.Price ?? 0, 2);

                var response = await _apiService.PostAsync("Service", service);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Service added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add service.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(service);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _apiService.GetAsync<ServiceModel>($"Service/{id}");
            if (service == null)
            {
                return NotFound();
            }

            if (service.Price == null)
            {
                service.Price = 0; 
            }

            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceModel service)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.PutAsync($"Service/{service.Id}", service);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Service updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update service.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"Service/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Service deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete service.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}