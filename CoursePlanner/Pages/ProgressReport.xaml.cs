using CoursePlanner.Data;
using CoursePlanner.Models;
using CoursePlanner.Pages;
using CoursePlanner.Services;
using CoursePlanner.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoursePlanner.Reports;

namespace CoursePlanner.Pages;

public partial class ProgressReport : ContentPage, INotifyPropertyChanged
{
    //===== Data =====
    private readonly AppDatabase _database;
    private readonly IShareService _shareService;


    private List<CourseReportItem> _allCourses = new();

    public ObservableCollection<CourseReportItem> FilteredCourses { get; set; } = new();

    public ObservableCollection<TermCompletionReportItem> TermCompletionItems { get; set; } = new();

    public string CourseReportTitle =>
        FilteredCourses.Any() ? FilteredCourses.First().Title : "No Courses Yet";

    public string TermReportTitle =>
         TermCompletionItems.Any() ? TermCompletionItems.First().Title : "No Term Completion Yet";

    public string ReportDate =>
        TermCompletionItems.Any() ? TermCompletionItems.First().Date : DateTime.Now.ToString("MM/dd/yyyy");

    //===== INotifyPropertyChanged Implementation =====

    public new event PropertyChangedEventHandler? PropertyChanged;

    //==== Constructor ====
    public ProgressReport(AppDatabase database, IShareService shareService)
    {
        InitializeComponent();
        _database = database;
        _shareService = shareService;
        BindingContext = this;
    }

    //==== Helper Methods ====
    private async Task LoadReportDataAsync()
    {
        try
        {
            var terms = await _database.GetTermsAsync();

            _allCourses.Clear();
            FilteredCourses.Clear();
            TermCompletionItems.Clear();

            foreach (var term in terms)
            {
                var courses = await _database.GetCoursesByTermAsync(term.TermId);

                // Course Report
                foreach (var course in courses)
                {
                    var courseReportItem = new CourseReportItem
                    {
                        TermTitle = term.Title,
                        TermId = term.TermId,
                        CourseId = course.CourseId,
                        CourseName = course.Name,
                        CourseStatus = course.Status,
                        CourseDueDate = course.DueDate
                    };

                    courseReportItem.Title = courseReportItem.GetReportType();
                    courseReportItem.Date = courseReportItem.GetReportDate().ToString("MM/dd/yyyy");

                    _allCourses.Add(courseReportItem);
                }

                // Term Completion Report
                int totalCourses = courses.Count;

                int completedCourses = courses.Count(c =>
                    !string.IsNullOrWhiteSpace(c.Status) &&
                    c.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase));

                int percent = totalCourses == 0
                    ? 0
                    : (int)Math.Round((double)completedCourses / totalCourses * 100);

                var termCompletionReportItem = new TermCompletionReportItem
                {
                    TermName = term.Title,
                    TermId = term.TermId,
                    DateRange = $"{term.StartDate:MM/dd/yyyy} - {term.EndDate:MM/dd/yyyy}",
                    CompletionPercentage = percent
                };

                termCompletionReportItem.Title = termCompletionReportItem.GetReportType();
                termCompletionReportItem.Date = termCompletionReportItem.GetReportDate().ToString("MM/dd/yyyy");

                TermCompletionItems.Add(termCompletionReportItem);
            }

            OnPropertyChanged(nameof(TermCompletionItems));
            OnPropertyChanged(nameof(TermReportTitle));

            ApplyFilter(SearchEntry?.Text);
        }
        catch
        {
            await DisplayAlert("Error", "Failed to load report data.", "OK");
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReportDataAsync();
    }

    // ===== Events =====
    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void ApplyFilter(string? searchText)
    {
        IEnumerable<CourseReportItem> results = _allCourses;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string search = searchText.Trim();

            results = _allCourses.Where(c =>
                c.TermTitle.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.CourseName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.CourseStatus.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.CourseDueDate.ToString("MM/dd/yyyy").Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        FilteredCourses = new ObservableCollection<CourseReportItem>(results);
        OnPropertyChanged(nameof(FilteredCourses));
        OnPropertyChanged(nameof(CourseReportTitle));
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilter(e.NewTextValue);
    }

    private async void TermCompletionCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as TermCompletionReportItem;

            if (selectedItem != null)
            {
                var term = await _database.GetTermByIdAsync(selectedItem.TermId);

                if (term != null)
                {
                    await Navigation.PushAsync(new TermDetails(_database, term, _shareService));
                }
                TermCompletionCollectionView.SelectedItem = null;
            }
        }
        catch
        {
            await DisplayAlert("Error", "Failed to load term details.", "OK");
            TermCompletionCollectionView.SelectedItem = null;
            return;
        }
    }

    private async void CoursesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var selectedCourse = e.CurrentSelection.FirstOrDefault() as CourseReportItem;

            if (selectedCourse != null)
            {
                var course = await _database.GetCourseByIdAsync(selectedCourse.CourseId);

                if (course != null)
                {
                    var term = await _database.GetTermByIdAsync(course.TermId);

                    if (term != null)
                    {
                        await Navigation.PushAsync(new CourseDetails(_database, course, term, _shareService));
                    }
                }
            }
            CoursesCollectionView.SelectedItem = null;
        }
        catch
        {
            await DisplayAlert("Error", "Failed to load course details.", "OK");
            CoursesCollectionView.SelectedItem = null;
            return;
        }
       
    }
}