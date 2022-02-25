﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using TestsBaza.Repositories;

using System.Security.Claims;
namespace TestsBaza.Controllers
{   
    [ApiController]
    [Route("api/test")]
    public class TestsController : Controller
    {
        private readonly ITestsRepository _testsRepo;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<TestsController> _logger;
        public TestsController(ITestsRepository testsRepo, 
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ILogger<TestsController> logger)
        {
            _testsRepo = testsRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet("get-all-tests")]
        public IActionResult Get()
        {
            if (!_testsRepo.GetAllTests().Any()) return NotFound();
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
            return Json(new { test= "test" });
        }

        [HttpPost("add-test")]
        public async Task<IActionResult> CreateTest([FromForm] CreateTestRequestModel model)
        {
            try
            {
                _logger.LogInformation($"New create test request, TestName-{model.TestName}, IsPrivate:{model.IsPrivate}");
                if (ModelState.IsValid)
                {
                    User creator = await _userManager.GetUserAsync(HttpContext.User);

                    if (_testsRepo.GetTest(model.TestName!) is not null)
                        return Forbid();

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
                else
                {
                    var errors = ModelState.Select(entry =>
                    {
                        var query = entry.Value?.Errors.Select(e => e.ErrorMessage);
                        if (query is null || !query.Any()) return string.Empty;
                        else return query.Aggregate((x, y) => x + ", " + y);
                    });
                    return BadRequest(new { errors });
                }
            } catch(Exception e)
            {
                _logger.LogError(e.InnerException?.Message ?? e.Message);
                return BadRequest(105);
            }
        }

        [HttpPost("update-test")]
        public async Task<IActionResult> UpdateTest([FromForm] UpdateTestRequestModel model)
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