using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Domain
{
    public class FilesUpload
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? ProductId { get; set; }
        public string Name { get; private set; }
        public DateTime Data { get; private set; }
        public bool? IsDefault { get; set; }

        private FilesUpload()
        {
        }

        public FilesUpload(Guid userId, Guid? productId, string name,
            DateTime data): this(new Guid(),userId,productId, name, data)
        {
        }

        public FilesUpload(Guid id, Guid userId, Guid? productId, string name, DateTime data)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ProductId = productId;
            Name = name;
            Data = data;
        }
    }
}
