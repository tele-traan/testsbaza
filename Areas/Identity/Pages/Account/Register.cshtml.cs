#nullable disable

using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace App.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public class InputModel
        {

            [Required(ErrorMessage = "Вы не ввели имя пользователя")]
            [StringLength(21, ErrorMessage ="Имя пользователя должно быть длиной от 4 до 21 символов", MinimumLength = 4)]
            [Display(Name ="Имя пользователя")]
            public string UserName { get; set; }

            [Required(ErrorMessage ="Вы не ввели электронную почту")]
            [EmailAddress(ErrorMessage ="Вы ввели некорректный адрес электронной почты")]
            [Display(Name = "Эл. почта")]
            public string Email { get; set; }

            [Required(ErrorMessage ="Вы не ввели пароль")]
            [StringLength(50, ErrorMessage = "Пароль должен быть длиной от 6 до 50 символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }
            
            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }
        }


        public void OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                //User createdUser = await _userManager.FindByNameAsync(Input.UserName);
                //await _userManager.AddClaimAsync(createdUser, new Claim(ClaimsIdentity.DefaultNameClaimType, Input.UserName));
                    
                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователем создан новый аккаунт с паролем.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId, code, returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Подтверждение электронной почты",
                        $"Подтвердите электронную почту, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>нажав здесь</a>.");
                    await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, isPersistent: true, false);

                    var createdUser = await _userManager.FindByIdAsync(userId);
                    await _userManager.AddClaimAsync(createdUser, new Claim(ClaimsIdentity.DefaultNameClaimType, Input.UserName));

                    var claims = await _userManager.GetClaimsAsync(createdUser);
                    ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    
                    return LocalRedirect(returnUrl);
                }
                else foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
            }
            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Ошибка при создании сущности '{nameof(User)}'. " +
                    $"Убедитесь, что '{nameof(User)}' не абстрактный класс и имеет конструктор без параметров, либо " +
                    $"переопределите существующую страницу в /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Стандартный UI требует поддержки отправки электронной почты");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
