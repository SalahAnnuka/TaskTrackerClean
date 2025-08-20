using System.Threading.Tasks;
using TaskTrackerClean.Application.Interfaces;


namespace Test
{
    public class Test
    {
        IProjectService _projectService;

        public Test(IProjectService projectService) {
        
            _projectService = projectService;
        }

        [Fact]
        public async Task TestThatServiceReturnsCorrectItemWhenCalledById()
        {
            int id = 1;
            var result = await _projectService.FindByIdAsync(id);
            Assert.NotNull(null);
            Assert.Equal(result.Id,id);
        }
    }
}