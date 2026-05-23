using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoursePlanner.Models;
using CoursePlanner.Data;

namespace CoursePlanner.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDatabase database)
        {
            if (await database.GetTermsAsync() != null && (await database.GetTermsAsync()).Count > 0)
            {
                return;
            }

            // ===== Create Term =====
            var term = new Term
            {
                Title = "Term 1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6)
            };

            await database.SaveTermAsync(term);

            // ===== Create Course =====
            var course = new Course
            {
                TermId = term.TermId,
                Name = "Software Engineering",
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddMonths(3),
                Status = "In Progress",

                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu"
            };

            await database.SaveCourseAsync(course);

            // ===== Create Assessments =====
            var objectiveAssessment = new Assessment
            {
                CourseId = course.CourseId,
                Title = "Objective Assessment",
                Type = "OA",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7)
            };

            var performanceAssessment = new Assessment
            {
                CourseId = course.CourseId,
                Title = "Performance Assessment",
                Type = "PA",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(14)
            };

            await database.SaveAssessmentAsync(objectiveAssessment);
            await database.SaveAssessmentAsync(performanceAssessment);
        }
    }
}
