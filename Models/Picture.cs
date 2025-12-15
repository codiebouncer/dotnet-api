using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PropMan.Models;

public partial class Picture
{
    public Guid PictureId { get; set; } =Guid.NewGuid();

    public Guid PropertyId { get; set; }

    public string FilePath { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime DateUploaded { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public virtual Property Property { get; set; } = null!;
}
