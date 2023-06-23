using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class SalaryMst
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int EmployeeCode { get; set; }

    public decimal Ctc { get; set; }

    public decimal BasicSalary { get; set; }

    public decimal FixedHra { get; set; }

    public decimal FixedConveyanceAllowance { get; set; }

    public decimal FixedMedicalAllowance { get; set; }

    public decimal AdditionalHraallowance { get; set; }

    public decimal TotalDays { get; set; }

    public decimal PaidLeave { get; set; }

    public decimal UnpaidLeave { get; set; }

    public decimal PayableDays { get; set; }

    public decimal GrossSalaryPayable { get; set; }

    public decimal Basic { get; set; }

    public decimal Hra { get; set; }

    public decimal EmployerContributionToPf { get; set; }

    public decimal ConveyanceAllowance { get; set; }

    public decimal MedicalAllowance { get; set; }

    public decimal Hraallowance { get; set; }

    public decimal FlexibleAllowance { get; set; }

    public decimal IncentiveAllowance { get; set; }

    public decimal TotalEarning { get; set; }

    public decimal Pfemployer { get; set; }

    public decimal Pfemployee { get; set; }

    public decimal Esicemployer { get; set; }

    public decimal Esicemployee { get; set; }

    public decimal ProfessionalTax { get; set; }

    public decimal Advances { get; set; }

    public decimal IncomeTax { get; set; }

    public decimal TotalDeduction { get; set; }

    public decimal NetPayable { get; set; }

    public string SalartMonth { get; set; } = null!;

    public decimal SalaryYear { get; set; }

    public bool IsDelete { get; set; }

    public int CreatedBy { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
