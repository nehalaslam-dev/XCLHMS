ALTER PROCEDURE sp_GetPatientReport
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @PatientType NVARCHAR(50) = NULL,
    @ChallanNo NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Id AS PatientID,
        p.Name AS PatientName,
        p.Age,
        p.Gender,
        p.Address,
        p.ContactNo AS Mobile,
        p.NIC,
        pt.Name AS PatientType, -- Joining with PatientType table
        '' AS DoctorName, -- Add Doctor join if applicable
        p.AdmitDate AS VisitDate,
        c.IRCode AS ChallanNo
    FROM Pateint p -- Corrected spelling
    LEFT JOIN PatientType pt ON p.PatientTypeID = pt.Id
    LEFT JOIN IR c ON c.customerId = p.Id -- Assuming customerId refers to Patient Id in this context
    WHERE 
        (@ChallanNo IS NULL OR c.IRCode = @ChallanNo)
        AND (@StartDate IS NULL OR CAST(COALESCE(p.AdmitDate, p.CreatedDate) AS DATE) >= CAST(@StartDate AS DATE))
        AND (@EndDate IS NULL OR CAST(COALESCE(p.AdmitDate, p.CreatedDate) AS DATE) <= CAST(@EndDate AS DATE))
        AND (
            @PatientType IS NULL OR @PatientType = '' 
            OR LTRIM(RTRIM(pt.Name)) = LTRIM(RTRIM(@PatientType))
            OR (@PatientType = 'InPatient' AND pt.Name IN ('InPatient', 'Indoor', 'Indoor Patient'))
            OR (@PatientType = 'OutPatient' AND pt.Name IN ('OutPatient', 'Outdoor', 'Outdoor Patient', 'Out Patienat'))
        )
    ORDER BY COALESCE(p.AdmitDate, p.CreatedDate) ASC;
END
GO
