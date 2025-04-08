using Microsoft.AspNetCore.Mvc;
using nhom4_quanlyadmin.Models;
using nhom4_quanlyadmin.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace nhom4_quanlyadmin.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApiService _apiService;

        public PatientController(ApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var patients = await _apiService.GetAsync<List<Patients>>("Patient");
            return View(patients);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Create(Patients patient)
        {
            if (ModelState.IsValid)
            {
                patient.CreatedAt = DateTime.Now;

                var response = await _apiService.PostAsync("Patient", patient);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Patient added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add patient.";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(patient);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _apiService.GetAsync<Patients>($"Patient/{id}");
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Patients patient)
        {
            if (ModelState.IsValid)
            {
                if (patient.CreatedAt.HasValue)
                {
                    patient.CreatedAt = DateTime.ParseExact(patient.CreatedAt.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                var response = await _apiService.PutAsync($"Patient/{patient.Id}", patient);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Patient updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update patient.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(patient);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"Patient/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Patient deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete patient.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}