using System.ComponentModel.DataAnnotations;

namespace TaDaa.WebUI.ViewModels
{
    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut şifre zorunlu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunlu.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Yeni şifre en az 6 karakter olmalı.")]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre (tekrar) zorunlu.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        public string ConfirmNewPassword { get; set; }
    }
}
