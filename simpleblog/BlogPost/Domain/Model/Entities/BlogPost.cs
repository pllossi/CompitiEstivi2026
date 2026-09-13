using System.Security.AccessControl;

namespace Domain.Model.Entities
{
    public class BlogPost
    {
        public Guid Id { get; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentException("title null or empty"); }
                _title = value;
            }
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentException("content null or empty"); }
                _content = value;
            }

        }
        public DateTime CreatedAt { get; }

        public BlogPost(Guid id, string title, string content, DateTime createdAt)
        {
            Id = id;
            Title = title;
            Content = content;
            CreatedAt = createdAt;
        }

        public BlogPost(string title, string content) : this(Guid.NewGuid(), title, content, DateTime.Now)
        { }
    }
}

