using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Domain
{
    public class FilesUpload
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public DateTime Data { get; private set; }

        private FilesUpload()
        {
        }

        public FilesUpload(Guid id, Guid userId, string name, DateTime data)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Data = data;
        }
    }
}
