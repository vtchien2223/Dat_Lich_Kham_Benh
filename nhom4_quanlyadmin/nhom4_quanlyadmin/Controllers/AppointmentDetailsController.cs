using Microsoft.AspNetCore.Mvc;
using nhom4_quanlyadmin.Models;
using nhom4_quanlyadmin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace nhom4_quanlyadmin.Controllers
{
    public class AppointmentDetailsController : Controller
    {
        private readonly ApiService _apiService;

        public AppointmentDetailsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var appointmentDetails = await _apiService.GetAsync<List<AppointmentDetails>>("AppointmentDetails");
            return View(appointmentDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownData();
            return View(new AppointmentDetails());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentDetails appointmentDetail)
        {
            if (ModelState.IsValid)
            {
                
                if (appointmentDetail.CreatedAt == DateTime.MinValue)
                {
                    appointmentDetail.CreatedAt = DateTime.Now;
                }

                if (appointmentDetail.AppointmentDate == null)
                {
                    appointmentDetail.AppointmentDate = DateTime.Today;
                }
                var response = await _apiService.PostAsync("AppointmentDetails", appointmentDetail);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm chi tiết cuộc hẹn thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Thêm chi tiết cuộc hẹn thất bại!";
                }
            }

            await LoadDropdownData();
            return View(appointmentDetail);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var appointmentDetail = await _apiService.GetAsync<AppointmentDetails>($"AppointmentDetails/{id}");

            if (appointmentDetail == null)
            {
                return NotFound();
            }

            var doctors = await _apiService.GetAsync<List<Doctors>>("Doctor");
            var services = await _apiService.GetAsync<List<ServiceModel>>("Service");
            var appointments = await _apiService.GetAsync<List<Appointments>>("Appointment");

            if (doctors == null || services == null || appointments == null)
            {
                TempData["ErrorMessage"] = "Cannot load required data.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Doctors = doctors;
            ViewBag.Services = services;
            ViewBag.Appointments = appointments;

            return View(appointmentDetail);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentDetails appointmentDetail)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiService.PutAsync($"AppointmentDetails/{appointmentDetail.Id}", appointmentDetail);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật chi tiết cuộc hẹn thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Cập nhật chi tiết cuộc hẹn thất bại.";
                return RedirectToAction(nameof(Index));
            }
            await LoadDropdownData();
            return View(appointmentDetail); 
        }

        [HttpGet]
        public async Task<JsonResult> GetAppointmentTimes(int appointmentId)
        {
            var appointment = await _apiService.GetAsync<Appointments>($"Appointment/{appointmentId}");
            if (appointment != null)
            {
                return Json(new
                {
                    startTime = appointment.AppointmentDateStart?.ToString(@"hh\:mm"),
                    endTime = appointment.AppointmentDateEnd?.ToString(@"hh\:mm")
                });
            }

            return Json(null);
        }
        private async Task LoadDropdownData()
        {
            var doctors = await _apiService.GetAsync<List<Doctors>>("Doctor");
            var services = await _apiService.GetAsync<List<ServiceModel>>("Service");
            var appointments = await _apiService.GetAsync<List<Appointments>>("Appointment");

            ViewBag.Doctors = doctors;
            ViewBag.Services = services;
            ViewBag.Appointments = appointments;
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"AppointmentDetails/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa chi tiết cuộc hẹn thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa chi tiết cuộc hẹn thất bại!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}