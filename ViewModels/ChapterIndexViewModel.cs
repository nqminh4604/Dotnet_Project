using System.Collections.Generic;
using Dotnet_Project.Models;

namespace Dotnet_Project.ViewModels
{
    public class ChapterIndexViewModel
    {
        public Course? Course { get; set; }
        public List<Chapter> Chapters { get; set; } = new();
        public Lesson? SelectedLesson { get; set; }
        public Exercise? SelectedExercise { get; set; }
        public int? OpenChapterIndex { get; set; }
    }
}
