using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using TestsBaza.Repositories;

namespace TestsBaza.Controllers
{   
    [ApiController]
    [Route("api/test")]
    public class TestsController : Controller
    {
        private readonly ITestsRepository _testsRepo;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TestsController> _logger;
        public TestsController(ITestsRepository testsRepo, UserManager<User> userManager, ILogger<TestsController> logger)
        {
            _testsRepo = testsRepo;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("get-all-tests")]
        public IActionResult Get()
        {
            if (!_testsRepo.GetAllTests().Any()) return NotFound(new {result="emptytests" });
            IEnumerable<TestJsonModel> allTests = _testsRepo.GetAllTests().Select(t=>new TestJsonModel
            {
                TestName = t.TestName!,
                AuthorName=t.Creator!.UserName,
                Questions = t.Questions.Select(q=>new QuestionJsonModel
                {
                    Question = q.Value,
                    Answer = q.Answer
                })
            });
            return Ok(allTests);
        }

        [Authorize]
        [HttpGet("get-users-tests")]
        public async Task<IActionResult> GetUsersTests()
        {
            string userName = User.Identity!.Name!;
            User user = await _userManager.FindByNameAsync(userName);
            if (!user.Tests.Any()) return NotFound(new { result="emptytests" });
            IEnumerable<TestJsonModel> tests = user.Tests.Select(t => new TestJsonModel
            {
                TestName = t.TestName!,
                AuthorName = userName,
                Questions = t.Questions.Select(q => new QuestionJsonModel
                    {
                        Question = q.Value,
                        Answer = q.Answer
                    })
            });
            return Ok(tests);
        }

        [Authorize]
        [HttpGet("get-test/{testId?}")]
        public IActionResult GetTest([FromQuery][FromRoute]int testId)
        {
            Test? test = _testsRepo.GetTest(testId);
            if (test is null) return NotFound(new { msg = $"Теста с идентификатором {testId} не существует"});
            return Json(test);
        }

        [Authorize]
        [HttpPost("get-test")]
        public IActionResult GetTest([FromBody][FromForm]string testName)
        {
            Test? test = _testsRepo.GetTest(testName);
            //if (test is null) return NotFound(new { msg = $"Теста с именем {testName} не существует" });
            return Json(new { test= "test" });
            //return Json(test!);
        }
        [Authorize]
        [HttpPost("add-test")]
        public async Task<IActionResult> CreateTest([FromForm] CreateTestRequestModel model)
        {
            _logger.LogInformation($"New create test request, TestName-{model.TestName}, IsPrivate:{model.IsPrivate}");

            string userName = User.Identity!.Name!;
            User creator = await _userManager.FindByNameAsync(userName);

            if (_testsRepo.GetTest(model.TestName!) is not null) 
                return StatusCode(403, new { message = "Тест с таким названием уже есть" });

            Test test = new()
            {
                Creator = creator,
                TestName = model.TestName!,
                IsPrivate = model.IsPrivate,
                TimeCreated = DateTime.Now
            };
            _testsRepo.AddTest(test);
            return Ok();
        }

        [HttpPost("update-test")]
        public async Task<IActionResult> UpdateTest([FromBody][FromForm] UpdateTestRequestModel model)
        {
            User? creator = await _userManager.FindByNameAsync(User.Identity!.Name!);
            Test? test = _testsRepo.GetTest(model.TestId);
            if (test is null) return NotFound(new { message = $"Теста с идентификатором {model.TestId} не существует" });
            if (creator is null || !test.Creator!.Equals(creator)) return Unauthorized();
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

        [HttpPost("remove-test")]
        public async Task<IActionResult> RemoveTest([FromBody][FromForm] int testId)
        {
            User? creator = await _userManager.FindByNameAsync(User.Identity!.Name!);
            Test? test = _testsRepo.GetTest(testId);
            if (test is null) return NotFound(new { message = $"Теста с идентификатором {testId} не существует" });
            if (creator is null || !test.Creator!.Equals(creator)) return Unauthorized();
                  _testsRepo.RemoveTest(test);
            return Ok();
        }
    }
}