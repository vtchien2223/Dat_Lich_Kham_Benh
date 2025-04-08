using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4_QLBA_API.Models;
using Nhom4_QLBA_API.Repositories;

namespace Nhom4_QLBA_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentDetailsController : ControllerBase
    {
        private readonly IAppointmentDetailsRepository _appointmentDetailsRepository;

        public AppointmentDetailsController(IAppointmentDetailsRepository appointmentDetailsRepository)
        {
            _appointmentDetailsRepository = appointmentDetailsRepository;
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointmentDetailsRepository.GetAllAppointments();
            return Ok(appointments);
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _appointmentDetailsRepository.GetAppointmentById(id);
            if (appointment == null)
                return NotFound(new { message = $"Appointment with ID {id} not found." });

            var result = new
            {
                appointment.Id,
                appointment.UserName,
                appointment.AppointmentId,
                appointment.AppointmentDate,
                appointment.CreatedAt,
                AppointmentDateStart = appointment.AppointmentDateStart?.ToString(@"hh\:mm"),
                AppointmentDateEnd = appointment.AppointmentDateEnd?.ToString(@"hh\:mm"),
                Doctor = appointment.Doctor != null ? new { appointment.Doctor.Id, appointment.Doctor.FullName } : null,
                Service = appointment.Service != null ? new { appointment.Service.Id, appointment.Service.ServiceName } : null
            };

            return Ok(result);
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDetails appointment)
        {
            if (appointment == null)
                return BadRequest("Appointment data is null.");

            await _appointmentDetailsRepository.AddAppointment(appointment);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDetails appointment)
        {
            if (id != appointment.Id)
                return BadRequest("Appointment ID mismatch.");

            var existingAppointment = await _appointmentDetailsRepository.GetAppointmentById(id);
            if (existingAppointment == null)
                return NotFound();

            await _appointmentDetailsRepository.UpdateAppointment(appointment);
            return NoContent();
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var existingAppointment = await _appointmentDetailsRepository.GetAppointmentById(id);
            if (existingAppointment == null)
                return NotFound();

            await _appointmentDetailsRepository.DeleteAppointment(id);
            return NoContent();
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpGet("UpcomingAppointments")]
        public async Task<IActionResult> GetUpcomingAppointments([FromQuery] string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest("UserName không hợp lệ.");

            var currentDate = DateTime.Now;

            var appointments = await _appointmentDetailsRepository.GetUpcomingAppointments(userName, currentDate);

            if (!appointments.Any())
                return NoContent(); 

            return Ok(appointments);
        }

        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpGet("GetAppointmentsByUser")]
        public async Task<IActionResult> GetAppointmentsByUser([FromQuery] string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest("UserName không hợp lệ.");

            var userAppointments = await _appointmentDetailsRepository.GetAppointmentsByUser(userName);

            if (!userAppointments.Any())
            {
                return NoContent(); 
            }

            return Ok(userAppointments);
        }
        [Authorize(Policy = "CanManageAppointmentDetails")]
        [HttpGet("GetAppointmentTimes/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentTimes(int appointmentId)
        {
            var appointment = await _appointmentDetailsRepository.GetAppointmentTimesById(appointmentId);
            if (appointment == null)
            {
                return NotFound(new { message = $"Appointment with ID {appointmentId} not found." });
            }

            return Ok(new
            {
                startTime = appointment.AppointmentDateStart?.ToString(@"hh\:mm"),
                endTime = appointment.AppointmentDateEnd?.ToString(@"hh\:mm")
            });
        }


    }
}
