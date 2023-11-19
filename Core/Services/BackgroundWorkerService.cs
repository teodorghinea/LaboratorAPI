namespace Core.Services
{
    public class BackgroundWorkerService
    {
        private StudentService studentService { get; set; }

        public BackgroundWorkerService(StudentService studentService)
        {
            this.studentService = studentService;
        }

        public void Run()
        {
            var studentsCount = studentService.CountAllStudents();
            Console.WriteLine($"{studentsCount} students");
        }
    }
}
