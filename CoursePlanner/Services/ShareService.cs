using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoursePlanner.Services
{
    public interface IShareService
    {
        Task ShareCourseNotesAsync(string courseTitle, string? notes);
    }

    class ShareService : IShareService
    {
        private readonly IShare _share;

        public ShareService(IShare share)
        {
            _share = share;
        }

        public async Task ShareCourseNotesAsync(string courseTitle, string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                throw new InvalidOperationException("There are no notes to share.");

            string shareText = $"{courseTitle} Notes\n\n{notes}";

            await _share.RequestAsync(new ShareTextRequest
            {
                Title = $"Share {courseTitle} Notes",
                Subject = $"{courseTitle} Notes",
                Text = shareText
            });
        }
    }
}