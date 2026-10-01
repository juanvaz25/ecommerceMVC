using EComerce.Application.Interfaces;
using EComerce.Infrastructure.Identity;
using EComerce.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EComerce.Web.Controllers;

/// <summary>
/// Controlador del módulo de chat privado.
/// Solo accesible para usuarios autenticados.
/// </summary>
[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(IChatService chatService, UserManager<ApplicationUser> userManager)
    {
        _chatService = chatService;
        _userManager = userManager;
    }

    /// <summary>
    /// Página principal del chat. Muestra lista de usuarios disponibles para chatear.
    /// </summary>
    public IActionResult Index()
    {
        var currentUserId = _userManager.GetUserId(User)!;

        // Lista tipada de todos los usuarios excepto el actual
        var users = _userManager.Users
            .Where(u => u.Id != currentUserId && u.IsActive)
            .Select(u => new UserSummaryViewModel
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FirstName = u.FirstName,
                LastName = u.LastName
            })
            .ToList();

        ViewBag.Users = users;
        ViewBag.CurrentUserId = currentUserId;
        ViewBag.CurrentUserName = User.Identity!.Name;

        return View();
    }

    /// <summary>
    /// Carga el historial de conversación con un usuario específico (llamada AJAX).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Conversation(string receiverId, int page = 1)
    {
        var currentUserId = _userManager.GetUserId(User)!;

        var result = await _chatService.GetConversationAsync(currentUserId, receiverId, page, 50);

        if (result.IsFailure)
            return BadRequest(result.Error);

        // Marcar como leídos los mensajes recibidos
        await _chatService.MarkAsReadAsync(currentUserId, receiverId);

        return Json(result.Value);
    }
}
