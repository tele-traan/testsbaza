﻿namespace TestsBaza.Models
{
    public class Question
    {
#pragma warning disable CS8618
        public int QuestionId { get; set; }
        
        public string Value { get; set; }
        public string Answer { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Question) return false;
            Question question = (obj as Question)!;
            return question.QuestionId == QuestionId && question.Value == Value
                && question.Answer == Answer;
        }
        public override int GetHashCode() => QuestionId.GetHashCode();
    }
}