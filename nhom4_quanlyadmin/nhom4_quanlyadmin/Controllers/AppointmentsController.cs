using Microsoft.AspNetCore.Mvc;
using nhom4_quanlyadmin.Models;
using nhom4_quanlyadmin.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace nhom4_quanlyadmin.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApiService _apiService;

        public AppointmentController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var appointments = await _apiService.GetAsync<List<Appointments>>("Appointment");

            if (appointments == null || appointments.Count == 0)
            {
                TempData["ErrorMessage"] = "Không có cuộc hẹn nào để hiển thị.";
                return View(new List<Appointments>());
            }

            return View(appointments);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointments appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Gửi yêu cầu tạo cuộc hẹn mới đến API
                    var response = await _apiService.PostAsync("Appointment", appointment);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Appointment created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to create appointment.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating appointment: " + ex.Message;
                }
            }
            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var appointment = await _apiService.GetAsync<Appointments>($"Appointment/{id}");
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(appointment);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error fetching appointment details: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Appointments appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _apiService.PutAsync($"Appointment/{appointment.Id}", appointment);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Appointment updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update appointment.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error updating appointment: " + ex.Message;
                }
            }
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _apiService.DeleteAsync($"Appointment/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Appointment deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete appointment.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting appointment: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}