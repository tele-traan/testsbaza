namespace TestsBaza.Models
{
    public class TestJsonModel
    {
#pragma warning disable CS8618
        public string TestName { get; set; }
        public IEnumerable<QuestionJsonModel> Questions { get; set; }
        public string AuthorName { get; set; }
    }
}
