using System;
using System.ComponentModel.DataAnnotations;

namespace AvitalERP.Models
{
    public class ProyectoPasoEvidencia
    {
        public int Id { get; set; }

        public int ProyectoPasoId { get; set; }
        public ProyectoPaso ProyectoPaso { get; set; } = default!;

        [Required]
        [StringLength(400)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(255)]
        public string? OriginalFileName { get; set; }

        [StringLength(120)]
        public string? ContentType { get; set; }

        public long SizeBytes { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}
