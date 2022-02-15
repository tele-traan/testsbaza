using System.ComponentModel.DataAnnotations;

namespace TestsBaza.Models
{
    public class CreateTestRequestModel
    {
        [Required(ErrorMessage="Вы не ввели название теста")]
        [MinLength(4, ErrorMessage="Минимальная длина названия - 4 символа")]
        [MaxLength(35, ErrorMessage="Максимальная длина названия - 35 символов")]
        public string? TestName { get; set; }
        public IEnumerable<QuestionJsonModel> Questions { get; set; } = new List<QuestionJsonModel>();
    }
}
