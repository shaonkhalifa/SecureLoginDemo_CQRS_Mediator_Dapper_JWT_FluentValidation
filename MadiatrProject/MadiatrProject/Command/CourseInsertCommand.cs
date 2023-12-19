using FluentValidation;
using MadiatrProject.DbContexts;
using MadiatrProject.Model;
using MediatR;

namespace MadiatrProject.Command;

public class CourseInsertCommand:IRequest<int>
{
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public string? CourseDescription { get; set; }
    public DateTime Durations { get; set; }
    private class CourseInsertCommandHandler : IRequestHandler<CourseInsertCommand, int>
    {
        private readonly SDBContext _dBContext;

        public CourseInsertCommandHandler(SDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<int> Handle(CourseInsertCommand request, CancellationToken cancellationToken)
        {
            var courses = new Course
            {
                CourseName = request.CourseName,
                CourseDescription = request.CourseDescription,
                Durations = request.Durations,
            };
           await _dBContext.Course.AddAsync(courses);
           await _dBContext.SaveChangesAsync();
            return courses.CourseId;
        }
    }
}
public class CourseValidator : AbstractValidator<CourseInsertCommand>
{
    public CourseValidator()
    {
        RuleFor(a => a.CourseName)
        .NotEmpty()
        .MaximumLength(40)
        .MinimumLength(6);

        RuleFor(a => a.CourseDescription)
            .MaximumLength(250);
    }
}
