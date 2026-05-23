using CoursePlanner.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoursePlanner.Utilities;
using CoursePlanner.Services;

namespace CoursePlanner.Data
{
    public class AppDatabase
    {
        // ===== Database Connection ======
        private SQLiteAsyncConnection? _database;

        public async Task Init()
        {
            if (_database is not null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "courseplannerDB.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<Assessment>();
        }

        // ===== Methods ======
        // USER METHODS
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            await Init();

            return await _database!
                .Table<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await Init();

            return await _database!.InsertAsync(user);
        }

        // TERM METHODS
        public async Task<List<Term>> GetTermsAsync()
        {
            await Init();
            return await _database!.Table<Term>().ToListAsync();
        }

        public async Task<Term?> GetTermByIdAsync(int termId)
        {
            await Init();
            return await _database!.Table<Term>()
                                   .FirstOrDefaultAsync(t => t.TermId == termId);
        }

        public async Task<int> SaveTermAsync(Term term)
        {
            await Init();

            if (term.TermId != 0)
                return await _database!.UpdateAsync(term);

            return await _database!.InsertAsync(term);
        }

        public async Task<int> DeleteTermAsync(Term term)
        {
            await Init();
            return await _database!.DeleteAsync(term);
        }

        // COURSE METHODS
        public async Task<List<Course>> GetCoursesByTermAsync(int termId)
        {
            await Init();
            return await _database!.Table<Course>()
                                   .Where(c => c.TermId == termId)
                                   .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            await Init();
            return await _database!.Table<Course>()
                                   .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await Init();

            if (course.CourseId != 0)
                return await _database!.UpdateAsync(course);

            return await _database!.InsertAsync(course);
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await Init();
            return await _database!.DeleteAsync(course);
        }

        // ASSESSMENT METHODS
        public async Task<List<Assessment>> GetAssessmentsByCourseAsync(int courseId)
        {
            await Init();

            return await _database!.Table<Assessment>()
                                   .Where(a => a.CourseId == courseId)
                                   .ToListAsync();
        }

        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            await Init();

            if (assessment.AssessmentId != 0)
                return await _database!.UpdateAsync(assessment);

            return await _database!.InsertAsync(assessment);
        }

        public async Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            await Init();
            return await _database!.DeleteAsync(assessment);
        } 
    }
}
