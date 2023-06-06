using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class InterviewMst
{
    public int Id { get; set; }

    public int ResumeFileUploadId { get; set; }

    public int ResumeFileUploadDetailId { get; set; }

    public int HireStatusId { get; set; }

    public int OfferStatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
