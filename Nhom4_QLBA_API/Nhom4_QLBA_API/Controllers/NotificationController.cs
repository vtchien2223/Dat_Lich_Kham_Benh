using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4_QLBA_API.Models;
using Nhom4_QLBA_API.Repositories;
using System;

namespace Nhom4_QLBA_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ApplicationDbContext _context; 

        public NotificationController(INotificationRepository notificationRepository, ApplicationDbContext context)
        {
            _notificationRepository = notificationRepository;
            _context = context; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _notificationRepository.GetAllNotifications();
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var notification = await _notificationRepository.GetNotificationById(id);
            if (notification == null)
                return NotFound();
            return Ok(notification);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody] Notifications notification)
        {
            await _notificationRepository.AddNotification(notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] Notifications notification)
        {
            if (id != notification.Id)
                return BadRequest();

            await _notificationRepository.UpdateNotification(notification);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            await _notificationRepository.DeleteNotification(id);
            return NoContent();
        }

        [HttpPost("AddUpcomingNotifications")]
        public async Task<IActionResult> AddUpcomingNotifications()
        {
            var today = DateTime.UtcNow.Date;

            var upcomingAppointments = await _context.AppointmentDetails
                .Where(a => a.AppointmentDate.HasValue &&
                            a.AppointmentDate.Value.Date == today.AddDays(1))
                .ToListAsync();

            if (!upcomingAppointments.Any())
                return NoContent(); 

            foreach (var appointment in upcomingAppointments)
            {
                var notification = new Notifications
                {
                    AppointmentDetailId = appointment.Id,
                    Message = $"Bạn có cuộc hẹn với bác sĩ vào ngày {appointment.AppointmentDate.Value:dd/MM/yyyy}.",
                    SentAt = DateTime.UtcNow,
                    UrlImage = "https://example.com/default-image.png", 
                    AppointmentDetail = appointment
                };

                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync(); 

            return Ok("Notifications for upcoming appointments added successfully");
        }

        [HttpGet("GetUserNotifications/{userName}")]
        public async Task<IActionResult> GetUserNotifications(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest("UserName không hợp lệ.");

            var notifications = await _context.Notifications
                .Include(n => n.AppointmentDetail)
                .Where(n => n.AppointmentDetail.UserName == userName) 
                .OrderByDescending(n => n.SentAt)
                .Select(n => new
                {
                    n.Message,
                    n.SentAt,
                    AppointmentDate = n.AppointmentDetail.AppointmentDate,
                })
                .ToListAsync();

            if (!notifications.Any())
                return NoContent(); 

            return Ok(notifications); 
        }
    }
}
