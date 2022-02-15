using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using TestsBaza.Repositories;

namespace TestsBaza.Controllers
{   
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestsRepository _testsRepo;
        private readonly UserManager<User> _userManager;
        public TestsController(ITestsRepository testsRepo, UserManager<User> userManager)
        {
            _testsRepo = testsRepo;
            _userManager = userManager;
        }

        [HttpGet("alltests")]
        public IActionResult GetAllTests()
        {
            IEnumerable<TestJsonModel> allTests = _testsRepo.GetAllTests().Select(t=>new TestJsonModel
            {
                TestName = t.TestName,
                AuthorName=t.Creator.UserName,
                Questions = t.Questions.Select(q=>new QuestionJsonModel
                {
                    Question = q.Value,
                    Answer = q.Answer
                })
            });
            return Json(allTests);
        }

        [HttpGet("test{testId?}")]
        public IActionResult GetTest([FromQuery][FromRoute]int testId)
        {
            Test? test = _testsRepo.GetTest(testId);
            if (test is null) return NotFound(new { msg = $"Теста с идентификатором {testId} не существует"});
            return Json(test!);
        }

        [HttpPost("test")]
        public IActionResult GetTest([FromBody][FromForm]string testName)
        {
            Test? test = _testsRepo.GetTest(testName);
            if (test is null) return NotFound(new { msg = $"Теста с именем {testName} не существует" });
            return Json(test!);
        }
        [HttpPost("createtest")]
        public async Task<IActionResult> CreateTest([FromBody][FromForm] CreateTestRequestModel model)
        {
            string userName = User.Identity!.Name!;
            User creator = await _userManager.FindByNameAsync(userName);
            if (ModelState.IsValid) {
                Test test = new()
                {
                    Creator = creator,
                    TestName = model.TestName!
                };
                test.Questions = model.Questions.Select(q => new Question
                {
                    Test = test,
                    Value = q.Question,
                    Answer = q.Answer
                });
                _testsRepo.AddTest(test);
                return Ok();
            } else return BadRequest(ModelState);
        }

        [HttpPost("updatetest")]
        public async Task<IActionResult> UpdateTest([FromBody][FromForm] UpdateTestRequestModel model)
        {
            User? creator = await _userManager.FindByNameAsync(User.Identity!.Name!);
            Test? test = _testsRepo.GetTest(model.TestId);
            if (test is null) return NotFound(new { message = $"Теста с идентификатором {model.TestId} не существует" });
            if (creator is null || !test.Creator.Equals(creator)) return Unauthorized();
            test.TestName = model.NewTestName ?? test!.TestName;
            test.Questions = test.Questions.Concat(model.NewQuestions.Select(q=>new Question
            {
                Value = q.Question,
                Answer = q.Answer,
                Test = test
            }));
            _testsRepo.UpdateTest(test);
            return Ok();
        }

        [HttpPost("removetest")]
        public async Task<IActionResult> RemoveTest([FromBody][FromForm] int testId)
        {
            User? creator = await _userManager.FindByNameAsync(User.Identity!.Name!);
            Test? test = _testsRepo.GetTest(testId);
            if (test is null) return NotFound(new { message = $"Теста с идентификатором {testId} не существует" });
            if (creator is null || !test.Creator.Equals(creator)) return Unauthorized();
            _testsRepo.RemoveTest(test);
            return Ok();
        }
    }
}