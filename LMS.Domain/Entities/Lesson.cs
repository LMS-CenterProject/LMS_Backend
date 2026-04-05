using LMS.Domain.Enums;
using LMS.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Entities
{
    public sealed class Lesson : Entity
    {
        private Lesson() { }

        public Guid SectionId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string ContentUrl { get; private set; } = string.Empty;
        public ContentType ContentType { get; private set; }
        public int DurationSeconds { get; private set; }
        public int OrderIndex { get; private set; }
        public bool IsFreePreview { get; private set; }

        // Navigation properties
        public Section Section { get; private set; } = null!;
        public ICollection<LessonProgress> Progresses { get; private set; } = [];

        // ── Factory method ───────────────────────────────────────

        public static Lesson Create(
            Guid sectionId,
            string title,
            string contentUrl,
            ContentType contentType,
            int durationSeconds,
            int orderIndex,
            bool isFreePreview = false)
        {
            return new Lesson
            {
                SectionId = sectionId,
                Title = title.Trim(),
                ContentUrl = contentUrl.Trim(),
                ContentType = contentType,
                DurationSeconds = durationSeconds,
                OrderIndex = orderIndex,
                IsFreePreview = isFreePreview
            };
        }

        // ── Business methods ─────────────────────────────────────

        public void Update(
            string title,
            string contentUrl,
            ContentType contentType,
            int durationSeconds,
            int orderIndex,
            bool isFreePreview)
        {
            Title = title.Trim();
            ContentUrl = contentUrl.Trim();
            ContentType = contentType;
            DurationSeconds = durationSeconds;
            OrderIndex = orderIndex;
            IsFreePreview = isFreePreview;
        }

        public void ToggleFreePreview() => IsFreePreview = !IsFreePreview;
    }
}
