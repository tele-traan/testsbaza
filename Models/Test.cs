namespace TestsBaza.Models
{
    public class Test
    {
        public int TestId { get; set; }
#pragma warning disable CS8618
        public string TestName { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public string CreatorId { get; set; }
        public User Creator { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Test) return false;
            Test test = (obj as Test)!;
            return test.TestId == TestId && test.TestName == TestName
                && Questions.SequenceEqual(test.Questions);

        }
        public override int GetHashCode() => TestId.GetHashCode();
    }
}
