using System;
using System.ComponentModel.DataAnnotations;

namespace XCLHMS.Models
{
    public partial class Manufacture
    {
        [Key]
        public int Id { get; set; }
        public string ManufactureName { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
    }

    public partial class Brands
    {
        [Key]
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
    }

    public partial class Issuance
    {
        [Key]
        public int SNO { get; set; }
        public string Head { get; set; }
        public int? ProductId { get; set; }
        public decimal? Qty { get; set; }
        public DateTime? Date { get; set; }

        public virtual Products Products { get; set; }
    }

    public partial class AU
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public partial class Dosage
    {
        [Key]
        public int Id { get; set; }
        public string DosageName { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public int? AUId { get; set; }
    }
}
