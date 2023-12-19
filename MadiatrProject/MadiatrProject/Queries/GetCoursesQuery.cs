using MediatR;

namespace MadiatrProject.Queries;

public class GetCoursesQuery:IRequest<List<CouresDto>>
{
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public string? CourseDescription { get; set; }
    public DateTime Durations { get; set; }

    private  class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CouresDto>>
    {
        public async Task<List<CouresDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
public class CouresDto
{
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public string? CourseDescription { get; set; }
    public DateTime Durations { get; set; }
}