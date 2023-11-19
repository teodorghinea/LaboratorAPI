using Core.Dtos;
using DataLayer;
using DataLayer.Dtos;
using DataLayer.Entities;
using DataLayer.Enums;
using DataLayer.Mapping;
using Infrastructure.Exceptions;
using DataLayer.Repositories;

namespace Core.Services
{
    public class StudentService
    {
        private readonly UnitOfWork unitOfWork;

        private AuthorizationService authService { get; set; }

        public StudentService(UnitOfWork unitOfWork, AuthorizationService authService)
        {
            this.unitOfWork = unitOfWork;
            this.authService = authService;
        }

        public void Register(RegisterDto registerData)
        {
            if (registerData == null)
            {
                return;
            }

            var existingClass = unitOfWork.Classes.GetById(registerData.ClassId);
            if (existingClass == null)
                throw new ResourceMissingException($"Class with id {registerData.ClassId} doesn't exist");
            var hashedPassword = authService.HashPassword(registerData.Password);

            var student = new Student
            {
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                Email = registerData.Email,
                PasswordHash = hashedPassword,
                ClassId = registerData.ClassId
            };

            unitOfWork.Students.Insert(student);
            unitOfWork.SaveChanges();
        }

        public string Validate(LoginDto payload)
        {
            var student = unitOfWork.Students.GetByEmail(payload.Email);

            var passwordFine = authService.VerifyHashedPassword(student.PasswordHash, payload.Password);

            if (passwordFine)
            {
                var role = GetRole(student);

                return authService.GetToken(student, role);
            }
            else
            {
                return null;
            }

        }

        public string GetRole(Student student)
        {
            if(student.Email == "alexandra.donici@cst.ro")
            {
                return "Teacher";
            }
            else
            {
                return "Student";
            }
        }

        public List<Student> GetAll()
        {
            var results = unitOfWork.Students.GetAll();

            return results;
        }

        public StudentDto GetById(int studentId)
        {
            var student = unitOfWork.Students.GetById(studentId);

            var result = student.ToStudentDto();

            return result;
        }

        public bool EditName(StudentUpdateDto payload)
        {
            if (payload == null || payload.FirstName == null || payload.LastName == null)
            {
                return false;
            }

            var result = unitOfWork.Students.GetById(payload.Id);
            if (result == null) return false;

            result.FirstName = payload.FirstName;
            result.LastName = payload.LastName;

            return true;
        }

        public GradesByStudent GetGradesById(int studentId, CourseType courseType)
        {
            var studentWithGrades = unitOfWork.Students.GetByIdWithGrades(studentId, courseType);

            var result = new GradesByStudent(studentWithGrades);

            return result;
        }

        public List<string> GetClassStudents(int classId)
        {
            var students = unitOfWork.Students.GetClassStudents(classId);

            //var results = students.ToStudentDtos();

            return students;
        }

        public Dictionary<int, List<Student>> GetGroupedStudents()
        {
            var results = unitOfWork.Students.GetGroupedStudents();

            return results;
        }

        public int CountAllStudents()
        {
            var result = unitOfWork.Students.CountAllStudents();
                
            return result;
        }
    }
}
